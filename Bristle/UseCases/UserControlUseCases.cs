using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    public static class UserControlUseCases
    {
        public static bool ValidatePassword(string password, string salt_, string key_)
        {
            // load salt and key from database
            byte[] salt = Convert.FromBase64String(salt_);
            byte[] key = Convert.FromBase64String(key_);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt))
            {
                byte[] newKey = deriveBytes.GetBytes(20);  // derive a 20-byte key

                if (!newKey.SequenceEqual(key))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static string GetDomainName()
        {
            return IPGlobalProperties.GetIPGlobalProperties().DomainName;
        }

        public static bool ValidateUserInDomain(string domain, string userName, string password)
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
            {
                return pc.ValidateCredentials(userName, password, ContextOptions.Negotiate);
            }
        }
    }
}
