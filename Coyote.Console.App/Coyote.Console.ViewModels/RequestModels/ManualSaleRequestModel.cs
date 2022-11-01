using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class ManualSaleRequestModel
    {
        [Required(ErrorMessage = ErrorMessages.ManualSaleCodeIsRequired)]
        [MaxLength(MaxLengthConstants.MinCommodityLength, ErrorMessage = ErrorMessages.ManualSaleCode)]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MaxCommodityLength, ErrorMessage = ErrorMessages.ManualSaleDesc)]
        [Required(ErrorMessage = ErrorMessages.ManualSaleDescIsRequired)]
        public string Desc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public List<ManualSaleItemRequestModel> ManualSaleItemRequestModel { get; set; }
    }

    public class ManualSaleItemRequestModel
    {
        [Required(ErrorMessage = ErrorMessages.ProductIdReq)]
        public long ProductId { get; set; }
        [Required(ErrorMessage = ErrorMessages.OutletId)]
        public int OutletId { get; set; }
        [Required(ErrorMessage = ErrorMessages.ManualSaleInvalidQuantity)]
        public float Qty { get; set; }
        [Required(ErrorMessage = ErrorMessages.ManualSaleInvalidPrice)]
        public float Price { get; set; }
        [Required(ErrorMessage = ErrorMessages.ManualSaleInvalidAmount)]
        public float Amount { get; set; }
        [Required(ErrorMessage = ErrorMessages.ManualSaleInvalidCost)]
        public float Cost { get; set; }
        [Required(ErrorMessage = ErrorMessages.ManualSalePriceLevelrequired)]
        public string PriceLevel { get; set; }
    }
}
