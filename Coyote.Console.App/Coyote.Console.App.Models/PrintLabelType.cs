using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("PrintLabelType")]
    public class PrintLabelType : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxPrintLabelTypeCodeLength, ErrorMessage = ErrorMessages.PrintLabelTypeCodeLength)]
        public String Code { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxPrintLabelDescCodeLength, ErrorMessage = ErrorMessages.PrintLabelTypeDescLength)]
        public String Desc { get; set; }
        
        public int LablesPerPage { get; set; }

        [MaxLength(MaxLengthConstants.MaxPrintLabelTypeCodeLength)]
        public string PrintBarCodeType { get; set; }
        public bool Status { get; set; } = true;
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
        public virtual ICollection<Store> StoreDefaultPromo { get; }
        public virtual ICollection<Store> StoreDefaultShelf { get; }
        public virtual ICollection<Store> StoreDefaultShort { get; }

    }
}



