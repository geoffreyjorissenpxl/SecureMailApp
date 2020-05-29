using Microsoft.AspNetCore.Http;
using SecureMailApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureMailApp.Services
{
    public interface IFileEncryptionService
    {
        public void EncryptFile(IFormFile fileform, EncryptedMessage encryptedMessage, RSAEncryption rsaEncryption,
          DigitalSignature signature);

        public void DecryptData(EncryptedFile encryptedFile, RSAEncryption rsaEncryption, DigitalSignature signature);
        public bool Compare(byte[] array1, byte[] array2);
    }
}
