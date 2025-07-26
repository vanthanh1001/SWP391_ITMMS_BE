using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        // Main entities
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<TreatmentService> TreatmentServices { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlans { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        
        // Legacy entities for backward compatibility
        public DbSet<TreatmentHistory> TreatmentHistories { get; set; }
        public DbSet<UserFeedback> UserFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Doctor configurations
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            // Customer configurations
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // TreatmentService configurations
            modelBuilder.Entity<TreatmentService>()
                .HasIndex(ts => ts.ServiceCode)
                .IsUnique();

            // TreatmentPlan configurations
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
                .WithMany(ts => ts.TreatmentPlans)
                .HasForeignKey(tp => tp.TreatmentServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            // Appointment configurations
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

            // MedicalRecord configurations
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
                .OnDelete(DeleteBehavior.Cascade);

            // Prescription configurations
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.MedicalRecord)
                .WithMany(mr => mr.Prescriptions)
                .HasForeignKey(p => p.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // TestResult configurations
            modelBuilder.Entity<TestResult>()
                .HasOne(tr => tr.Customer)
                .WithMany(c => c.TestResults)
                .HasForeignKey(tr => tr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestResult>()
                .HasOne(tr => tr.Doctor)
                .WithMany(d => d.TestResults)
                .HasForeignKey(tr => tr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback configurations
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.GivenFeedbacks)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Doctor)
                .WithMany(d => d.ReceivedFeedbacks)
                .HasForeignKey(f => f.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Appointment)
                .WithMany(a => a.Feedbacks)
                .HasForeignKey(f => f.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // BlogPost configurations
            modelBuilder.Entity<BlogPost>()
                .HasOne(bp => bp.Author)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(bp => bp.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);



            // Seed default data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@itmms.com",
                    FullName = "System Administrator",
                    Phone = "0123456789",
                    Address = "System",
                    Role = "Admin",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            );

            // Seed Sample Doctor User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 2,
                    Username = "doctor1",
                    Password = BCrypt.Net.BCrypt.HashPassword("doctor123"),
                    Email = "doctor1@itmms.com",
                    FullName = "Dr. Nguyễn Văn A",
                    Phone = "0987654321",
                    Address = "Hà Nội",
                    Role = "Doctor",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            );

            // Seed Manager User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 4,
                    Username = "manager1",
                    Password = BCrypt.Net.BCrypt.HashPassword("manager123"),
                    Email = "manager@itmms.com",
                    FullName = "Nguyễn Thị B",
                    Phone = "0123456790",
                    Address = "Hà Nội",
                    Role = "Manager",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            );

            // Seed Sample Doctor Profile
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = 1,
                    UserId = 2,
                    Specialization = "Sản phụ khoa - Hiếm muộn",
                    LicenseNumber = "BS001234",
                    Education = "Bác sĩ chuyên khoa II - Đại học Y Hà Nội",
                    ExperienceYears = 10,
                    Description = "Chuyên gia điều trị hiếm muộn với 10 năm kinh nghiệm",
                    ConsultationFee = 500000m,
                    IsAvailable = true
                }
            );

            // Seed Treatment Services
            // modelBuilder.Entity<TreatmentService>().HasData(
            //     new TreatmentService
            //     {
            //         Id = 1,
            //         ServiceName = "Thụ tinh trong ống nghiệm (IVF)",
            //         ServiceCode = "IVF001",
            //         Description = "Kỹ thuật hỗ trợ sinh sản hiện đại, tỷ lệ thành công cao. Quy trình bao gồm kích thích buồng trung, lấy trứng, thụ tinh ngoài cơ thể và chuyển phôi.",
            //         BasePrice = 85000000,
            //         Procedures = "Khám sàng lọc → Kích thích buồng trung → Lấy trứng → Thụ tinh → Nuôi cấy phôi → Chuyển phôi",
            //         Requirements = "Khám tổng quát, xét nghiệm hormone, siêu âm, tinh dịch đồ",
            //         DurationDays = 30,
            //         SuccessRate = 68.5f,
            //         IsActive = true,
            //         CreatedAt = DateTime.Now,
            //         ImageUrl = ""
            //     },
            //     new TreatmentService
            //     {
            //         Id = 2,
            //         ServiceName = "Thụ tinh nhân tạo (IUI)",
            //         ServiceCode = "IUI001",
            //         Description = "Kỹ thuật đưa tinh trùng đã được xử lý vào buồng tử cung vào thời điểm rụng trứng.",
            //         BasePrice = 15000000,
            //         Procedures = "Khám sàng lọc → Theo dõi rụng trứng → Xử lý tinh trùng → Bơm tinh trùng vào tử cung",
            //         Requirements = "Vòi trứng thông thoáng, tinh trùng đạt chất lượng tối thiểu",
            //         DurationDays = 14,
            //         SuccessRate = 35.2f,
            //         IsActive = true,
            //         CreatedAt = DateTime.Now,
            //         ImageUrl = ""
            //     },
            //     new TreatmentService
            //     {
            //         Id = 3,
            //         ServiceName = "IVF với ICSI",
            //         ServiceCode = "ICSI001",
            //         Description = "Kỹ thuật tiêm tinh trùng vào bào tương trứng, áp dụng cho các trường hợp nam giới có chất lượng tinh trùng kém.",
            //         BasePrice = 95000000,
            //         Procedures = "Quy trình IVF kết hợp với kỹ thuật ICSI",
            //         Requirements = "Tinh trùng số lượng ít hoặc chất lượng kém",
            //         DurationDays = 35,
            //         SuccessRate = 72.3f,
            //         IsActive = true,
            //         CreatedAt = DateTime.Now,
            //         ImageUrl = ""
            //     },
            //     new TreatmentService
            //     {
            //         Id = 4,
            //         ServiceName = "Điều trị nội khoa hiếm muộn",
            //         ServiceCode = "MED001",
            //         Description = "Điều trị bằng thuốc cho các trường hợp rối loạn nội tiết, PCOS, rối loạn tinh trùng.",
            //         BasePrice = 5000000,
            //         Procedures = "Khám và chẩn đoán → Điều trị nội khoa → Theo dõi đáp ứng",
            //         Requirements = "Khám tổng quát, xét nghiệm chuyên sâu",
            //         DurationDays = 90,
            //         SuccessRate = 45.7f,
            //         IsActive = true,
            //         CreatedAt = DateTime.Now,
            //         ImageUrl = ""
            //     }
            // );
        }
    }
} 