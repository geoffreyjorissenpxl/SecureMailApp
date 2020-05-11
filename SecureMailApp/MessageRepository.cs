using System.Collections.Generic;
using System.Linq;
using SecureMailApp.Entities;

namespace SecureMailApp.Controllers
{
    internal class MessageRepository : IMessageRepository
    {
        private SecureMailDbContext context;

        public MessageRepository(SecureMailDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Message> GetMessages()
        {
            return context.Messages.ToList();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}