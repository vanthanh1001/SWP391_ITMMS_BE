using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        private readonly string[] _timeSlots = new[]
        {
            "08:00-09:00", "09:00-10:00", "10:00-11:00", "11:00-12:00",
            "13:00-14:00", "14:00-15:00", "15:00-16:00", "16:00-17:00"
        };

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment> CreateAppointmentAsync(int customerId, CreateAppointmentDto appointmentDto)
        {
            // Validate customer exists
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw new InvalidOperationException("Khách hàng không tồn tại");

            // Validate doctor exists and is available
            var doctor = await _context.Doctors.FindAsync(appointmentDto.DoctorId);
            if (doctor == null || !doctor.IsAvailable)
                throw new InvalidOperationException("Bác sĩ không tồn tại hoặc không khả dụng");

            // Check if time slot is available
            if (!await IsTimeSlotAvailableAsync(appointmentDto.DoctorId, appointmentDto.AppointmentDate, appointmentDto.TimeSlot))
                throw new InvalidOperationException("Khung giờ này đã được đặt");

            var appointment = new Appointment
            {
                CustomerId = customerId,
                DoctorId = appointmentDto.DoctorId,
                TreatmentPlanId = appointmentDto.TreatmentPlanId,
                AppointmentDate = appointmentDto.AppointmentDate,
                TimeSlot = appointmentDto.TimeSlot,
                Type = appointmentDto.Type,
                Status = "Scheduled",
                Notes = appointmentDto.Notes ?? ""
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return await GetAppointmentByIdAsync(appointment.Id) ?? appointment;
        }

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

        public async Task<IEnumerable<string>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && 
                           a.AppointmentDate.Date == date.Date && 
                           a.Status != "Cancelled")
                .Select(a => a.TimeSlot)
                .ToListAsync();

            return _timeSlots.Except(bookedSlots);
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