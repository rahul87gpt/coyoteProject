using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class MinStockHandRequestModel
    {
        [Required]
        public int OutletId { get; set; }
        [Required]
        public int DaysHist { get; set; }
        public bool ExcludePromo { get; set; }
        //[Required]
        public int? DepartmentId { get; set; }
        public bool LeaveExisting { get; set; }
    }
}
