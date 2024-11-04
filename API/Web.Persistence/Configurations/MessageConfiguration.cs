using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Core.Entities;

namespace Web.Persistence.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);

            builder
               .Property(m => m.Content)
               .IsRequired()
               .HasMaxLength(5000);

            builder
                .Property(m => m.Timestamp)
                .IsRequired();

            builder.HasIndex(m => m.Timestamp);

            builder
                .HasOne(m => m.Sender)
                .WithMany(s => s.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => m.SenderId);

            builder
                .HasOne(m => m.Group)
                .WithMany(g => g.Messages)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.GroupId);

            builder
                .HasOne(m => m.PrivateChat)
                .WithMany(pc => pc.Messages)
                .HasForeignKey(m => m.PrivateChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.PrivateChatId);
        }
    }
}