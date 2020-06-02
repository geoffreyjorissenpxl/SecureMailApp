using SecureMailApp.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureMailApp.Services
{
    public class MessageEncryptionService : IMessageEncryptionService
    {
        private readonly AesEncryption _aes;
        private SecureMailDbContext _dbContext;

        public MessageEncryptionService(SecureMailDbContext secureMailDbContext)
        {
            _aes = new AesEncryption();
            _dbContext = secureMailDbContext;
        }

        public EncryptedMessage EncryptData(Message email, RSAEncryption rsaEncryption,
            DigitalSignature signature)
        {
            byte[] message = Encoding.ASCII.GetBytes(email.Text);

            var sessionKey = _aes.GenerateRandomNumber(32);

            var encryptedPacket = new EncryptedMessage
            {
                ReceiveDate = DateTime.Now,
                Iv = _aes.GenerateRandomNumber(16),
                SenderEmail = email.EmailSender,
                ReceiverEmail = email.EmailReceiver
            };

            encryptedPacket.EncryptedData = _aes.Encrypt(message, sessionKey, encryptedPacket.Iv);
            encryptedPacket.EncryptedSessionKey = rsaEncryption.EncryptData(sessionKey);

            using (var hmac = new HMACSHA256(sessionKey))
            {
                encryptedPacket.Hmac = hmac.ComputeHash(encryptedPacket.EncryptedData);
            }

            encryptedPacket.Signature = signature.SignData(encryptedPacket.Hmac);

            _dbContext.EncryptedMessages.Add(encryptedPacket);
            _dbContext.SaveChanges();
            return encryptedPacket;

        }

        public byte[] DecryptData(EncryptedMessage encryptedPacket, RSAEncryption rsaEncryption, DigitalSignature signature)
        {

            var decryptedSessionKey = rsaEncryption.DecryptData(encryptedPacket.EncryptedSessionKey);

            using (var hmac = new HMACSHA256(decryptedSessionKey))
            {
                var hmacToCheck = hmac.ComputeHash(encryptedPacket.EncryptedData);

                if (!Compare(encryptedPacket.Hmac, hmacToCheck))
                {
                    throw new CryptographicException("HMAC for decryption does not match encrypted packet.");
                }

                if (!signature.VerifySignature(encryptedPacket.Hmac, encryptedPacket.Signature))
                {
                    throw new CryptographicException("Digital Signature can not be verified.");
                }
            }

            var decryptedData = _aes.Decrypt(encryptedPacket.EncryptedData, decryptedSessionKey, encryptedPacket.Iv);

            return decryptedData;

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
