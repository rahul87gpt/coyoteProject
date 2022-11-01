using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ImportModels
{
    public class CashierImportModel
    {
        public long Number { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public string Password { get; set; }
        public string AccessLevel { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string WristBandInd { get; set; }
        public string Dispname { get; set; }
        public string LeftHandTillInd { get; set; }
        public string FuelUser { get; set; }
        public string FuelPass { get; set; }
        public string Department { get; set; }
    }
}
