using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RelativeFormulas.Domain.Entities;

namespace RelativeFormulas.Infrastructure.Data.Configurations;

public class RecipeImageConfiguration : IEntityTypeConfiguration<RecipeImage>
{
    public void Configure(EntityTypeBuilder<RecipeImage> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.Caption)
            .HasMaxLength(200);

        builder.Property(i => i.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(i => i.Recipe)
            .WithMany(r => r.Images)
            .HasForeignKey(i => i.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
