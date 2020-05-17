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
        public DbSet<Message> Messages { get; set; }
        public DbSet<EncryptedPacket> EncryptedPackets { get; set; }


        public SecureMailDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Message>().HasKey(m => m.MessageId);
            builder.Entity<Message>().Property(m => m.Text).IsRequired();

            builder.Entity<EncryptedPacket>().HasKey(e => e.EncryptedPacketId);
            base.OnModelCreating(builder);
        }
    }
}
