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

            builder.HasIndex(uc => uc.ConnectionId);
            builder.HasIndex(uc => uc.UserId);

            builder.HasOne(uc => uc.User)
               .WithMany(u => u.Connections)
               .HasForeignKey(uc => uc.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}