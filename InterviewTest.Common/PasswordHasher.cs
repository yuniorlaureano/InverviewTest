using System.Security.Cryptography;

namespace InterviewTest.Common
{
    public class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; // 256 bits
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Create the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Combine the salt and hash
            byte[] hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

            // Convert to base64-encoded string
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Extract the salt from the hashed password
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Compute the hash of the given password using the salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Compare the computed hash with the hash from the stored password
            for (int i = 0; i < KeySize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }

            return true;
        }
    }
}
