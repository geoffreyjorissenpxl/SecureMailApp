﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SecureMailApp.Entities
{
    public class User : IdentityUser
    {
        public bool RsaKeysSet { get; set; }

    }
}
