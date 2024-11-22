using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Core.Entities;

namespace Web.Persistence.Configurations
{
    public class UnreadMessagesConfiguration : IEntityTypeConfiguration<UnreadMessages>
    {
        public void Configure(EntityTypeBuilder<UnreadMessages> builder)
        {
            builder.HasKey(um => um.Id);

            builder.Property(um => um.Count)
                .IsRequired();

            builder.HasOne(um => um.User)
                .WithMany(u => u.UnreadMessages)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(um => um.PrivateChat)
                .WithMany(pc => pc.UnreadMessages)
                .HasForeignKey(um => um.PrivateChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(um => um.Group)
                .WithMany(g => g.UnreadMessages)
                .HasForeignKey(um => um.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(um => um.UserId);
            builder.HasIndex(um => um.PrivateChatId);
            builder.HasIndex(um => um.GroupId);
        }
    }
}