using GmailRegistrationDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailRegistrationDemo.Data.Data
{
    public class GmailDBContext : DbContext
    {

        public GmailDBContext(DbContextOptions<GmailDBContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Gender to be stored as string
            modelBuilder.Entity<User>()
                .Property(u => u.Gender)
                .HasConversion<string>()
                .IsRequired();
        }
        // DbSet representing the Users table in the database.
        public DbSet<User> Users { get; set; }
    }
}
