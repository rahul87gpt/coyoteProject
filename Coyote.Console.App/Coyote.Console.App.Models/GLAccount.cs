using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("GLAccount")]
    public class GLAccount : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(MaxLengthConstants.MaxGLADescLength, ErrorMessage = ErrorMessages.MaxGLADescLength)]
        [Required]
        public string Desc { get; set; }

        [MaxLength(MaxLengthConstants.MaxGLASystemLength, ErrorMessage = ErrorMessages.MaxGLASystemLength)]
        [Required]
        public string AccountSystem { get; set; }

        [MaxLength(MaxLengthConstants.MaxGLANumberLength, ErrorMessage = ErrorMessages.MaxGLANumberLength)]
        [Required]
        public string AccountNumber { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public int TypeId { get; set; }
        public MasterListItems TypeMasterListItem { get; set; }

        public int? Company { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public Users UserCreatedBy { get; set; }
        public Users UserUpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}