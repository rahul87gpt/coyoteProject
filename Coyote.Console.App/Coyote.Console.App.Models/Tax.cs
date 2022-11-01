using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Tax")]
    public class Tax : IKeyIdentifier<long>, IAuditable<int>
    {
        [Key]
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
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

        public virtual ICollection<Product> ProductsTax { get; }

    }
}
