using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailRegistrationDemo.Data.Models
{

    // ViewModel for Step 8: Review Account Information.
    public class RegisterStep8ViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string RecoveryEmail { get; set; }
    }

}
