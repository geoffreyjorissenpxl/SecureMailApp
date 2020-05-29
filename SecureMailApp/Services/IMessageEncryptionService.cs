using SecureMailApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureMailApp.Services
{
    public interface IMessageEncryptionService
    {

        public EncryptedMessage EncryptData(Message email, RSAEncryption rsaEncryption,
            DigitalSignature signature);

        public byte[] DecryptData(EncryptedMessage encryptedPacket, RSAEncryption rsaEncryption, DigitalSignature signature);

        public bool Compare(byte[] array1, byte[] array2);

    }
}
