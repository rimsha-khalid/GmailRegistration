using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GmailRegistrationDemo.Data.Models
{
    // ViewModel for Step 5: Enter Phone Number.
    // Implementing IValidatableObject to handle custom validation
    public class RegisterStep5ViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Country code is required.")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Display(Name = "Phone Number")]
        public long? PhoneNumber { get; set; }

        // List of country codes for the dropdown, typically provided in the view
        public IEnumerable<SelectListItem>? CountryCodes { get; set; }

        // Implementing the Validate method from IValidatableObject for custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // If the phone number is not valid for the selected country, return a validation error
            if (!IsValidPhoneNumber(CountryCode, PhoneNumber))
            {
                // Yield a validation result if the phone number is invalid
                yield return new ValidationResult("Please enter a valid phone number.", new[] { "PhoneNumber" });
            }
        }

        // Helper method to validate the phone number based on the country code
        private bool IsValidPhoneNumber(string countryCode, long? phoneNumber)
        {
            // If either the country code or phone number is null/empty, return false (invalid)
            if (string.IsNullOrEmpty(countryCode) || phoneNumber <= 0)
                return false;

            // Define regular expression patterns based on the country code
            string pattern = countryCode switch
            {
                "+1" => @"^[2-9]\d{9}$",      // USA/Canada: 10 digits, starting with digits between 2 and 9
                "+44" => @"^7\d{9}$",         // UK: 10 digits, starting with 7
                "+91" => @"^[6-9]\d{9}$",     // India: 10 digits, starting with 6-9
                "+92" => @"^[0-9]{8,11}$",         // Pakistan: 11 digits, starting with 03
                //"+92" => @"^03\d{9}$",         // Pakistan: 11 digits, starting with 03
                _ => @"^\d{7,15}$",           // Default: Allows 7-15 digits for other countries
            };

            // Return whether the phone number matches the regex pattern
            return Regex.IsMatch(Convert.ToString(phoneNumber.Value), pattern);
        }

        // Static method to generate a list of country codes for dropdown selection
        public static List<SelectListItem> GetCountryCodes()
        {
            // Returning a list of countries and their respective country codes
            return new List<SelectListItem>
            {
                 new SelectListItem { Value = "+1", Text = "US (+1)" },  // US with country code +1
                 new SelectListItem { Value = "+1", Text = "CA (+1)" },  // Canada with country code +1
                 new SelectListItem { Value = "+44", Text = "UK (+44)" }, // UK with country code +44
                 new SelectListItem { Value = "+91", Text = "IND (+91)" }, // India with country code +91
                 new SelectListItem { Value = "+92", Text = "PAK (+92)" } // Pakistan with country code +92
                // More countries can be added to this list as needed
            };
        }
    }
}
