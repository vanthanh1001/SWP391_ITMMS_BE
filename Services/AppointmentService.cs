using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            // Check for conflicting appointments
            var hasConflict = await _context.Appointments
                .AnyAsync(a => a.DoctorId == appointment.DoctorId &&
                              a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                              a.Status != "Cancelled");

            if (hasConflict)
            {
                throw new InvalidOperationException("Doctor already has an appointment at this time");
            }

            appointment.CreatedAt = DateTime.UtcNow;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            var existingAppointment = await GetAppointmentByIdAsync(appointment.Id);
            if (existingAppointment == null)
                return false;

            // Check for conflicting appointments if date is being changed
            if (existingAppointment.AppointmentDate != appointment.AppointmentDate)
            {
                var hasConflict = await _context.Appointments
                    .AnyAsync(a => a.Id != appointment.Id &&
                                  a.DoctorId == appointment.DoctorId &&
                                  a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                                  a.Status != "Cancelled");

                if (hasConflict)
                {
                    throw new InvalidOperationException("Doctor already has an appointment at this time");
                }
            }

            appointment.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existingAppointment).CurrentValues.SetValues(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await GetAppointmentByIdAsync(id);
            if (appointment == null)
                return false;

            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteAppointmentAsync(int id)
        {
            var appointment = await GetAppointmentByIdAsync(id);
            if (appointment == null)
                return false;

            appointment.Status = "Completed";
            appointment.CompletedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 