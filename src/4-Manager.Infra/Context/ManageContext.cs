using System;
using Manager.Domain.Entities;
using Manager.Infra.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infra.Context{
    public class ManagerContext : DbContext{
        public ManagerContext(){

        }

        public ManagerContext(DbContextOptions<ManagerContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuider)
        {
            optionsBuider.UseMySql("Server=localhost;Database=api;Uid=root;Pwd=", new MySqlServerVersion(new Version(8, 0, 11)));
        }

        public virtual DbSet<User> Users{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserMap());
        }
    }
}