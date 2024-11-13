using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailRegistrationDemo.Data.Models
{

    // ViewModel for Step 4: Create a Strong Password.
    public class RegisterStep4ViewModel
    {
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$!%*?&])[A-Za-z\d@#$!%*?&]{8,}$",
            ErrorMessage = "Password must include uppercase, lowercase, number, and special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        // Property to toggle password visibility
        [Display(Name = "Show Password")]
        public bool ShowPassword { get; set; }
    }

}
