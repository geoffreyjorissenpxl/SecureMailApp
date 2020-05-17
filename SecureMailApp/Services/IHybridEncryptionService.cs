using SecureMailApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureMailApp.Services
{
    public interface IHybridEncryptionService
    {

        public void EncryptData(byte[] message, RSAEncryption rsaEncryption,
            DigitalSignature signature, string senderEmail, string receiverEmail);

        public byte[] DecryptData(EncryptedPacket encryptedPacket, RSAEncryption rsaEncryption, DigitalSignature signature);

        public bool Compare(byte[] array1, byte[] array2);

    }
}
