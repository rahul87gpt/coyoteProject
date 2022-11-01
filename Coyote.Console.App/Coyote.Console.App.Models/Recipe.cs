using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;

namespace Coyote.Console.App.Models
{
    [Table("Recipe")]
    public class Recipe : IAuditableField<int>, IKeyIdentifierField<int>
    {
        [Key]
        public int ID { get; set; }
        public long ProductID { get; set; }
        public virtual Product Product { get; set; }
        public int? OutletID { get; set; }
        public virtual Store Store { get; set; }
        public long? IngredientProductID { get; set; }
        public virtual Product IngredientProduct { get; set; }
        public DateTime RecipeTimeStamp { get; set; }
        public string Description { get; set; }
        public float Qty { get; set; }
        public ParentStatus IsParents { get; set; }
        public Status IsActive { get; set; } 
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public virtual Users UserModifiedBy { get; set; }
    }
}
