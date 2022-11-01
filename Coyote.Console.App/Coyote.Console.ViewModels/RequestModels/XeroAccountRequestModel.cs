using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class XeroAccountRequestModel
    {
        [MaxLength(MaxLengthConstants.XeroAccountingCodeLength, ErrorMessage = ErrorMessages.XeroAccountingCodeLength)]
      //  [Required]
        public string Code { get; set; }

        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        [Required]
        public string Desc { get; set; }
        [Required]
        public int StoreId { get; set; }

        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string FinAccSummary { get; set; }

        public long? GSTProdSale { get; set; }

        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string GSTProdSaleDesc { get; set; }

        public long? NonGSTProdSale { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string NonGSTProdSaleDesc { get; set; }


        public long? LessUberEats { get; set; }

        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string LessUberEatsDesc { get; set; }

        public long? Anex { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string AnexDesc { get; set; }

        public long? CashEFTPOS { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string CashEFTPOSDesc { get; set; }

        public long? UnderOver { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string UnderOverDesc { get; set; }


        public long? FuelCard { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string FuelCardDesc { get; set; }

        public long? FleetCard { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string FleetCardDesc { get; set; }

        public long? MotorPass { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string MotorPassDesc { get; set; }

        public long? MotorCharge { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string MotorChargeDesc { get; set; }

        public long? Other { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string OtherDesc { get; set; }

        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string StockAccSummary { get; set; }

        public long? BalanceSheet { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string BalanceSheetDesc { get; set; }

        public long? ProfitLoss { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingDescLength, ErrorMessage = ErrorMessages.XeroAccountingDescLength)]
        public string ProfitLossDesc { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingKeyLength, ErrorMessage = ErrorMessages.XeroAccountingKeyLength)]
        public string XeroSecretKey { get; set; }
        [MaxLength(MaxLengthConstants.XeroAccountingKeyLength, ErrorMessage = ErrorMessages.XeroAccountingKeyLength)]
        public string XeroConsumerKey { get; set; }
    }
}
