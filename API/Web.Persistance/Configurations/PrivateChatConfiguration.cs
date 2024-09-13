using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Web.Core.Entites;

namespace Web.Persistence.Configurations
{
    public class PrivateChatConfiguration : IEntityTypeConfiguration<PrivateChat>
    {
        public void Configure(EntityTypeBuilder<PrivateChat> builder)
        {
            builder
                .HasKey(pc => pc.Id);

            builder
                .HasOne(pc => pc.User1)
                .WithMany(u1 => u1.PrivateChatsAsUser1)
                .HasForeignKey(pc => pc.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(pc => pc.User2)
                .WithMany(u2 => u2.PrivateChatsAsUser2)
                .HasForeignKey(pc => pc.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(pc => pc.Messages)
                .WithOne(m => m.PrivateChat)
                .HasForeignKey(m => m.PrivateChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
