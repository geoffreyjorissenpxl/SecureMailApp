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
                var digitalSignature = new DigitalSignature(email.EmailSender);

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
                ViewBag.MessageDecryption = "Successfully decrypted the message";

                var encryptedFile = _secureMailDbContext.EncryptedFiles.FirstOrDefault(f => f.EncryptedMessageId == id);

                if (encryptedFile != null)
                {
                    try
                    {
                        _fileEncryptionService.DecryptData(encryptedFile, rsaEncryption, digitalSignature);
                        ViewBag.FileDecryption = "Successfully decrypted the file";
                    }
                    catch(CryptographicException e)
                    {
                        ViewBag.FileEncryption = e.Message;
                    }
                    
                }

            }
            catch (CryptographicException e)
            {
                ViewBag.Message = "Error";
                ViewBag.MessageDecryption = e.Message;
            }


            return View();
        }

        public IActionResult MessageSentSuccessfully()
        {
            return View();
        }
    }
}
