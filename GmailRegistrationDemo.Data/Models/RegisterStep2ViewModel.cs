using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailRegistrationDemo.Data.Models
{
    public class RegisterStep2ViewModel
    {
        [Required(ErrorMessage = "Month is required.")]
        public string Month { get; set; }
        [Required(ErrorMessage = "Day is required.")]
        [Range(1, 31, ErrorMessage = "Please enter a valid day.")]
        public int? Day { get; set; }
        [Required(ErrorMessage = "Year is required.")]
        [Range(1900, 2100, ErrorMessage = "Please enter a valid year.")]
        public int? Year { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public Gender? Gender { get; set; }
    }
}
