using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SecureMailApp.ViewModels
{
    public class CreateMessageModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string EmailReceiver { get; set; }

        [Required]
        [Display(Name = "Text area")]
        public string Text { get; set; }

        public IFormFile File { get; set; }




    }
}
