using GmailRegistrationDemo.Data.Data;
using GmailRegistrationDemo.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using GmailRegistrationDemo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailRegistrationDemo.Controllers
{
    // Controller responsible for handling remote validation requests.
    public class RemoteValidationController : Controller
    {
        private readonly GmailDBContext _context;
        private readonly GenerateEmailSuggestions _generateSuggestions;

        public RemoteValidationController(GmailDBContext context, GenerateEmailSuggestions generateSuggestions)
        {
            _context = context;
            _generateSuggestions = generateSuggestions;
        }

        // Checks if the provided email is available. If not, returns suggestions.
        // Email: The email to validate
        // Returns a JSON result indicating availability or suggestions
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsEmailAvailable(string CustomEmail)
        {
            // Check if the email is empty
            if (string.IsNullOrEmpty(CustomEmail))
            {
                return Json("Please enter a valid email address.");
            }

            // Validate the email format
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(CustomEmail))
            {
                return Json("Please enter a valid email address.");
            }

            // Check if the email is already in use (case-insensitive)
            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == CustomEmail.ToLower());

            if (emailExists)
            {
                // Optionally, provide alternative suggestions
                //var suggestedEmails = await _generateSuggestions.GenerateUniqueEmailsAsync(CustomEmail, 3);
                //var suggestions = string.Join(", ", suggestedEmails);
                //return Json($"This email address is already in use. Try one of these: {suggestions}");
                return Json($"This email address is already in use.");
            }

            // If the email is available
            return Json(true);  // Indicates success to jQuery Unobtrusive Validation
        }
    }
}