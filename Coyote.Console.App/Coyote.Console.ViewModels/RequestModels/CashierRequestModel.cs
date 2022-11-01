using Coyote.Console.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class CashierRequestModel
    {
        [Required]
        public long Number { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxCashierFirstNameLength, ErrorMessage = ErrorMessages.CashierFirstNameLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxCashierSurnameLength, ErrorMessage = ErrorMessages.CashierSurnameLength)]
        public string Surname { get; set; }

        [MaxLength(MaxLengthConstants.MaxCashierAddressLength, ErrorMessage = ErrorMessages.CashierAddressLength)]
        public string Addr1 { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierAddressLength, ErrorMessage = ErrorMessages.CashierAddressLength)]
        public string Addr2 { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierAddressLength, ErrorMessage = ErrorMessages.CashierAddressLength)]
        public string Addr3 { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierPostcodeLength, ErrorMessage = ErrorMessages.CashierPostcodeLength)]
        public string Postcode { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierPhoneLength, ErrorMessage = ErrorMessages.CashierPhoneLength)]
        public string Phone { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierPhoneLength, ErrorMessage = ErrorMessages.CashierPhoneLength)]
        public string Mobile { get; set; }


        [Required]
        [MaxLength(MaxLengthConstants.MaxCashierEmailLength, ErrorMessage = ErrorMessages.CashierEmailLength)]
        [EmailAddress(ErrorMessage = ErrorMessages.UserEmailAddressRegularExpression)]
        public string Email { get; set; }

        [MaxLength(MaxLengthConstants.MaxCashierGenderLength, ErrorMessage = ErrorMessages.CashierGenderLength)]
        public string Gender { get; set; }

        //Masterlist item
        public int TypeId { get; set; }
        public bool Status { get; set; }
        public int? StoreGroupId { get; set; }

        //storeId
        public int? OutletId { get; set; }

        //Master list item
        public int? ZoneId { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierPasswordLength, ErrorMessage = ErrorMessages.CashierPasswordLength)]
        //[Required(ErrorMessage = ErrorMessages.CashierPassword)]
        public string Password { get; set; }

        //MasterListItem
        public int? AccessLevelId { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(MaxLengthConstants.MaxCashierWristBandIndLength, ErrorMessage = ErrorMessages.CashierWristBandIndLength)]
        public string WristBandInd { get; set; }

        [MaxLength(MaxLengthConstants.MaxCashierDispnameLength, ErrorMessage = ErrorMessages.CashierDispnameLength)]
      //  [Required(ErrorMessage=ErrorMessages.CashierDispnameLength)]
        public string Dispname { get; set; }

        [MaxLength(MaxLengthConstants.MaxCashierLeftHandTillIndLength, ErrorMessage = ErrorMessages.CashierLeftHandTillIndLength)]
        public string LeftHandTillInd { get; set; }

        [MaxLength(MaxLengthConstants.MaxCashierFuelUserLength, ErrorMessage = ErrorMessages.CashierFuelUserLength)]
        public string FuelUser { get; set; }

        [MaxLength(MaxLengthConstants.MaxCashierFuelPassLength, ErrorMessage = ErrorMessages.CashierFuelPassLength)]
        public string FuelPass { get; set; }

        public byte[] Image { get; set; }
        public string ImagePath { get; set; }
    }
}
