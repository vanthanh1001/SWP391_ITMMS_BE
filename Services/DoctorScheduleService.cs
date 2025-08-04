using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly AppDbContext _context;

        public DoctorScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorTimeSlot>> GenerateTimeSlotsAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            var schedules = await _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId && ds.IsActive)
                .ToListAsync();

            var leaves = await _context.DoctorLeaves
                .Where(dl => dl.DoctorId == doctorId && dl.IsApproved)
                .ToListAsync();

            var timeSlots = new List<DoctorTimeSlot>();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                // Kiểm tra nghỉ phép
                var leave = leaves.FirstOrDefault(l => 
                    date >= l.StartDate.Date && date <= l.EndDate.Date);
                if (leave != null) continue;

                var schedule = schedules.FirstOrDefault(s => s.DayOfWeek == date.DayOfWeek);
                if (schedule == null) continue;

                // Tạo các slot trong ngày
                var currentTime = schedule.StartTime;
                while (currentTime < schedule.EndTime)
                {
                    var endTime = currentTime.Add(TimeSpan.FromMinutes(schedule.SlotDurationMinutes));
                    if (endTime > schedule.EndTime) break;

                    var timeSlot = new DoctorTimeSlot
                    {
                        DoctorId = doctorId,
                        Date = date,
                        StartTime = currentTime,
                        EndTime = endTime,
                        MaxPatients = schedule.MaxPatientsPerSlot,
                        CurrentPatients = 0,
                        IsAvailable = true
                    };

                    timeSlots.Add(timeSlot);
                    currentTime = endTime;
                }
            }

            // Lưu vào database
            _context.DoctorTimeSlots.AddRange(timeSlots);
            await _context.SaveChangesAsync();

            return timeSlots;
        }

        public async Task<List<DoctorTimeSlot>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            // Lấy các slot có sẵn
            var availableSlots = await _context.DoctorTimeSlots
                .Where(dts => dts.DoctorId == doctorId 
                           && dts.Date.Date == date.Date
                           && dts.IsAvailable
                           && dts.CurrentPatients < dts.MaxPatients)
                .OrderBy(dts => dts.StartTime)
                .ToListAsync();

            // Nếu chưa có slot cho ngày này, tạo mới
            if (!availableSlots.Any())
            {
                await GenerateTimeSlotsAsync(doctorId, date, date);
                availableSlots = await _context.DoctorTimeSlots
                    .Where(dts => dts.DoctorId == doctorId 
                               && dts.Date.Date == date.Date
                               && dts.IsAvailable
                               && dts.CurrentPatients < dts.MaxPatients)
                    .OrderBy(dts => dts.StartTime)
                    .ToListAsync();
            }

            return availableSlots;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan startTime)
        {
            var timeSlot = await _context.DoctorTimeSlots
                .FirstOrDefaultAsync(dts => dts.DoctorId == doctorId
                                        && dts.Date.Date == date.Date
                                        && dts.StartTime == startTime
                                        && dts.IsAvailable
                                        && dts.CurrentPatients < dts.MaxPatients);

            return timeSlot != null;
        }

        public async Task<bool> BookTimeSlotAsync(int doctorId, DateTime date, TimeSpan startTime)
        {
            var timeSlot = await _context.DoctorTimeSlots
                .FirstOrDefaultAsync(dts => dts.DoctorId == doctorId
                                        && dts.Date.Date == date.Date
                                        && dts.StartTime == startTime
                                        && dts.IsAvailable
                                        && dts.CurrentPatients < dts.MaxPatients);

            if (timeSlot == null) return false;

            timeSlot.CurrentPatients++;
            if (timeSlot.CurrentPatients >= timeSlot.MaxPatients)
            {
                timeSlot.IsAvailable = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseTimeSlotAsync(int doctorId, DateTime date, TimeSpan startTime)
        {
            var timeSlot = await _context.DoctorTimeSlots
                .FirstOrDefaultAsync(dts => dts.DoctorId == doctorId
                                        && dts.Date.Date == date.Date
                                        && dts.StartTime == startTime);

            if (timeSlot != null)
            {
                timeSlot.CurrentPatients = Math.Max(0, timeSlot.CurrentPatients - 1);
                if (timeSlot.CurrentPatients < timeSlot.MaxPatients)
                {
                    timeSlot.IsAvailable = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<DoctorLeave>> GetDoctorLeavesAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            return await _context.DoctorLeaves
                .Where(dl => dl.DoctorId == doctorId
                         && dl.StartDate >= startDate
                         && dl.EndDate <= endDate)
                .OrderBy(dl => dl.StartDate)
                .ToListAsync();
        }

        public async Task<bool> RequestLeaveAsync(DoctorLeave leave)
        {
            // Validate leave dates
            if (leave.StartDate < DateTime.Today)
                throw new InvalidOperationException("Không thể xin nghỉ trong quá khứ");

            if (leave.EndDate < leave.StartDate)
                throw new InvalidOperationException("Ngày kết thúc phải sau ngày bắt đầu");

            // Kiểm tra xung đột với lịch hẹn
            var conflictingAppointments = await _context.Appointments
                .AnyAsync(a => a.DoctorId == leave.DoctorId
                           && a.AppointmentDate >= leave.StartDate
                           && a.AppointmentDate <= leave.EndDate
                           && a.Status == "Scheduled");

            if (conflictingAppointments)
                throw new InvalidOperationException("Có lịch hẹn trong thời gian xin nghỉ");

            _context.DoctorLeaves.Add(leave);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateScheduleAsync(DoctorSchedule schedule)
        {
            var existingSchedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(ds => ds.Id == schedule.Id);

            if (existingSchedule == null)
                return false;

            existingSchedule.DayOfWeek = schedule.DayOfWeek;
            existingSchedule.StartTime = schedule.StartTime;
            existingSchedule.EndTime = schedule.EndTime;
            existingSchedule.MaxPatientsPerSlot = schedule.MaxPatientsPerSlot;
            existingSchedule.SlotDurationMinutes = schedule.SlotDurationMinutes;
            existingSchedule.IsActive = schedule.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }
    }
} 