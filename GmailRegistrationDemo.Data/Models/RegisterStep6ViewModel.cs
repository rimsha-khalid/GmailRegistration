using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailRegistrationDemo.Data.Models
{
    // ViewModel for Step 6: Enter Verification Code.
    public class RegisterStep6ViewModel
    {
        [Required(ErrorMessage = "Verification code is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit code.")]
        [Display(Name = "Verification Code")]
        public string VerificationCode { get; set; }
    }
}
