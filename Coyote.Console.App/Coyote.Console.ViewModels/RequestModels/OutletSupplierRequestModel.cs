using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class OutletSupplierRequestModel
    {
        public int StoreId { get; set; }

        public int SupplierId { get; set; }


        public bool Status { get; set; } = true;

        [Required]
        [MaxLength(MaxLengthConstants.MaxOutletSupplierSettingDescLength, ErrorMessage = ErrorMessages.OutletSupplierSettingDescLength)]
        public string Desc { get; set; }

        [MaxLength(MaxLengthConstants.MaxOutletSupplierSettingDescLength)]
        public string CustomerNumber { get; set; }

        public int StateId { get; set; }
        public int DivisionId { get; set; }
        public int? OrderTypeId { get; set; }

        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string QtyDefault { get; set; }
        public bool BuyCartoon { get; set; } = true;
        public string PostedOrder { get; set; }
    }
}
