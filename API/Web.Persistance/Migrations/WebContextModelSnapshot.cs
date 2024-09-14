﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Web.Persistence;

#nullable disable

namespace Web.Persistence.Migrations
{
    [DbContext(typeof(WebContext))]
    partial class WebContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<int>("GroupsId")
                        .HasColumnType("int");

                    b.Property<int>("UsersId")
                        .HasColumnType("int");

                    b.HasKey("GroupsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("Web.Core.Entites.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Web.Core.Entites.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<int?>("PrivateChatId")
                        .HasColumnType("int");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("PrivateChatId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Web.Core.Entites.PrivateChat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("User1Id")
                        .HasColumnType("int");

                    b.Property<int>("User2Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User1Id");

                    b.HasIndex("User2Id");

                    b.ToTable("PrivateChats");
                });

            modelBuilder.Entity("Web.Core.Entites.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("Web.Core.Entites.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Web.Core.Entites.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Web.Core.Entites.Group", b =>
                {
                    b.HasOne("Web.Core.Entites.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("Web.Core.Entites.Message", b =>
                {
                    b.HasOne("Web.Core.Entites.Group", "Group")
                        .WithMany("Messages")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Core.Entites.PrivateChat", "PrivateChat")
                        .WithMany("Messages")
                        .HasForeignKey("PrivateChatId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Core.Entites.User", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("PrivateChat");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Web.Core.Entites.PrivateChat", b =>
                {
                    b.HasOne("Web.Core.Entites.User", "User1")
                        .WithMany("PrivateChatsAsUser1")
                        .HasForeignKey("User1Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Web.Core.Entites.User", "User2")
                        .WithMany("PrivateChatsAsUser2")
                        .HasForeignKey("User2Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("Web.Core.Entites.Group", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Web.Core.Entites.PrivateChat", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Web.Core.Entites.User", b =>
                {
                    b.Navigation("PrivateChatsAsUser1");

                    b.Navigation("PrivateChatsAsUser2");

                    b.Navigation("SentMessages");
                });
#pragma warning restore 612, 618
        }
    }
}