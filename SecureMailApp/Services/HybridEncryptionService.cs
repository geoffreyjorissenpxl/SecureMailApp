using SecureMailApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecureMailApp.Services
{
    public class HybridEncryptionService : IHybridEncryptionService
    {
        private IEnumerable<EncryptedPacket> _encryptedPackets;
        private readonly AesEncryption _aes;
        private SecureMailDbContext _dbContext;
      
        public HybridEncryptionService(SecureMailDbContext secureMailDbContext)
        {
            _encryptedPackets = new List<EncryptedPacket>();
            _aes = new AesEncryption();
            _dbContext = secureMailDbContext;
        }

        public void EncryptData(byte[] message, RSAEncryption rsaEncryption, 
            DigitalSignature signature, string senderEmail, string receiverEmail)
        {
            var sessionKey = _aes.GenerateRandomNumber(32);

            var encryptedPacket = new EncryptedPacket { ReceiveDate = DateTime.Now ,Iv = _aes.GenerateRandomNumber(16), 
                SenderEmail = senderEmail, ReceiverEmail = receiverEmail };

            encryptedPacket.EncryptedData = _aes.Encrypt(message, sessionKey, encryptedPacket.Iv);

            encryptedPacket.EncryptedSessionKey = rsaEncryption.EncryptData(sessionKey);

            using (var hmac = new HMACSHA256(sessionKey))
            {
                encryptedPacket.Hmac = hmac.ComputeHash(encryptedPacket.EncryptedData);
            }

            encryptedPacket.Signature = signature.SignData(encryptedPacket.Hmac);

            /* _encryptedMessages.ToList().Add(encryptedPacket);*/
            _dbContext.EncryptedPackets.Add(encryptedPacket);
            _dbContext.SaveChanges();

        }

        public byte[] DecryptData(EncryptedPacket encryptedPacket, RSAEncryption rsaEncryption, DigitalSignature signature)
        {
         
            var decryptedSessionKey = rsaEncryption.DecryptData(encryptedPacket.EncryptedSessionKey);

            using (var hmac = new HMACSHA256(decryptedSessionKey))
            {
                var hmacToCheck = hmac.ComputeHash(encryptedPacket.EncryptedData);

                if(!Compare(encryptedPacket.Hmac, hmacToCheck))
                {
                    throw new CryptographicException("HMAC for decryption does not match encrypted packet.");
                }

                if(!signature.VerifySignature(encryptedPacket.Hmac, encryptedPacket.Signature))
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
