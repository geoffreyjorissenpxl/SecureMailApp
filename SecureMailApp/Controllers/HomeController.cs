﻿using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureMailApp.Entities;
using SecureMailApp.Models;
using SecureMailApp.ViewModels;

namespace SecureMailApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private SecureMailDbContext _secureMailDbContext;

        public HomeController(UserManager<User> userManager, IUserClaimsPrincipalFactory<User> claimsPrincipalFactory, SecureMailDbContext secureMailDbContext)
        {
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _secureMailDbContext = secureMailDbContext;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext))
                {
                    ModelState.AddModelError("", "Wrong captcha, please register again!");
                    return View();
                }
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.Email,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationEmail = Url.Action("ConfirmEmailAddress", "Home",
                            new { token = token, email = user.Email }, Request.Scheme);
                        System.IO.File.WriteAllText("confirmations.txt", confirmationEmail);
                    }
                  
                }

                return RedirectToAction(nameof(Success));
            }

            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAddress(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Success));
                }
            }

            return RedirectToAction(nameof(Error));
        }

        [AllowAnonymous]
        public IActionResult Success()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

      

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Email is not confirmed");
                        return View();
                    }

                    if (!user.RsaKeysSet)
                    {
                        CreateRsaKeys($"Storage/{user.NormalizedEmail}/keys/publicKey{user.NormalizedEmail}.xml", $"Storage/{user.NormalizedEmail}/keys/privateKey{user.NormalizedEmail}.xml");
                        user.RsaKeysSet = true;
                        await _secureMailDbContext.SaveChangesAsync();
                    }

                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    await HttpContext.SignInAsync("Identity.Application", principal);

                    return RedirectToAction("Inbox", "Message");
                }

            }
            ModelState.AddModelError("", "Invalid Username or Password.");
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }


        private void CreateRsaKeys(string pathToPublicKey, string pathToPrivateKey)
        {

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                if (System.IO.File.Exists(pathToPublicKey))
                {
                    System.IO.File.Delete(pathToPublicKey);
                }

                if (System.IO.File.Exists(pathToPrivateKey))
                {
                    System.IO.File.Delete(pathToPrivateKey);
                }

                var keysFolder = Path.GetDirectoryName(pathToPublicKey);

                if (!Directory.Exists(keysFolder))
                {
                    Directory.CreateDirectory(keysFolder);
                }

                System.IO.File.WriteAllText(pathToPublicKey, rsa.ToXmlString(false));
                System.IO.File.WriteAllText(pathToPrivateKey, rsa.ToXmlString(true));

            }
        }
    }
}
