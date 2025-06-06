using Services.AuthenServices.InterfaceAuthen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthenServices
{
    public class PasswordHashingService : IPasswordHashingService
    {
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashResult = HashPassword(password);
            return hashResult.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
