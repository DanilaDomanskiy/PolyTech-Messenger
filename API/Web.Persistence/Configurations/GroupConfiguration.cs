using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Core.Entities;

namespace Web.Persistence.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(m => m.Id);

            builder
                .Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .HasOne(g => g.Creator)
                .WithMany()
                .HasForeignKey(g => g.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(g => g.Users)
                .WithMany(u => u.Groups);

            builder
                .HasMany(g => g.Messages)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}