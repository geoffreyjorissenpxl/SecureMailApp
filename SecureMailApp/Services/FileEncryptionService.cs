using Microsoft.AspNetCore.Http;
using SecureMailApp.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecureMailApp.Services
{
    public class FileEncryptionService : IFileEncryptionService
    {
        private readonly AesEncryption _aes;
        private SecureMailDbContext _dbContext;

        public FileEncryptionService(SecureMailDbContext secureMailDbContext)
        {
            _aes = new AesEncryption();
            _dbContext = secureMailDbContext;
        }
        public void EncryptFile(IFormFile fileform, EncryptedMessage encryptedMessage, RSAEncryption rsaEncryption, DigitalSignature signature)
        {
            var sessionKey = _aes.GenerateRandomNumber(32);

            var encryptedFile = new EncryptedFile
            {
                Iv = _aes.GenerateRandomNumber(16),
                SenderEmail = encryptedMessage.SenderEmail,
                ReceiverEmail = encryptedMessage.ReceiverEmail,
                EncryptedMessageId = encryptedMessage.EncryptedMessageId,
                FileName = fileform.FileName,
            };

            byte[] encryptedData;

            using (var stream = new MemoryStream())
            {
                fileform.CopyToAsync(stream);
                encryptedData = stream.ToArray();
            }

            encryptedFile.EncryptedData = _aes.Encrypt(encryptedData, sessionKey, encryptedFile.Iv);
            encryptedFile.EncryptedSessionKey = rsaEncryption.EncryptData(sessionKey);

            using (var hmac = new HMACSHA256(sessionKey))
            {
                encryptedFile.Hmac = hmac.ComputeHash(encryptedFile.EncryptedData);
            }

            encryptedFile.Signature = signature.SignData(encryptedFile.Hmac);

            _dbContext.EncryptedFiles.Add(encryptedFile);
            _dbContext.SaveChanges();

           
        }
        public void DecryptData(EncryptedFile encryptedFile, RSAEncryption rsaEncryption, DigitalSignature signature)
        {
            var decryptedSessionKey = rsaEncryption.DecryptData(encryptedFile.EncryptedSessionKey);

            using (var hmac = new HMACSHA256(decryptedSessionKey))
            {
                var hmacToCheck = hmac.ComputeHash(encryptedFile.EncryptedData);

                if (!Compare(encryptedFile.Hmac, hmacToCheck))
                {
                    throw new CryptographicException("HMAC for decryption does not match encrypted packet.");
                }

                if (!signature.VerifySignature(encryptedFile.Hmac, encryptedFile.Signature))
                {
                    throw new CryptographicException("Digital Signature can not be verified.");
                }
            }

            byte[] fileInBytes = _aes.Decrypt(encryptedFile.EncryptedData, decryptedSessionKey, encryptedFile.Iv);


            if (!Directory.Exists($"storage/{encryptedFile.ReceiverEmail}/files")){
                Directory.CreateDirectory($"storage/{encryptedFile.ReceiverEmail}/files");
            }

            using (var fs = new FileStream($"storage/{encryptedFile.ReceiverEmail}/files/{encryptedFile.FileName}", FileMode.Create, FileAccess.Write))
            {
                fs.Write(fileInBytes, 0, fileInBytes.Length);
            }

         
        }
        public bool Compare(byte[] array1, byte[] array2)
        {
            var result = array1.Length == array2.Length;

            for (var i = 0; i < array1.Length && i < array2.Length; ++i)
            {
                result &= array1[i] == array2[i];
            }

            return result;
        }

    }
}
