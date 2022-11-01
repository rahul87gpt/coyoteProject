using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class StoreGroupViewModel
    {
        public int Id { get; set; }


        [MaxLength(MaxLengthConstants.MinStoreGroupLength, ErrorMessage = ErrorMessages.StoreGroupCode)]
        [Required]
        public string Code { get; set; }

        [MaxLength(MaxLengthConstants.MaxStoreGroupLength, ErrorMessage = ErrorMessages.StoreGroupName)]
        [Required]
        public string Name { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }

        [Required]
        public int CreatedById { get; set; }

        [Required]
        public int UpdatedById { get; set; }
    }
}
