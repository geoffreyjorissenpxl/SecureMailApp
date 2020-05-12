using System.Collections.Generic;
using SecureMailApp.Entities;

namespace SecureMailApp.Controllers
{
    internal interface IMessageRepository
    {
        IEnumerable<Message> GetMessages();
    }
}