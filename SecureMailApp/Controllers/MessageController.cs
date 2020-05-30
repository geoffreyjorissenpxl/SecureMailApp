using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SecureMailApp.Services;
using SecureMailApp.ViewModels;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.IO;
using SecureMailApp.Entities;

namespace SecureMailApp.Controllers
{
    public class MessageController : Controller
    {
        private SecureMailDbContext _secureMailDbContext;
        private IMessageEncryptionService _messageEncryptionService;
        private IFileEncryptionService _fileEncryptionService;

        public MessageController(SecureMailDbContext context,
            IMessageEncryptionService hybridEncryptionService,
            IFileEncryptionService fileEncryptionService)
        {
            _secureMailDbContext = context;
            _messageEncryptionService = hybridEncryptionService;
            _fileEncryptionService = fileEncryptionService;
        }

        public IActionResult Inbox()
        {
            var model = _secureMailDbContext.EncryptedMessages.
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
                var email = new Message
                {
                    EmailSender = User.Identity.Name,
                    EmailReceiver = model.EmailReceiver,
                    Text = model.Text,
                    AttachedFile = model.File
                };

                var rsaEncryption = new RSAEncryption(email.EmailReceiver);
                var digitalSignature = new DigitalSignature(User.Identity.Name);

                var encryptedMessage = _messageEncryptionService.EncryptData(email, rsaEncryption, digitalSignature);

                if(email.AttachedFile != null)
                {
                    _fileEncryptionService.EncryptFile(email.AttachedFile, encryptedMessage, rsaEncryption, digitalSignature);
                }

                return RedirectToAction(nameof(MessageSentSuccessfully));
            }

            return View();

        }


        public IActionResult GetMessage(int id)
        {
            var encryptedMessage = _secureMailDbContext.EncryptedMessages.FirstOrDefault(e => e.EncryptedMessageId == id);

            try
            {
                var rsaEncryption = new RSAEncryption(encryptedMessage.ReceiverEmail);
                var digitalSignature = new DigitalSignature(encryptedMessage.SenderEmail);




                var decryptedMessage = _messageEncryptionService.DecryptData(encryptedMessage,
                   rsaEncryption, digitalSignature);
                ViewBag.Message = Encoding.UTF8.GetString(decryptedMessage);

                var encryptedFile = _secureMailDbContext.EncryptedFiles.FirstOrDefault(f => f.EncryptedFileId == id);

                if (encryptedFile != null)
                {
                    _fileEncryptionService.DecryptData(encryptedFile, rsaEncryption, digitalSignature);
                }

            }
            catch (CryptographicException e)
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
