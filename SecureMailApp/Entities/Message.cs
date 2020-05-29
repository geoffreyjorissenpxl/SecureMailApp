using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureMailApp.Entities
{
    public class Message
    {
        public string EmailSender { get; set; }
        public string EmailReceiver { get; set; }
        public string Text { get; set; }

        public IFormFile AttachedFile { get; set; }

    }
}
