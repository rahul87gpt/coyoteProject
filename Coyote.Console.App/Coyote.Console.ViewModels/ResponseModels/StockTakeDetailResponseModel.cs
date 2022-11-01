using Coyote.Console.ViewModels.RequestModels;
using System;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class StockTakeDetailResponseModel : StockTakeDetailRequestModel
    {
        //values need to be updated   
        public long? Number { get; set; }
        public string ProductDesc { get; set; }
        public float? UnitCost { get; set; } = 0;
        public float? UnitCount { get; set; } = 0;
        public float? VarCost { get; set; } = 0;
        public float? VarUnits { get; set; } = 0;
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
