using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RelativeFormulas.Domain.Entities;

namespace RelativeFormulas.Infrastructure.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(r => r.Slug)
            .IsUnique();

        builder.Property(r => r.InstructionsText)
            .IsRequired();

        builder.Property(r => r.TranscriptionText)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();
    }
}
