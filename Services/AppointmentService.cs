using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        private readonly IDoctorScheduleService _scheduleService;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(AppDbContext context, IDoctorScheduleService scheduleService, ILogger<AppointmentService> logger)
        {
            _context = context;
            _scheduleService = scheduleService;
            _logger = logger;
        }

        public async Task<Appointment> CreateAppointmentAsync(int customerId, CreateAppointmentDto appointmentDto)
        {
            // Validate customer và doctor
            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer == null)
                throw new InvalidOperationException("Khách hàng không tồn tại");

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == appointmentDto.DoctorId);
            if (doctor == null || !doctor.IsAvailable)
                throw new InvalidOperationException("Bác sĩ không khả dụng");

            // Validate thời gian
            if (appointmentDto.AppointmentDate.Date < DateTime.Today)
                throw new InvalidOperationException("Không thể đặt lịch trong quá khứ");

            // Kiểm tra slot có sẵn không
            var startTime = TimeSpan.Parse(appointmentDto.TimeSlot.Split('-')[0]);
            if (!await _scheduleService.IsTimeSlotAvailableAsync(
                appointmentDto.DoctorId, 
                appointmentDto.AppointmentDate, 
                startTime))
            {
                throw new InvalidOperationException("Khung giờ này không khả dụng");
            }

            // Book slot
            await _scheduleService.BookTimeSlotAsync(
                appointmentDto.DoctorId,
                appointmentDto.AppointmentDate,
                startTime);

            // Tạo appointment
            var appointment = new Appointment
            {
                CustomerId = customerId,
                DoctorId = appointmentDto.DoctorId,
                TreatmentPlanId = appointmentDto.TreatmentPlanId,
                AppointmentDate = appointmentDto.AppointmentDate,
                TimeSlot = appointmentDto.TimeSlot,
                Type = appointmentDto.Type,
                Status = "Scheduled",
                Notes = appointmentDto.Notes ?? "",
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, CancelAppointmentDto dto, string userId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new InvalidOperationException("Không tìm thấy lịch hẹn");

            // Kiểm tra quyền hủy
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
                throw new InvalidOperationException("Người dùng không hợp lệ");

            bool canCancel = false;
            if (user.Role == "Admin")
            {
                canCancel = true;
            }
            else if (user.Role == "Doctor" && appointment.DoctorId == int.Parse(userId))
            {
                canCancel = true;
            }
            else if (user.Role == "Customer" && appointment.CustomerId == int.Parse(userId))
            {
                // Kiểm tra thời gian hủy (ít nhất 2 giờ trước)
                var timeUntilAppointment = appointment.AppointmentDate - DateTime.Now;
                if (timeUntilAppointment.TotalHours < 2)
                {
                    throw new InvalidOperationException("Chỉ có thể hủy lịch hẹn ít nhất 2 giờ trước");
                }
                canCancel = true;
            }

            if (!canCancel)
                throw new InvalidOperationException("Bạn không có quyền hủy lịch hẹn này");

            // Cập nhật trạng thái
            appointment.Status = "Cancelled";
            appointment.CancellationReason = dto.Reason;
            appointment.CancelledBy = dto.CancelledBy;
            appointment.CancelledAt = DateTime.Now;
            appointment.CancellationNotes = dto.Notes;
            appointment.LastModified = DateTime.Now;

            // Giải phóng slot
            var startTime = TimeSpan.Parse(appointment.TimeSlot.Split('-')[0]);
            await _scheduleService.ReleaseTimeSlotAsync(
                appointment.DoctorId,
                appointment.AppointmentDate,
                startTime);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Appointment {appointmentId} cancelled by {user.Role} {userId}");

            return true;
        }

        public async Task<bool> RescheduleAppointmentAsync(int appointmentId, RescheduleAppointmentDto dto, string userId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new InvalidOperationException("Không tìm thấy lịch hẹn");

            if (appointment.Status != "Scheduled")
                throw new InvalidOperationException("Chỉ có thể đổi lịch hẹn đang ở trạng thái Scheduled");

            // Kiểm tra quyền
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
                throw new InvalidOperationException("Người dùng không hợp lệ");

            bool canReschedule = false;
            if (user.Role == "Admin" || user.Role == "Doctor")
            {
                canReschedule = true;
            }
            else if (user.Role == "Customer" && appointment.CustomerId == int.Parse(userId))
            {
                // Kiểm tra thời gian đổi lịch (ít nhất 4 giờ trước)
                var timeUntilAppointment = appointment.AppointmentDate - DateTime.Now;
                if (timeUntilAppointment.TotalHours < 4)
                {
                    throw new InvalidOperationException("Chỉ có thể đổi lịch hẹn ít nhất 4 giờ trước");
                }
                canReschedule = true;
            }

            if (!canReschedule)
                throw new InvalidOperationException("Bạn không có quyền đổi lịch hẹn này");

            // Validate thời gian mới
            if (dto.NewDate.Date < DateTime.Today)
                throw new InvalidOperationException("Không thể đặt lịch trong quá khứ");

            var newStartTime = TimeSpan.Parse(dto.NewTimeSlot.Split('-')[0]);
            if (!await _scheduleService.IsTimeSlotAvailableAsync(
                appointment.DoctorId, 
                dto.NewDate, 
                newStartTime))
            {
                throw new InvalidOperationException("Khung giờ mới không khả dụng");
            }

            // Giải phóng slot cũ
            var oldStartTime = TimeSpan.Parse(appointment.TimeSlot.Split('-')[0]);
            await _scheduleService.ReleaseTimeSlotAsync(
                appointment.DoctorId,
                appointment.AppointmentDate,
                oldStartTime);

            // Book slot mới
            await _scheduleService.BookTimeSlotAsync(
                appointment.DoctorId,
                dto.NewDate,
                newStartTime);

            // Cập nhật appointment
            appointment.AppointmentDate = dto.NewDate;
            appointment.TimeSlot = dto.NewTimeSlot;
            appointment.Notes = $"{appointment.Notes}\n[Đổi lịch] {dto.Reason} - {dto.RequestedBy}";
            appointment.LastModified = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<object>> GetAppointmentHistoryAsync(int customerId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.AppointmentDate,
                    a.TimeSlot,
                    a.Status,
                    a.Type,
                    DoctorName = a.Doctor.User.FullName,
                    a.CancellationReason,
                    a.CancelledBy,
                    a.CancelledAt,
                    a.CreatedAt,
                    a.LastModified
                })
                .ToListAsync();

            return appointments.Cast<object>().ToList();
        }

        public async Task<List<object>> GetDoctorAppointmentsAsync(int doctorId, DateTime? date = null)
        {
            var query = _context.Appointments
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Where(a => a.DoctorId == doctorId);

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.TimeSlot)
                .Select(a => new
                {
                    a.Id,
                    a.AppointmentDate,
                    a.TimeSlot,
                    a.Status,
                    a.Type,
                    CustomerName = a.Customer.User.FullName,
                    CustomerPhone = a.Customer.User.Phone,
                    a.Notes,
                    a.CancellationReason,
                    a.CancelledBy,
                    a.CreatedAt,
                    a.LastModified
                })
                .ToListAsync();

            return appointments.Cast<object>().ToList();
        }

        public async Task<List<object>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            var timeSlots = await _scheduleService.GetAvailableTimeSlotsAsync(doctorId, date);
            
            return timeSlots.Select(ts => new
            {
                startTime = ts.StartTime.ToString(@"hh\:mm"),
                endTime = ts.EndTime.ToString(@"hh\:mm"),
                timeSlot = $"{ts.StartTime:hh\\:mm}-{ts.EndTime:hh\\:mm}",
                availableSlots = ts.MaxPatients - ts.CurrentPatients,
                isAvailable = ts.IsAvailable && ts.CurrentPatients < ts.MaxPatients
            }).Cast<object>().ToList();
        }

        // Các method cũ giữ nguyên
        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.TreatmentPlan)
                .Include(a => a.MedicalRecord)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByCustomerAsync(int customerId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.TreatmentPlan)
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Include(a => a.TreatmentPlan)
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.TimeSlot)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.TimeSlot)
                .ToListAsync();
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            appointment.Status = status;
            _context.Appointments.Update(appointment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            return await UpdateAppointmentStatusAsync(id, "Cancelled");
        }

        public async Task<bool> RescheduleAppointmentAsync(int id, DateTime newDate, string newTimeSlot)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            // Check if new time slot is available
            if (!await IsTimeSlotAvailableAsync(appointment.DoctorId, newDate, newTimeSlot))
                throw new InvalidOperationException("Khung giờ mới đã được đặt");

            appointment.AppointmentDate = newDate;
            appointment.TimeSlot = newTimeSlot;
            _context.Appointments.Update(appointment);
            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<string>> GetLegacyAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && 
                           a.AppointmentDate.Date == date.Date && 
                           a.Status != "Cancelled")
                .Select(a => a.TimeSlot)
                .ToListAsync();

            var timeSlots = new[]
            {
                "08:00-09:00", "09:00-10:00", "10:00-11:00", "11:00-12:00",
                "13:00-14:00", "14:00-15:00", "15:00-16:00", "16:00-17:00"
            };

            return timeSlots.Except(bookedSlots);
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, string timeSlot)
        {
            return !await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId && 
                              a.AppointmentDate.Date == date.Date && 
                              a.TimeSlot == timeSlot && 
                              a.Status != "Cancelled");
        }
    }
} 