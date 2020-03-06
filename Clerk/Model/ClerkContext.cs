using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;

namespace Clerk.Model
{
    public class ClerkContext : IdentityDbContext<ClerkUser>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Experiment> Experiments { get; set; }

        public ClerkContext(DbContextOptions<ClerkContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Project>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<Project>(entity =>
                {
                    entity.Property(r => r.CreateTime)
                        .HasDefaultValueSql("now()");
                });

            modelBuilder.Entity<Experiment>(entity =>
            {
                entity.Property(r => r.StartTime)
                    .HasDefaultValueSql("now()");
            });
        }

        [DbFunction(Name = "now")]
        public static DateTime Now()
        {
            throw new NotImplementedException();
        }
    }
}