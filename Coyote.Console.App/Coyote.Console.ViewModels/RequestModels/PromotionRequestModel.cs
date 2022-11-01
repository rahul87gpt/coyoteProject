using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class PromotionRequestModel
    {
        [MaxLength(MaxLengthConstants.MaxPromotionCodeLength, ErrorMessage = ErrorMessages.PromotionCodeLength)]
        [Required]
        public string Code { get; set; }
        public int PromotionTypeId { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionDescLength, ErrorMessage = ErrorMessages.PromotionDescLength)]
        [Required]
        public string Desc { get; set; }
        public bool Status { get; set; }
     //   public int SourceId { get; set; }
        public string Source { get; set; }
        public int ZoneId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        [MaxLength(MaxLengthConstants.MaxPromotionRptGroupLength, ErrorMessage = ErrorMessages.PromotionRptGroupLength)]
        public string RptGroup { get; set; }
        //public int? FrequencyId { get; set; }

        [MaxLength(MaxLengthConstants.MaxPromotionAvailibilityLength, ErrorMessage = ErrorMessages.PromotionAvailibilityLength)]
        public string Availibility { get; set; }
        public string ImagePath { get; set; }

        public List<PromotionProductRequestModel> PromotionProduct { get; set; }       
    }

    public class PromotionProductRequestModel
    {
        public int Id { get; set; }
        public string PromoCode { get; set; }
        public long ProductId { get; set; }
        public int PromotionId { get; set; }
        public string Desc { get; set; }
        public bool Status { get; set; }
        public string Action { get; set; }
        public string HostPromoType { get; set; }
        public float? AmtOffNorm1 { get; set; }
        public float? PromoUnits { get; set; }

        public byte? OfferGroup { get; set; }
        public float? Price { get; set; }
        public float? Price1 { get; set; }
        public float? Price2 { get; set; }
        public float? Price3 { get; set; }
        public float? Price4 { get; set; }
        public int? DeptId { get; set; }
        public int? SupplierId { get; set; } = 0;
        public DateTime? CostStart { get; set; }
        public DateTime? CostEnd { get; set; }
        public double CartonCost { get; set; }
        public int CartonQty { get; set; }
        public bool? CostIsPromInd { get; set; }
        public string PromotionType { get; set; }
        public long Number { get; set; } = 0;    
        public string DepartmentCode { get; set; }
        public string DepartmentDesc { get; set; }
        public string SupplierCode{ get; set; }
    }
}
