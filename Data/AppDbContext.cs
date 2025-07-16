using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TreatmentService> TreatmentServices { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlans { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasMaxLength(20);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
            });

            // Doctor configuration
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithOne()
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // TreatmentService configuration
            modelBuilder.Entity<TreatmentService>(entity =>
            {
                entity.HasOne(s => s.Category)
                    .WithMany(c => c.Services)
                    .HasForeignKey(s => s.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // TreatmentPlan configuration
            modelBuilder.Entity<TreatmentPlan>(entity =>
            {
                entity.HasOne(tp => tp.Customer)
                    .WithMany()
                .HasForeignKey(tp => tp.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tp => tp.TreatmentService)
                    .WithMany(ts => ts.TreatmentPlans)
                    .HasForeignKey(tp => tp.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // BlogPost configuration
            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.HasOne(b => b.Author)
                    .WithMany()
                    .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // Feedback configuration
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasOne(f => f.Customer)
                .WithMany(c => c.GivenFeedbacks)
                .HasForeignKey(f => f.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Doctor)
                .WithMany(d => d.ReceivedFeedbacks)
                .HasForeignKey(f => f.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(a => a.Customer)
                    .WithMany()
                    .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                    .WithMany()
                    .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
} 