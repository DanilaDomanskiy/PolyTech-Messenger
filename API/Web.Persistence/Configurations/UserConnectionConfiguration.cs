using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Core.Entities;

namespace Web.Persistence.Configurations
{
    public class UserConnectionConfiguration : IEntityTypeConfiguration<UserConnection>
    {
        public void Configure(EntityTypeBuilder<UserConnection> builder)
        {
            builder.HasKey(uc => uc.Id);

            builder.Property(uc => uc.ConnectionId)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(uc => uc.User)
               .WithMany(u => u.Connections)
               .HasForeignKey(uc => uc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uc => uc.ActivePrivateChat)
               .WithMany(pc => pc.ActiveUserConnections)
               .HasForeignKey(uc => uc.ActivePrivateChatId)
               .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(uc => uc.ActiveGroup)
               .WithMany(g => g.ActiveUserConnections)
               .HasForeignKey(uc => uc.ActiveGroupId)
               .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(uc => uc.ConnectionId);
            builder.HasIndex(uc => uc.UserId);
        }
    }
}