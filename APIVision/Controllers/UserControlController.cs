using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace APIVision.Controllers
{
    /// <summary>
    /// UserControlController
    /// </summary>
    public class UserControlController
    {
        /// <summary>
        /// Password validation 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt_"></param>
        /// <param name="key_"></param>
        /// <returns></returns>
        public bool ValidatePassword(string password, string salt_, string key_)
        {
            // load salt and key from database
            byte[] salt = Convert.FromBase64String(salt_);
            byte[] key = Convert.FromBase64String(key_);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt))
            {
                byte[] newKey = deriveBytes.GetBytes(20);  // derive a 20-byte key

                if (!newKey.SequenceEqual(key))
                {
                    return false; //throw new InvalidOperationException("Password is invalid!");
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
