using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GmailRegistrationDemo.Data.Models
{
    // ViewModel for Step 3: Choose Gmail Address.
    public class RegisterStep3ViewModel
    {
        [Required(ErrorMessage = "Gmail address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        // Initialize SuggestedEmails to prevent null references
        public List<string> SuggestedEmails { get; set; } = new List<string>();

        // Selected Suggested Email
        public string? SuggestedEmail { get; set; }

        // Custom Email Input 
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Remote(action: "IsEmailAvailable", controller: "RemoteValidation", ErrorMessage = "Email is already in use.")]
        public string? CustomEmail { get; set; }
    }
}
