using Microsoft.AspNetCore.Mvc;
using GmailRegistrationDemo.Data.Models;
using GmailRegistrationDemo.Services.Services;
using Microsoft.EntityFrameworkCore;
using global::GmailRegistrationDemo.Data.Data;
using global::GmailRegistrationDemo.Data.Models;
using global::GmailRegistrationDemo.Services.Services;


namespace GmailRegistrationDemo.Controllers
{
    // Controller responsible for managing the multi-step registration process.
    public class RegistrationController : Controller
    {
        private readonly GmailDBContext _context;
        private readonly GenerateEmailSuggestions _generateSuggestions;
        private readonly PhoneVerification _phoneVerification;

        public RegistrationController(
            GmailDBContext context,
            GenerateEmailSuggestions generateSuggestions,
            PhoneVerification phoneVerification)
        {
            _context = context;
            _generateSuggestions = generateSuggestions;
            _phoneVerification = phoneVerification;
        }

        /// Displays Step 1: Enter First and Last Name.
        [HttpGet]
        public IActionResult Step1()
        {
            try
            {
                // Retrieve existing first and last names from session, if any
                var model = new RegisterStep1ViewModel
                {
                    FirstName = HttpContext.Session.GetString("FirstName"),
                    LastName = HttpContext.Session.GetString("LastName")
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on logging setup)
                return View("Error");
            }
        }

        // Handles submission of Step 1: First and Last Name.
        [HttpPost]
        public IActionResult Step1(RegisterStep1ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Store first and last names in session
                    HttpContext.Session.SetString("FirstName", model.FirstName);
                    HttpContext.Session.SetString("LastName", model.LastName);
                    return RedirectToAction("Step2");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Displays Step 2: Enter Date of Birth and Gender.
        [HttpGet]
        public IActionResult Step2()
        {
            try
            {
                var model = new RegisterStep2ViewModel();

                // Pre-populate Date of Birth from session, if available
                var dateOfBirthString = HttpContext.Session.GetString("DateOfBirth");

                if (!string.IsNullOrEmpty(dateOfBirthString))
                {
                    var dateOfBirth = DateTime.Parse(dateOfBirthString);
                    model.Month = dateOfBirth.ToString("MMMM");
                    model.Day = dateOfBirth.Day;
                    model.Year = dateOfBirth.Year;
                }

                // Pre-populate Gender from session, if available
                var genderString = HttpContext.Session.GetString("Gender");
                if (!string.IsNullOrEmpty(genderString))
                {
                    model.Gender = Enum.Parse<Gender>(genderString);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Handles submission of Step 2: Date of Birth and Gender.
        [HttpPost]
        public IActionResult Step2(RegisterStep2ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Combine Month, Day, Year into a single DateOfBirth string
                    var dateOfBirth = $"{model.Month} {model.Day}, {model.Year}";

                    // Store DateOfBirth and Gender in session
                    HttpContext.Session.SetString("DateOfBirth", dateOfBirth);
                    HttpContext.Session.SetString("Gender", model.Gender.ToString());

                    return RedirectToAction("Step3");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Displays Step 3: Choose Gmail Address.
        [HttpGet]
        public async Task<IActionResult> Step3()
        {
            try
            {
                // Retrieve first and last names from session
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");

                // If first or last name is missing, redirect to Step1
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    return RedirectToAction("Step1");
                }

                // Generate base email (e.g., john.doe@example.com)
                var baseEmail = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";

                // Generate suggested emails
                // 1: pranaya.rout123@example.com 
                // 2: pranaya.rout456@example.com 
                var suggestedEmails = await _generateSuggestions.GenerateUniqueEmailsAsync(baseEmail, 3);

                // Retrieve the selected email from the session
                var selectedEmail = HttpContext.Session.GetString("Email"); //pranaya.rout@example.com 

                // Determine if the selected email is one of the suggested emails or a custom email
                var model = new RegisterStep3ViewModel
                {
                    SuggestedEmails = suggestedEmails,
                    Email = selectedEmail // This will be used to set the selected email in the view
                };

                // Check if the selected email is one of the suggested emails
                if (suggestedEmails.Contains(selectedEmail))
                {
                    model.SuggestedEmail = selectedEmail; //pranaya.rout123 @example.com
                }
                else if (!string.IsNullOrEmpty(selectedEmail))
                {
                    // If it's a custom email
                    model.SuggestedEmail = "createOwn";
                    model.CustomEmail = selectedEmail;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Handles submission of Step 3: Choose Gmail Address.
        [HttpPost]
        public async Task<IActionResult> Step3(RegisterStep3ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if the email already exists (server-side validation)
                    var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());
                    if (emailExists)
                    {
                        // Add model error if email is already taken
                        ModelState.AddModelError("Email", "This email address is already taken. Please choose another one.");

                        // Regenerate email suggestions
                        var firstName = HttpContext.Session.GetString("FirstName");
                        var lastName = HttpContext.Session.GetString("LastName");

                        var baseEmail = $"{firstName?.ToLower()}.{lastName?.ToLower()}@example.com";
                        model.SuggestedEmails = await _generateSuggestions.GenerateUniqueEmailsAsync(baseEmail, 3);

                        return View(model);
                    }

                    // Save the selected email to session
                    HttpContext.Session.SetString("Email", model.Email);

                    // Proceed to the next step
                    return RedirectToAction("Step4");
                }

                // If model state is invalid, regenerate suggestions to display again
                var userFirstName = HttpContext.Session.GetString("FirstName");
                var userLastName = HttpContext.Session.GetString("LastName");

                var baseEmailSuggestion = $"{userFirstName?.ToLower()}.{userLastName?.ToLower()}@gmail.com";
                model.SuggestedEmails = await _generateSuggestions.GenerateUniqueEmailsAsync(baseEmailSuggestion, 3);

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Displays Step 4: Create a Strong Password.
        [HttpGet]
        public IActionResult Step4()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Handles submission of Step 4: Create a Strong Password.
        [HttpPost]
        public IActionResult Step4(RegisterStep4ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Store the password in session
                    HttpContext.Session.SetString("Password", model.Password);
                    return RedirectToAction("Step5");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Displays Step 5: Enter Phone Number.
        [HttpGet]
        public IActionResult Step5()
        {
            try
            {
                var model = new RegisterStep5ViewModel();

                // Pre-populate CountryCode and PhoneNumber from session
                model.CountryCode = HttpContext.Session.GetString("CountryCode") ?? string.Empty;
                model.PhoneNumber = HttpContext.Session.GetString("PhoneNumber") != null ? Convert.ToInt64(HttpContext.Session.GetString("PhoneNumber")) : null;

                // Get the list of countries and their codes
                model.CountryCodes = RegisterStep5ViewModel.GetCountryCodes();

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Handles submission of Step 5: Enter Phone Number.
        [HttpPost]
        public async Task<IActionResult> Step5(RegisterStep5ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Combine country code and phone number
                    var fullPhoneNumber = $"{model.CountryCode}{model.PhoneNumber}";

                    HttpContext.Session.SetString("CountryCode", model.CountryCode);
                    HttpContext.Session.SetString("PhoneNumber", Convert.ToString(model.PhoneNumber.Value)); // Store only the number without country code
                    HttpContext.Session.SetString("FullPhoneNumber", fullPhoneNumber);

                    // Send verification code (simulated)
                    await _phoneVerification.SendVerificationCodeAsync(fullPhoneNumber);

                    return RedirectToAction("Step6");
                }

                // Get the list of countries and their codes in case of validation failure
                model.CountryCodes = RegisterStep5ViewModel.GetCountryCodes();

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Displays Step 6: Enter Verification Code.
        [HttpGet]
        public IActionResult Step6()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Handles submission of Step 6: Enter Verification Code.
        [HttpPost]
        public IActionResult Step6(RegisterStep6ViewModel model)
        {
            try
            {
                if (ModelState.IsValid && model.VerificationCode != null)
                {
                    var fullPhoneNumber = HttpContext.Session.GetString("FullPhoneNumber");

                    // Validate the verification code
                    if (_phoneVerification.ValidateCode(fullPhoneNumber, model.VerificationCode))
                    {
                        return RedirectToAction("Step7");
                    }
                    else
                    {
                        // Add model error if verification code is invalid
                        ModelState.AddModelError("VerificationCode", "Invalid verification code.");
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Displays Step 7: Add Recovery Email.
        [HttpGet]
        public IActionResult Step7()
        {
            try
            {
                // Retrieve existing recovery email from session, if any
                RegisterStep7ViewModel model = new RegisterStep7ViewModel();
                model.RecoveryEmail = HttpContext.Session.GetString("RecoveryEmail");

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Handles submission of Step 7: Add Recovery Email.
        [HttpPost]
        public IActionResult Step7(RegisterStep7ViewModel model)
        {
            try
            {
                if (ModelState.IsValid && model.RecoveryEmail != null)
                {
                    // Store the recovery email in session
                    HttpContext.Session.SetString("RecoveryEmail", model.RecoveryEmail);
                    return RedirectToAction("Step8");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Displays Step 8: Review Account Information.
        [HttpGet]
        public IActionResult Step8()
        {
            try
            {
                var model = new RegisterStep8ViewModel
                {
                    FullName = $"{HttpContext.Session.GetString("FirstName")} {HttpContext.Session.GetString("LastName")}",
                    Email = HttpContext.Session.GetString("Email") ?? string.Empty,
                    PhoneNumber = HttpContext.Session.GetString("FullPhoneNumber") ?? string.Empty,
                    Gender = HttpContext.Session.GetString("Gender") ?? string.Empty,
                    RecoveryEmail = HttpContext.Session.GetString("RecoveryEmail") ?? string.Empty
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Handles submission of Step 8: Review Account Information.
        [HttpPost]
        public IActionResult Step8Confirm()
        {
            try
            {
                return RedirectToAction("Step9");
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Displays Step 9: Privacy and Terms.
        [HttpGet]
        public IActionResult Step9()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Handles submission of Step 9: Privacy and Terms.
        [HttpPost]
        public IActionResult Step9(RegisterStep9ViewModel model)
        {
            try
            {
                if (ModelState.IsValid && model.Agree == true)
                {
                    return RedirectToAction("Step10");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        // Final Step: Create Account and Show Dashboard.
        [HttpGet]
        public IActionResult Step10()
        {
            try
            {
                // Retrieve all registration data from session
                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var email = HttpContext.Session.GetString("Email");
                var password = HttpContext.Session.GetString("Password");
                var dateOfBirth = HttpContext.Session.GetString("DateOfBirth");
                var gender = HttpContext.Session.GetString("Gender");
                var countryCode = HttpContext.Session.GetString("CountryCode");
                var phoneNumber = HttpContext.Session.GetString("PhoneNumber");
                var recoveryEmail = HttpContext.Session.GetString("RecoveryEmail");

                // Validate that all required data is present
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                    string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(dateOfBirth) || string.IsNullOrEmpty(gender) ||
                    string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(phoneNumber))
                {
                    // Redirect to Step1 if any required data is missing
                    return RedirectToAction("Step1");
                }

                // Create a new User object with the collected data
                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = password, // In production, ensure to hash passwords
                    DateOfBirth = DateTime.Parse(dateOfBirth),
                    Gender = Enum.Parse<Gender>(gender),
                    CountryCode = countryCode,
                    PhoneNumber = Convert.ToInt64(phoneNumber),
                    RecoveryEmail = recoveryEmail
                };

                // Save the user to the database
                _context.Users.Add(user);
                _context.SaveChanges();

                // Optionally, clear the session data after successful registration
                HttpContext.Session.Clear();

                // Display the Dashboard view with the user information
                return View("Dashboard", user);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }
    }
}
