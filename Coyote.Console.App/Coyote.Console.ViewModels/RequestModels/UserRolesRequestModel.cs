using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class UserRolesRequestModel
    {
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
        public DateTime? LastLogin { get; set; }

        [JsonIgnore]
        public DateTime? CreatedAt { get; set; }

        [JsonIgnore]
        public DateTime? UpdatedAt { get; set; }
        public string PromoPrefix { get; set; }
        public string KeypadPrefix { get; set; }
        public byte? Type { get; set; }

        [JsonIgnore]
        public int? CreatedById { get; set; }
        [JsonIgnore]
        public int? UpdatedById { get; set; }
        [JsonIgnore]
        public bool IsResetPassword { get; set; }
        [JsonIgnore]
        public string TemporaryPassword { get; set; }
        [JsonIgnore]
        public object Password { get; set; }

        public int DefaultRoleId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public List<int> Roles { get; set; }
    }
}
