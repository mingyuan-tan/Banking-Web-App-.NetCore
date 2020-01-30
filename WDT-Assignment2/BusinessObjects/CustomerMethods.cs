using SimpleHashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WDT_Assignment2.Models;

namespace WDT_Assignment2.BusinessObjects
{
    public class CustomerMethods
    {
        // logic for changing password
        public void ChangePassword(Login login, string password)
        {
            var passwordHash = PBKDF2.Hash(password);
            login.Password = passwordHash;
            login.ModifyDate = DateTime.UtcNow;
        }
    }
}
