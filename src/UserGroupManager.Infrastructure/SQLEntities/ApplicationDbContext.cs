using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserGroupManager.Domain.Entities;

namespace UserGroupManager.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permission> Permissions { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.AccountCreateStamp)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Manage Users" },
                new Permission { Id = 2, Name = "View Users" },
                new Permission { Id = 3, Name = "Manage Groups" },
                new Permission { Id = 4, Name = "View Groups" },
                new Permission { Id = 5, Name = "View Reports" }
            );

            modelBuilder.Entity<Group>().HasData(
            new Group { Id = 1, Name = "Administrators" },
            new Group { Id = 2, Name = "Standard Users" }
            );



        }


    }
}
