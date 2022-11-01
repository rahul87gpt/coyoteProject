using Coyote.Console.ViewModels.RequestModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class ZoneOutletViewModel
    {
        public int Id { get; set; }

        [Required]
        public int StoreId { get; set; }


        [Required]
        public int ZoneId { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int CreatedById { get; set; }

        [Required]
        public int UpdatedById { get; set; }
        [JsonIgnore]
        public bool? IsDeleted { get; set; }
    }

    public class ZoneOutletInputModel : PagedInputModel
    {
        public int? StoreId { get; set; }
        public int? ZoneId { get; set; }
    }
}
