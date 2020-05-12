using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecureMailApp.Entities;

namespace SecureMailApp.Controllers
{
    public class MessageController : Controller 
    {
        private IMessageRepository messageRepository;

        public MessageController(SecureMailDbContext context)
        {
            this.messageRepository = new MessageRepository(context);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Message>> GetMessages()
        {
            return messageRepository.GetMessages().ToList();
        }
    }
}
