using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ImportModels
{
    public class UserImportModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostCode { get; set; }
        public string Gender { get; set; }
        public bool Status { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PromoPrefix { get; set; }
        public string KeypadPrefix { get; set; }
        public int Type { get; set; }
        public string UserName { get; set; }
        public string Stores { get; set; }
        public string Zones { get; set; }
        public bool AddUnlockProduct { get; set; } = false;
        public string EncryptedPassword { get; set; }


    }
}
