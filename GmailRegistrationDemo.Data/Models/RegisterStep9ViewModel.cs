using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailRegistrationDemo.Data.Models
{

    // ViewModel for Step 9: Privacy and Terms.
    public class RegisterStep9ViewModel
    {
        [Required(ErrorMessage = "You must agree to the terms and privacy policy to proceed.")]
        [Display(Name = "I agree to the Terms and Privacy Policy")]
        public bool Agree { get; set; }
    }

}
