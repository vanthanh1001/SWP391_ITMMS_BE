using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlans { get; set; }
        public DbSet<TreatmentService> TreatmentServices { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        // Thêm các DbSet mới
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<DoctorLeave> DoctorLeaves { get; set; }
        public DbSet<DoctorTimeSlot> DoctorTimeSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure Doctor
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Doctor>(d => d.UserId);

            // Configure Customer
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId);

            // Configure Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.TreatmentPlan)
                .WithMany(tp => tp.Appointments)
                .HasForeignKey(a => a.TreatmentPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure MedicalRecord
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Customer)
                .WithMany(c => c.MedicalRecords)
                .HasForeignKey(mr => mr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(mr => mr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(mr => mr.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.TreatmentPlan)
                .WithMany(tp => tp.MedicalRecords)
                .HasForeignKey(mr => mr.TreatmentPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure TreatmentPlan
            modelBuilder.Entity<TreatmentPlan>()
                .HasOne(tp => tp.Customer)
                .WithMany(c => c.TreatmentPlans)
                .HasForeignKey(tp => tp.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TreatmentPlan>()
                .HasOne(tp => tp.Doctor)
                .WithMany(d => d.TreatmentPlans)
                .HasForeignKey(tp => tp.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TreatmentPlan>()
                .HasOne(tp => tp.TreatmentService)
                .WithMany()
                .HasForeignKey(tp => tp.TreatmentServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure BlogPost
            modelBuilder.Entity<BlogPost>()
                .HasOne(bp => bp.Author)
                .WithMany()
                .HasForeignKey(bp => bp.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Doctor)
                .WithMany(d => d.ReceivedFeedbacks)
                .HasForeignKey(f => f.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Appointment)
                .WithMany()
                .HasForeignKey(f => f.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure DoctorSchedule
            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.Schedules)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure DoctorLeave
            modelBuilder.Entity<DoctorLeave>()
                .HasOne(dl => dl.Doctor)
                .WithMany(d => d.Leaves)
                .HasForeignKey(dl => dl.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure DoctorTimeSlot
            modelBuilder.Entity<DoctorTimeSlot>()
                .HasOne(dts => dts.Doctor)
                .WithMany(d => d.TimeSlots)
                .HasForeignKey(dts => dts.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index cho performance
            modelBuilder.Entity<DoctorTimeSlot>()
                .HasIndex(dts => new { dts.DoctorId, dts.Date, dts.StartTime });

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.TimeSlot });

            modelBuilder.Entity<MedicalRecord>()
                .HasIndex(mr => new { mr.DoctorId, mr.RecordDate });

            // Configure VitalSigns as complex type
            modelBuilder.Entity<MedicalRecord>()
                .ComplexProperty(mr => mr.VitalSigns, vs =>
                {
                    vs.Property(v => v.BloodPressure);
                    vs.Property(v => v.HeartRate);
                    vs.Property(v => v.Temperature);
                    vs.Property(v => v.Weight);
                });
        }
    }
} 