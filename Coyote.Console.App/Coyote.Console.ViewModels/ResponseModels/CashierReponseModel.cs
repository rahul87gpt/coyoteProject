using Coyote.Console.ViewModels.RequestModels;
using System;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class CashierResponseModel : CashierRequestModel
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string AccessLevel { get; set; }
        public string OutletName { get; set; }
        public string OutletDesc { get; set; }
        public bool IsStoreGroupDeleted { get; set; }
        public string StoreGroup { get; set; }
        public string StoreGroupDesc { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneDesc { get; set; }
        public string ImageUploadStatusCode { get; set; }
        public byte[] ImageBytes { get; set; }

        public string CashierName { get; set; }

    }
}
