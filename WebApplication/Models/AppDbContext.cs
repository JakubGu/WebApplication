using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication.Models.Entities;

namespace WebApplication.Models
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Translation> Translations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Translation>()
                .HasRequired(t => t.User)
                .WithMany(u => u.Translations)
                .HasForeignKey(t => t.UserId)
                .WillCascadeOnDelete(false);
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
    }
}