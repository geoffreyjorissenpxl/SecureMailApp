using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecureMailApp.Services
{
    public class RSAEncryption
    {
        private readonly string _pathToPrivateKey;
        private readonly string _pathToPublicKey;

        public RSAEncryption(string receiverEmail)
        {
            _pathToPrivateKey = $"storage/{receiverEmail}/privateKey{receiverEmail}.xml";
            _pathToPublicKey = $"storage/{receiverEmail}/publicKey{receiverEmail}.xml";
        }
 
        public byte[] EncryptData(byte[] dataToEncrypt)
        {
            byte[] cipherbytes;

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(File.ReadAllText(_pathToPublicKey));

                cipherbytes = rsa.Encrypt(dataToEncrypt, false);
            }

            return cipherbytes;
        }

        public byte[] DecryptData(byte[] dataToDecrypt)
        {
            byte[] plain;

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(File.ReadAllText(_pathToPrivateKey));
                plain = rsa.Decrypt(dataToDecrypt, false);
            }

            return plain;
        }
    }
}
