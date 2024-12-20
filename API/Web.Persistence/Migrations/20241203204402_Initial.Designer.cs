﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Web.Persistence;

#nullable disable

namespace Web.Persistence.Migrations
{
    [DbContext(typeof(WebContext))]
    [Migration("20241203204402_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<Guid>("GroupsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GroupsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("PrivateChatUser", b =>
                {
                    b.Property<Guid>("PrivateChatsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PrivateChatsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("PrivateChatUser");
                });

            modelBuilder.Entity("Web.Core.Entities.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImagePath")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Web.Core.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PrivateChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("PrivateChatId");

                    b.HasIndex("SenderId");

                    b.HasIndex("Timestamp");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Web.Core.Entities.PrivateChat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("PrivateChats");
                });

            modelBuilder.Entity("Web.Core.Entities.UnreadMessages", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PrivateChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("PrivateChatId");

                    b.HasIndex("UserId");

                    b.ToTable("UnreadMessages");
                });

            modelBuilder.Entity("Web.Core.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ProfileImagePath")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Web.Core.Entities.UserConnection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ActiveGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ActivePrivateChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConnectionId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ActiveGroupId");

                    b.HasIndex("ActivePrivateChatId");

                    b.HasIndex("ConnectionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserConnections");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("Web.Core.Entities.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Web.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PrivateChatUser", b =>
                {
                    b.HasOne("Web.Core.Entities.PrivateChat", null)
                        .WithMany()
                        .HasForeignKey("PrivateChatsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Web.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Web.Core.Entities.Group", b =>
                {
                    b.HasOne("Web.Core.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("Web.Core.Entities.Message", b =>
                {
                    b.HasOne("Web.Core.Entities.Group", "Group")
                        .WithMany("Messages")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Core.Entities.PrivateChat", "PrivateChat")
                        .WithMany("Messages")
                        .HasForeignKey("PrivateChatId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Core.Entities.User", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("PrivateChat");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Web.Core.Entities.UnreadMessages", b =>
                {
                    b.HasOne("Web.Core.Entities.Group", "Group")
                        .WithMany("UnreadMessages")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Core.Entities.PrivateChat", "PrivateChat")
                        .WithMany("UnreadMessages")
                        .HasForeignKey("PrivateChatId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Core.Entities.User", "User")
                        .WithMany("UnreadMessages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("PrivateChat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Web.Core.Entities.UserConnection", b =>
                {
                    b.HasOne("Web.Core.Entities.Group", "ActiveGroup")
                        .WithMany("ActiveUserConnections")
                        .HasForeignKey("ActiveGroupId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Web.Core.Entities.PrivateChat", "ActivePrivateChat")
                        .WithMany("ActiveUserConnections")
                        .HasForeignKey("ActivePrivateChatId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Web.Core.Entities.User", "User")
                        .WithMany("Connections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ActiveGroup");

                    b.Navigation("ActivePrivateChat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Web.Core.Entities.Group", b =>
                {
                    b.Navigation("ActiveUserConnections");

                    b.Navigation("Messages");

                    b.Navigation("UnreadMessages");
                });

            modelBuilder.Entity("Web.Core.Entities.PrivateChat", b =>
                {
                    b.Navigation("ActiveUserConnections");

                    b.Navigation("Messages");

                    b.Navigation("UnreadMessages");
                });

            modelBuilder.Entity("Web.Core.Entities.User", b =>
                {
                    b.Navigation("Connections");

                    b.Navigation("SentMessages");

                    b.Navigation("UnreadMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
