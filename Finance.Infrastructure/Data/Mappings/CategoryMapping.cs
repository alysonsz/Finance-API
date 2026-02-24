using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Mappings;

public class CategoryMapping : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired(true)
            .HasMaxLength(80);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(x => x.UserId)
            .IsRequired(true)
            .HasMaxLength(160);

        builder.HasOne(c => c.User)
           .WithMany(u => u.Categories)
           .HasForeignKey(c => c.UserId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}