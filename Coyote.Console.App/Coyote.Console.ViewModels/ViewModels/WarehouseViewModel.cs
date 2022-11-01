using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class WarehouseViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MinWarehouseCodeLength, ErrorMessage = ErrorMessages.WarehouseCode)]

        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MinWarehouseDescLength, ErrorMessage = ErrorMessages.WarehouseDesc)]
        [Required]
        public string Desc { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int HostFormatId { get; set; }
        public bool Status { get; set; }
        //  [Required]
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        public int CreatedById { get; set; }
        [JsonIgnore]
        public int UpdatedById { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}
