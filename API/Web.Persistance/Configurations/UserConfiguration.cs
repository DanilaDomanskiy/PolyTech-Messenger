using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Core.Entites;

namespace Web.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder
                .Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(100);

            builder
               .HasMany(u => u.SentMessages)
               .WithOne(m => m.Sender)
               .HasForeignKey(o => o.SenderId)
               .OnDelete(DeleteBehavior.Restrict);

            builder
               .HasMany(u => u.Groups)
               .WithMany(g => g.Users);

            builder
               .HasMany(u => u.PrivateChatsAsUser1)
               .WithOne(pc => pc.User1)
               .HasForeignKey(pc => pc.User1Id)
               .OnDelete(DeleteBehavior.Restrict);

            builder
               .HasMany(u => u.PrivateChatsAsUser2)
               .WithOne(pc => pc.User2)
               .HasForeignKey(pc => pc.User2Id)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
