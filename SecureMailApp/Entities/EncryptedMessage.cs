﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureMailApp.Entities
{
    public class EncryptedMessage
    {
        public DateTime ReceiveDate { get; set; }
        public int EncryptedMessageId { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public byte[] EncryptedSessionKey { get; set; }
        public byte[] EncryptedData { get; set; }
        public byte[] Iv { get; set; }
        public byte[] Hmac { get; set; }
        public byte[] Signature { get; set; }

        public EncryptedFile EncryptedFile { get; set; }

    }
}
