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
        
        // Add missing DbSets
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<UserFeedback> UserFeedbacks { get; set; }
        public DbSet<TreatmentHistory> TreatmentHistories { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

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

            // Doctor configuration - Fix explicit foreign key
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithOne(u => u.Doctor)
                    .HasForeignKey<Doctor>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Customer configuration - Fix explicit foreign key
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithOne(u => u.Customer)
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

            // TreatmentPlan configuration - Fix cascade paths
            modelBuilder.Entity<TreatmentPlan>(entity =>
            {
                entity.HasOne(tp => tp.Customer)
                    .WithMany(c => c.TreatmentPlans)  // Specify navigation property
                    .HasForeignKey(tp => tp.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(tp => tp.TreatmentService)
                    .WithMany(ts => ts.TreatmentPlans)
                    .HasForeignKey(tp => tp.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BlogPost configuration
            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.HasOne(b => b.Author)
                    .WithMany(u => u.BlogPosts)  // Specify navigation property
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Feedback configuration - Fix cascade paths
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasOne(f => f.Customer)
                    .WithMany(c => c.GivenFeedbacks)
                    .HasForeignKey(f => f.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.Doctor)
                    .WithMany(d => d.ReceivedFeedbacks)
                    .HasForeignKey(f => f.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Appointment configuration - FIX SHADOW PROPERTIES
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(a => a.Customer)
                    .WithMany(c => c.Appointments)  // Specify navigation property
                    .HasForeignKey(a => a.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Appointments)  // Specify navigation property
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MedicalRecord configuration - FIX MISSING CONFIGURATION
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasOne(mr => mr.Customer)
                    .WithMany()  // No navigation property back to Customer
                    .HasForeignKey(mr => mr.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(mr => mr.Doctor)
                    .WithMany()  // No navigation property back to Doctor
                    .HasForeignKey(mr => mr.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(mr => mr.Appointment)
                    .WithMany()  // No navigation property back to Appointment
                    .HasForeignKey(mr => mr.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // UserFeedback configuration - FIX MISSING CONFIGURATION
            modelBuilder.Entity<UserFeedback>(entity =>
            {
                entity.HasOne(uf => uf.Customer)
                    .WithMany()  // No navigation property back to Customer
                    .HasForeignKey(uf => uf.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(uf => uf.Doctor)
                    .WithMany()  // No navigation property back to Doctor
                    .HasForeignKey(uf => uf.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TreatmentHistory configuration - FIX MISSING CONFIGURATION
            modelBuilder.Entity<TreatmentHistory>(entity =>
            {
                entity.HasOne<User>()
                    .WithMany()  // No navigation property
                    .HasForeignKey(th => th.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Prescription configuration - FIX MISSING CONFIGURATION
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasOne(p => p.MedicalRecord)
                    .WithMany(mr => mr.Prescriptions)
                    .HasForeignKey(p => p.MedicalRecordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 