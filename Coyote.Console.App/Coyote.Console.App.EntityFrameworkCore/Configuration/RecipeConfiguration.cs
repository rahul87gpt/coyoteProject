using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.RecipeCreated)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_Recipe_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.RecipeUpdated)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_Recipe_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.RecipeOutlet)
               .HasForeignKey(c => c.OutletID).HasConstraintName("FK_Recipe_Outlet").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Product).WithMany(c => c.RecipeProduct)
            .HasForeignKey(c => c.ProductID).HasConstraintName("FK_Recipe_Product").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.IngredientProduct).WithMany(c => c.RecipeIngredientProduct)
           .HasForeignKey(c => c.IngredientProductID).HasConstraintName("FK_RecipeIngredient_Product").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
