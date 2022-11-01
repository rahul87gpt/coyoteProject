using Coyote.Console.Common;
using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.ViewModels.ViewModels
{
    public class TaxViewModel
    {
        public long Id { get; set; }

        [MaxLength(MaxLengthConstants.MinTaxCodeLength, ErrorMessage = ErrorMessages.TaxCode)]
        [Required]
        public string Code { get; set; }

        [MaxLength(MaxLengthConstants.MinTaxDescLength, ErrorMessage = ErrorMessages.TaxDesc)]
        [Required]
        public string Desc { get; set; }

        public float Factor { get; set; }

        public bool Status { get; set; }
        [Required]
        public System.DateTime CreatedAt { get; set; }

        [Required]
        public System.DateTime UpdatedAt { get; set; }

        [Required]
        public int CreatedById { get; set; }

        [Required]
        public int UpdatedById { get; set; }
    }

}
