using BCrypt.Net; // Add this using statement

namespace hw2.Services
{
    public class PasswordHashingService
    {
        // BCrypt work factor - higher is slower but more secure. 12 is a common default.
        private const int WorkFactor = 15;

        // Creates a BCrypt hash from a given password
        // Returns the hash string which includes the salt
        public string CreatePasswordHash(string password)
        {
            // Trim whitespace before validation and hashing
            string trimmedPassword = password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(trimmedPassword))
                throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

            // Use the trimmed password for hashing
            return BCrypt.Net.BCrypt.HashPassword(trimmedPassword, WorkFactor);
        }

        // Verifies a password against a stored BCrypt hash string
        public bool VerifyPasswordHash(string password, string storedHash)
        {
            // Trim whitespace before validation and verification
            string trimmedPassword = password?.Trim() ?? string.Empty;

            // Return false for invalid inputs instead of throwing exceptions
            if (string.IsNullOrWhiteSpace(trimmedPassword))
                return false; // Invalid password input
            if (string.IsNullOrWhiteSpace(storedHash))
                return false; // Invalid stored hash input

            try
            {
                // Use the trimmed password for verification
                return BCrypt.Net.BCrypt.Verify(trimmedPassword, storedHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Handle cases where the stored hash string is not a valid BCrypt hash
                // Log this error appropriately in a real application
                return false; // Hash format is invalid
            }
            catch (BCrypt.Net.HashInformationException)
            {
                // Handle cases where the hash is recognized but potentially malformed or from a different BCrypt version
                // Log this error appropriately in a real application
                return false; // Hash format is invalid or incompatible
            }
        }
    }
}
