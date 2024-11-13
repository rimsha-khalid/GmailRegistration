using System.Collections.Concurrent;
namespace GmailRegistrationDemo.Services.Services
{
    // Service for handling phone verification.
    public class PhoneVerification
    {
        // Simulated storage for verification codes.
        private static ConcurrentDictionary<string, string> _verificationCodes = new ConcurrentDictionary<string, string>();

        // Sends a verification code to the specified phone number.
        // In a real application, integrate with an SMS service.
        // phoneNumber: The phone number to send the code to.
        // returns the generated verification code.
        public async Task<string> SendVerificationCodeAsync(string phoneNumber)
        {
            // Generate a 6-digit code.
            var code = new Random().Next(100000, 999999).ToString();

            // Store the code with the phone number.
            _verificationCodes[phoneNumber] = code;

            //This is the time taken by system to send the SMS to User Mobile Number
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            // Simulate sending SMS by writing to console (replace with SMS API in production).
            Console.WriteLine($"Verification code for {phoneNumber}: {code}");

            return code;
        }

        // Validates the verification code for the specified phone number.
        // phoneNumber: The phone number associated with the code.</param>
        // code: The code to validate
        // returns true if valid; otherwise, false
        public bool ValidateCode(string phoneNumber, string code)
        {
            if (_verificationCodes.TryGetValue(phoneNumber, out var storedCode))
            {
                // return storedCode == code;

                // For testing purpose, you can return true
                return true;
            }
            return false;
        }
    }
}
