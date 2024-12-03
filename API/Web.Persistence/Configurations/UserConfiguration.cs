using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Core.Entities;

namespace Web.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(u => u.Email)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.IsActive)
                .IsRequired();

            builder.Property(u => u.LastActive)
                .IsRequired();

            builder.Property(u => u.ProfileImagePath)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}