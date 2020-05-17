using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecureMailApp.Services
{
    public class DigitalSignature
    {
        private readonly string _pathToPrivateKey;
        private readonly string _pathToPublicKey;
        public DigitalSignature(string senderEmail)
        {
            _pathToPrivateKey = $"storage/{senderEmail}/privateKey{senderEmail}.xml";
            _pathToPublicKey = $"storage/{senderEmail}/publicKey{senderEmail}.xml";
        }

        public byte[] SignData(byte[] hashOfDataToSign)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(File.ReadAllText(_pathToPrivateKey));

                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                rsaFormatter.SetHashAlgorithm("SHA256");

                return rsaFormatter.CreateSignature(hashOfDataToSign);
            }
        }

        public bool VerifySignature(byte[] hashOfDataToSign, byte[] signature)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(File.ReadAllText(_pathToPublicKey));

                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");

                return rsaDeformatter.VerifySignature(hashOfDataToSign, signature);
            }
        }
    }
}
