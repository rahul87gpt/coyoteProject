using Coyote.Console.App.Models.EntityContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("CompetitionTrigger")]
    public class CompetitionTrigger : IAuditable<int>, IKeyIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        //promo comp
        public int CompPromoId { get; set; }
        public virtual PromotionCompetition PromoComp { get; set; }

        public int TriggerProductGroupID { get; set; }
        public virtual MasterListItems TriggerProductGroup { get; set; }
        public bool Share { get; set; }
        public float LoyaltyFactor { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }
    }
}
