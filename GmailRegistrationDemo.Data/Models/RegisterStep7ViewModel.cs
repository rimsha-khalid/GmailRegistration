using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailRegistrationDemo.Data.Models
{

    // ViewModel for Step 7: Add Recovery Email.
    public class RegisterStep7ViewModel
    {
        [EmailAddress(ErrorMessage = "Please enter a valid recovery email address.")]
        [Display(Name = "Recovery Email (Optional)")]
        public string? RecoveryEmail { get; set; }
    }

}
