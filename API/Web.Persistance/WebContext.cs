﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Web.Core.Entites;
using Web.Persistence.Configurations;

namespace Web.Persistence
{
    public class WebContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public WebContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<PrivateChat> PrivateChats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("ConnectionString");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new PrivateChatConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
        }
    }
}