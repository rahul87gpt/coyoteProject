using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class ModuleActionsViewModel
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }

        [MaxLength(MaxLengthConstants.MaxModuleActionNameLength, ErrorMessage = ErrorMessages.ModuleActionsNameLength)]
        [Required]
        public string Name { get; set; }
        public string Desc { get; set; }
        [Required]
        public string Action { get; set; }
        public int? ActionTypeId { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

    }
}
