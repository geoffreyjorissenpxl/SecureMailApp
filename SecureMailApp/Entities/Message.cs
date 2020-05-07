using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureMailApp.Entities
{
    public class Message
    {
        public string MessageId { get; set; }
        public string Text { get; set; }
        public string Sender { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
