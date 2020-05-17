using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecureMailApp.Entities;
using SecureMailApp.Services;
using SecureMailApp.ViewModels;

namespace SecureMailApp.Controllers
{
    public class MessageController : Controller
    {
        private SecureMailDbContext _secureMailDbContext;
        private IHybridEncryptionService _hybridEncryptionService;

        public MessageController(SecureMailDbContext context, IHybridEncryptionService hybridEncryptionService)
        {
            _secureMailDbContext = context;
            _hybridEncryptionService = hybridEncryptionService;
        }

        public IActionResult Inbox()
        {
            var model = _secureMailDbContext.EncryptedPackets.
                Where(e => e.ReceiverEmail == User.Identity.Name).OrderByDescending(e => e.ReceiveDate).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateMessage(CreateMessageModel model)
        {
            if (ModelState.IsValid)
            {
                _hybridEncryptionService.EncryptData(Encoding.ASCII.GetBytes(model.Text), new RSAEncryption(model.EmailRecipient), new DigitalSignature(User.Identity.Name), User.Identity.Name, model.EmailRecipient);
                return RedirectToAction(nameof(MessageSentSuccessfully));
            }

            return View();

        }


        public IActionResult GetMessage(int id)
        {
            var encryptedPacket = _secureMailDbContext.EncryptedPackets.FirstOrDefault(e => e.EncryptedPacketId == id);
            string message;

            try
            {
                var decryptedData = _hybridEncryptionService.DecryptData(encryptedPacket,
                   new RSAEncryption(encryptedPacket.ReceiverEmail), new DigitalSignature(encryptedPacket.SenderEmail));
                ViewBag.Message = Encoding.UTF8.GetString(decryptedData);
            }
            catch(CryptographicException e)
            {
                ViewBag.Message = e.Message.ToString();
            }

            
            return View();
        }

        public IActionResult MessageSentSuccessfully()
        {
            return View();
        }
    }
}
