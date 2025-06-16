using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<TreatmentHistory> TreatmentHistories { get; set; }
        public DbSet<UserFeedback> UserFeedbacks { get; set; }
    }
} 