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
        public DbSet<UserGroup> UserGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.AccountCreateStamp)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<UserGroup>()
            .HasKey(ug => new { ug.UserId, ug.GroupId });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany() 
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany()
                .HasForeignKey(ug => ug.GroupId);

            modelBuilder.Entity<User>()
                .Ignore(u => u.Groups);

            modelBuilder.Entity<Group>()
                .Ignore(g => g.Users);



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

            modelBuilder.Entity("GroupPermission").HasData(
           // Administrators Group (Id = 1) gets all permissions
               new { GroupsId = 1, PermissionsId = 1 }, // Manage Users
               new { GroupsId = 1, PermissionsId = 2 }, // View Users
               new { GroupsId = 1, PermissionsId = 3 }, // Manage Groups
               new { GroupsId = 1, PermissionsId = 4 }, // View Groups
               new { GroupsId = 1, PermissionsId = 5 }, // View Reports

               // Standard Users Group (Id = 2) gets view-only permissions
               new { GroupsId = 2, PermissionsId = 2 }, // View Users
               new { GroupsId = 2, PermissionsId = 4 }, // View Groups
               new { GroupsId = 2, PermissionsId = 5 }  // View Reports
             );

        }


    }
}
