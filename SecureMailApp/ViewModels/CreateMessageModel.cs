using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureMailApp.ViewModels
{
    public class CreateMessageModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string EmailRecipient { get; set; }

        [Required]
        [Display(Name = "Text area")]
        public string Text { get; set; }

    }
}
