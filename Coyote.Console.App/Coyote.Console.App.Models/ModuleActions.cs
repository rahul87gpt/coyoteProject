using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("ModuleActions")]
    public class ModuleActions : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public virtual MasterListItems ModuleName { get; set; }
        [MaxLength(MaxLengthConstants.MaxModuleActionNameLength, ErrorMessage = ErrorMessages.ModuleActionsNameLength)]
        [Required(ErrorMessage = ErrorMessages.ModuleActionsNameRequired)]
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool Status { get; set; }
        [Required(ErrorMessage = ErrorMessages.ModuleActionsActionRequired)]
        public string Action { get; set; }
        public int? ActionTypeId { get; set; }
        public virtual MasterListItems ActionType { get; set; }



        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }


    }
}
