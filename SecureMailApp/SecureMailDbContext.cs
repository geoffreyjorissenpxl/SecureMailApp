using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureMailApp.Entities;

namespace SecureMailApp
{
    public class SecureMailDbContext : IdentityDbContext<User>
    {
      
        public DbSet<EncryptedMessage> EncryptedMessages { get; set; }
        public DbSet<EncryptedFile> EncryptedFiles { get; set; }
        public SecureMailDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {   
            builder.Entity<EncryptedMessage>().HasKey(m => m.EncryptedMessageId);
            builder.Entity<EncryptedFile>().HasKey(f => f.EncryptedFileId);

            builder.Entity<EncryptedFile>().HasOne(f => f.EncryptedMessage)
                .WithOne(m => m.EncryptedFile);
            base.OnModelCreating(builder);
        }
    }
}
