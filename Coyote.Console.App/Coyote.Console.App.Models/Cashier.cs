using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Coyote.Console.App.Models
{
    [Table("Cashier")]
    public class Cashier : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
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
        public string ImagePath { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierPostcodeLength, ErrorMessage = ErrorMessages.CashierPostcodeLength)]
        public string Postcode { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierPhoneLength, ErrorMessage = ErrorMessages.CashierPhoneLength)]
        public string Phone { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierPhoneLength, ErrorMessage = ErrorMessages.CashierPhoneLength)]
        public string Mobile { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.MaxCashierEmailLength, ErrorMessage = ErrorMessages.CashierEmailLength)]
        public string Email { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierGenderLength, ErrorMessage = ErrorMessages.CashierGenderLength)]
        public string Gender { get; set; }
        public int TypeId { get; set; }
        public virtual MasterListItems Type { get; set; }
        public bool Status { get; set; }
        public int? StoreGroupId { get; set; }
        public virtual StoreGroup StoreGroup { get; set; }
        public int? OutletId { get; set; }
        public virtual Store Outlet { get; set; }
        public int? ZoneId { get; set; }
        public virtual MasterListItems Zone { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierPasswordLength, ErrorMessage = ErrorMessages.CashierPasswordLength)]
        public string Password { get; set; }
        public int? AccessLevelId { get; set; }
        public virtual MasterListItems AccessLevel { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierWristBandIndLength, ErrorMessage = ErrorMessages.CashierWristBandIndLength)]
        public string WristBandInd { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierDispnameLength, ErrorMessage = ErrorMessages.CashierDispnameLength)]
        public string Dispname { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierLeftHandTillIndLength, ErrorMessage = ErrorMessages.CashierLeftHandTillIndLength)]
        public string LeftHandTillInd { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierFuelUserLength, ErrorMessage = ErrorMessages.CashierFuelUserLength)]
        public string FuelUser { get; set; }
        [MaxLength(MaxLengthConstants.MaxCashierFuelPassLength, ErrorMessage = ErrorMessages.CashierFuelPassLength)]
        public string FuelPass { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Department Departments { get; set; }
        public virtual Users UserUpdatedBy { get; set; }


        public virtual ICollection<JournalHeader> JournalHeaderCashier { get; }
        public virtual ICollection<JournalDetail> JournalDetailsCahier { get; }

    }
}
