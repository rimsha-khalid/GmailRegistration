using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
 

namespace GmailRegistrationDemo.Data.Models
{
    // Represents a user in the system.
    [Index(nameof(Email), Name = "UX_Users_Email_Unque", IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; } //PK

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public string CountryCode { get; set; }

        [Required]
        [Phone]
        public long PhoneNumber { get; set; }

        [EmailAddress]
        public string? RecoveryEmail { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}