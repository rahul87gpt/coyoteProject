using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class SupplierRequestModel
    {
        [MaxLength(MaxLengthConstants.SuppCodeLength, ErrorMessage = ErrorMessages.SuppCode)]
        [Required]
        public string Code { get; set; }

        [MaxLength(MaxLengthConstants.SuppDescLength, ErrorMessage = ErrorMessages.SuppDesc)]
        [Required]
        public string Desc { get; set; }

        [MaxLength(MaxLengthConstants.SuppAddress1Length, ErrorMessage = ErrorMessages.SuppAddress1)]
        public string Address1 { get; set; }

        [MaxLength(MaxLengthConstants.SuppAddress2Length, ErrorMessage = ErrorMessages.SuppAddress2)]
        public string Address2 { get; set; }

        [MaxLength(MaxLengthConstants.SuppAddress3Length, ErrorMessage = ErrorMessages.SuppAddress3)]
        public string Address3 { get; set; }

        [MaxLength(MaxLengthConstants.SuppAddress3Length, ErrorMessage = ErrorMessages.SuppAddress3)]
        public string Address4 { get; set; }

        [MaxLength(MaxLengthConstants.SuppPhoneLength, ErrorMessage = ErrorMessages.SuppPhone)]
        public string Phone { get; set; }

        [MaxLength(MaxLengthConstants.SuppFaxLength, ErrorMessage = ErrorMessages.SuppFax)]
        public string Fax { get; set; }

        [MaxLength(MaxLengthConstants.SuppEmailLength, ErrorMessage = ErrorMessages.SuppEmail)]
        public string Email { get; set; }

        [MaxLength(MaxLengthConstants.SuppABNLength, ErrorMessage = ErrorMessages.SuppABN)]
        public string ABN { get; set; }

        [MaxLength(MaxLengthConstants.SuppUpdateCostLength, ErrorMessage = ErrorMessages.SuppUpdateCost)]
        public string UpdateCost { get; set; }

        [MaxLength(MaxLengthConstants.SuppPromoCodeLength, ErrorMessage = ErrorMessages.SuppPromoCode)]
        public string PromoSupplier { get; set; }

        [MaxLength(MaxLengthConstants.SuppContactNameLength, ErrorMessage = ErrorMessages.SuppContactName)]
        public string Contact { get; set; }

        [MaxLength(MaxLengthConstants.SuppCostZoneLength, ErrorMessage = ErrorMessages.SuppCostZone)]
        public string CostZone { get; set; }

        [MaxLength(MaxLengthConstants.SuppGSTFreeItemCodeLength, ErrorMessage = ErrorMessages.SuppGSTFreeItemCode)]
        public string GSTFreeItemCode { get; set; }

        [MaxLength(MaxLengthConstants.SuppGSTFreeItemDescLength, ErrorMessage = ErrorMessages.SuppGSTFreeItemDesc)]
        public string GSTFreeItemDesc { get; set; }

        [MaxLength(MaxLengthConstants.SuppGSTInclItemCodeLength, ErrorMessage = ErrorMessages.SuppGSTInclItemCode)]
        public string GSTInclItemCode { get; set; }

        [MaxLength(MaxLengthConstants.SuppGSTInclItemDescLength, ErrorMessage = ErrorMessages.SuppGSTInclItemDesc)]
        public string GSTInclItemDesc { get; set; }

        [MaxLength(MaxLengthConstants.SuppXeroNameLength, ErrorMessage = ErrorMessages.SuppXeroName)]
        public string XeroName { get; set; }
    }
}
