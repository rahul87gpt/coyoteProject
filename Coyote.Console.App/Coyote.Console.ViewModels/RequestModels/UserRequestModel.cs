using Coyote.Console.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class UserRequestModel
    {
        public int Id { get; set; }

        [MaxLength(MaxLengthConstants.UserNameLength, ErrorMessage = ErrorMessages.UserFirstName)]
        [Required(ErrorMessage = ErrorMessages.UserFirstNameRequired)]
        public string FirstName { get; set; }
        [MaxLength(MaxLengthConstants.UserNameLength, ErrorMessage = ErrorMessages.UserMiddleName)]

        public string MiddleName { get; set; }
        [MaxLength(MaxLengthConstants.UserNameLength, ErrorMessage = ErrorMessages.UserLastName)]
        [Required]
        public string LastName { get; set; }

        [MaxLength(MaxLengthConstants.UserPhoneLength, ErrorMessage = ErrorMessages.UserMobileNo)]
        public string MobileNo { get; set; }

        [MaxLength(MaxLengthConstants.UserPhoneLength, ErrorMessage = ErrorMessages.UserPhoneNo)]
        public string PhoneNo { get; set; }

        [MaxLength(MaxLengthConstants.MaxUserLength, ErrorMessage = ErrorMessages.UserEmail)]
        [Required(ErrorMessage = ErrorMessages.UserEmailAddressRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.UserEmailAddressRegularExpression)]
        public string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.UserNameRequired)]
        [MaxLength(MaxLengthConstants.MaxUserNameLength, ErrorMessage = ErrorMessages.UsernameNameLength)]
        public string UserName { get; set; }

        [MaxLength(MaxLengthConstants.UserAddressLength, ErrorMessage = ErrorMessages.UserAddress)]
        public string Address1 { get; set; }
        [MaxLength(MaxLengthConstants.UserAddressLength, ErrorMessage = ErrorMessages.UserAddress)]
        public string Address2 { get; set; }
        [MaxLength(MaxLengthConstants.UserAddressLength, ErrorMessage = ErrorMessages.UserAddress)]
        public string Address3 { get; set; }
        [MaxLength(MaxLengthConstants.MinUserLength, ErrorMessage = ErrorMessages.UserPostal)]
        public string PostCode { get; set; }
        [MaxLength(MaxLengthConstants.UserGenderLength, ErrorMessage = ErrorMessages.UserGender)]
        public string Gender { get; set; }
        [Required]
        public bool Status { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<long> ZoneIdList { get; set; } = new List<long>();
        public List<int> StoreIdList { get; set; } = new List<int>();
        public List<int> RoleIdList { get; set; } = new List<int>();
        public int DefaultRoleId { get; set; }
        public DateTime? LastLogin { get; set; }

        public string PromoPrefix { get; set; }
        public string KeypadPrefix { get; set; }
        public byte? Type { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }

        [JsonIgnore]
        // [Required]
        public string ZoneIds { get; set; }
        [JsonIgnore]
        // [Required]
        public string StoreIds { get; set; }

        //[RegularExpression(@" ^ (?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = ErrorMessages.NewUserPassword)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = ErrorMessages.NewUserPassword)]
        [Required(ErrorMessage = ErrorMessages.UserPasswordForLogin)]
        public string Password { get; set; }
        public bool AddUnlockProduct { get; set; } = false;
    }
}
