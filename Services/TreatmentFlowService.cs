using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public class TreatmentFlowService : ITreatmentFlowService
    {
        private readonly AppDbContext _context;

        public TreatmentFlowService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetPatientTreatmentFlowAsync(int customerId)
        {
            var treatmentFlow = await _context.TreatmentPlans
                .Include(tp => tp.Customer)
                    .ThenInclude(c => c.User)
                .Include(tp => tp.Doctor)
                    .ThenInclude(d => d.User)
                .Include(tp => tp.TreatmentService)
                .Include(tp => tp.Appointments)
                    .ThenInclude(a => a.MedicalRecord)
                .Include(tp => tp.Customer.MedicalRecords)
                .Include(tp => tp.Customer.TestResults)
                .Where(tp => tp.CustomerId == customerId)
                .OrderByDescending(tp => tp.StartDate)
                .Select(tp => new
                {
                    TreatmentPlan = new
                    {
                        tp.Id,
                        tp.TreatmentType,
                        tp.Description,
                        tp.StartDate,
                        tp.EndDate,
                        tp.Status,
                        tp.CurrentPhase,
                        tp.PhaseDescription,
                        tp.NextPhaseDate,
                        tp.NextVisitDate,
                        tp.TotalCost,
                        tp.PaidAmount,
                        tp.PaymentStatus,
                        tp.Notes,
                        tp.ProgressNotes,
                        Doctor = new
                        {
                            tp.Doctor.Id,
                            Name = tp.Doctor.User.FullName,
                            tp.Doctor.Specialization,
                            tp.Doctor.ExperienceYears
                        },
                        TreatmentService = tp.TreatmentService != null ? new
                        {
                            tp.TreatmentService.Id,
                            tp.TreatmentService.ServiceName,
                            tp.TreatmentService.ServiceCode,
                            tp.TreatmentService.Description,
                            tp.TreatmentService.BasePrice,
                            tp.TreatmentService.DurationDays,
                            tp.TreatmentService.SuccessRate
                        } : null
                    },
                    Appointments = tp.Appointments.OrderBy(a => a.AppointmentDate).Select(a => new
                    {
                        a.Id,
                        a.AppointmentDate,
                        a.TimeSlot,
                        a.Type,
                        a.Status,
                        a.Notes,
                        MedicalRecord = a.MedicalRecord != null ? new
                        {
                            a.MedicalRecord.Id,
                            a.MedicalRecord.RecordDate,
                            a.MedicalRecord.Symptoms,
                            a.MedicalRecord.Diagnosis,
                            a.MedicalRecord.Treatment,
                            a.MedicalRecord.Prescription,
                            a.MedicalRecord.Notes
                        } : null
                    }),
                    MedicalHistory = tp.Customer.MedicalRecords.OrderByDescending(mr => mr.RecordDate).Select(mr => new
                    {
                        mr.Id,
                        mr.RecordDate,
                        mr.Symptoms,
                        mr.Diagnosis,
                        mr.Treatment,
                        mr.Prescription,
                        mr.Notes,
                        DoctorName = mr.Doctor.User.FullName
                    }),
                                            TestResults = tp.Customer.TestResults.OrderByDescending(tr => tr.TestDate).Select(tr => new
                        {
                            tr.Id,
                            tr.TestName,
                            tr.TestType,
                            tr.Results,
                            tr.NormalRange,
                            tr.Status,
                            tr.TestDate,
                            DoctorName = tr.Doctor.User.FullName
                        })
                })
                .ToListAsync();

            return treatmentFlow;
        }

        public async Task<object> UpdateTreatmentPhaseAsync(int treatmentPlanId, UpdateTreatmentPhaseDto dto)
        {
            var treatmentPlan = await _context.TreatmentPlans
                .Include(tp => tp.Customer)
                .Include(tp => tp.Doctor)
                .FirstOrDefaultAsync(tp => tp.Id == treatmentPlanId);

            if (treatmentPlan == null)
            {
                throw new InvalidOperationException("Không tìm thấy kế hoạch điều trị");
            }

            // 更新治疗阶段
            treatmentPlan.CurrentPhase = dto.CurrentPhase;
            treatmentPlan.PhaseDescription = dto.PhaseDescription;
            treatmentPlan.NextPhaseDate = dto.NextPhaseDate;
            treatmentPlan.NextVisitDate = dto.NextVisitDate;
            treatmentPlan.ProgressNotes = dto.ProgressNotes;
            treatmentPlan.Notes = dto.Notes;

            if (dto.Status != null)
            {
                treatmentPlan.Status = dto.Status;
            }

            await _context.SaveChangesAsync();

            return new
            {
                treatmentPlan.Id,
                treatmentPlan.CurrentPhase,
                treatmentPlan.PhaseDescription,
                treatmentPlan.NextPhaseDate,
                treatmentPlan.NextVisitDate,
                treatmentPlan.Status,
                treatmentPlan.ProgressNotes
            };
        }

        public async Task<object> CreateMedicalRecordAsync(CreateMedicalRecordDto dto)
        {
            // 验证患者和医生
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                throw new InvalidOperationException("Không tìm thấy bệnh nhân");
            }

            var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
            if (doctor == null)
            {
                throw new InvalidOperationException("Không tìm thấy bác sĩ");
            }

            var medicalRecord = new MedicalRecord
            {
                CustomerId = dto.CustomerId,
                DoctorId = dto.DoctorId,
                AppointmentId = dto.AppointmentId,
                Symptoms = dto.Symptoms,
                Diagnosis = dto.Diagnosis,
                Treatment = dto.Treatment,
                Prescription = dto.Prescription,
                Notes = dto.Notes,
                RecordDate = DateTime.Now
            };

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            return new
            {
                medicalRecord.Id,
                medicalRecord.RecordDate,
                medicalRecord.Symptoms,
                medicalRecord.Diagnosis,
                medicalRecord.Treatment,
                medicalRecord.Prescription,
                medicalRecord.Notes
            };
        }

        public async Task<object> CreateTestResultAsync(CreateTestResultDto dto)
        {
            var testResult = new TestResult
            {
                CustomerId = dto.CustomerId,
                DoctorId = dto.DoctorId,
                TestName = dto.TestName,
                TestType = dto.TestType,
                Results = dto.Results,
                NormalRange = dto.NormalRange,
                Status = dto.Status,
                TestDate = DateTime.Now
            };

            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync();

            return new
            {
                testResult.Id,
                testResult.TestName,
                testResult.TestType,
                testResult.Results,
                testResult.NormalRange,
                testResult.Status,
                testResult.TestDate
            };
        }

        public async Task<object> GetTreatmentRemindersAsync(int customerId)
        {
            var reminders = await _context.TreatmentPlans
                .Include(tp => tp.Appointments)
                .Where(tp => tp.CustomerId == customerId && tp.Status == "Active")
                .Select(tp => new
                {
                    TreatmentPlan = new
                    {
                        tp.Id,
                        tp.TreatmentType,
                        tp.CurrentPhase,
                        tp.PhaseDescription,
                        tp.NextPhaseDate,
                        tp.NextVisitDate,
                        tp.ProgressNotes
                    },
                    UpcomingAppointments = tp.Appointments
                        .Where(a => a.AppointmentDate >= DateTime.Today && a.Status == "Scheduled")
                        .OrderBy(a => a.AppointmentDate)
                        .Select(a => new
                        {
                            a.Id,
                            a.AppointmentDate,
                            a.TimeSlot,
                            a.Type,
                            a.Notes
                        }),
                    PhaseReminders = new
                    {
                        NextPhaseDate = tp.NextPhaseDate,
                        NextVisitDate = tp.NextVisitDate,
                        CurrentPhase = tp.CurrentPhase,
                        PhaseDescription = tp.PhaseDescription
                    }
                })
                .ToListAsync();

            return reminders;
        }

        public async Task<object> GetTreatmentProgressStatsAsync(int customerId)
        {
            var stats = await _context.TreatmentPlans
                .Where(tp => tp.CustomerId == customerId)
                .GroupBy(tp => tp.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalCost = g.Sum(tp => tp.TotalCost),
                    PaidAmount = g.Sum(tp => tp.PaidAmount)
                })
                .ToListAsync();

            var activePlan = await _context.TreatmentPlans
                .Where(tp => tp.CustomerId == customerId && tp.Status == "Active")
                .Select(tp => new
                {
                    tp.Id,
                    tp.TreatmentType,
                    tp.CurrentPhase,
                    tp.PhaseDescription,
                    tp.StartDate,
                    DaysInTreatment = (DateTime.Now - tp.StartDate).Days,
                    ProgressPercentage = tp.CurrentPhase * 20 // 假设每个阶段20%
                })
                .FirstOrDefaultAsync();

            return new
            {
                Stats = stats,
                ActivePlan = activePlan
            };
        }

        public async Task<object> GetTreatmentTimelineAsync(int customerId)
        {
            var timeline = await _context.TreatmentPlans
                .Include(tp => tp.Appointments.OrderBy(a => a.AppointmentDate))
                .Include(tp => tp.Customer.MedicalRecords.OrderBy(mr => mr.RecordDate))
                .Include(tp => tp.Customer.TestResults.OrderBy(tr => tr.TestDate))
                .Where(tp => tp.CustomerId == customerId)
                .Select(tp => new
                {
                    TreatmentPlan = new
                    {
                        tp.Id,
                        tp.TreatmentType,
                        tp.StartDate,
                        tp.Status,
                        tp.CurrentPhase,
                        tp.PhaseDescription
                    },
                    Timeline = tp.Appointments.Select(a => new
                    {
                        Date = a.AppointmentDate,
                        Type = "Appointment",
                        Title = $"Lịch hẹn - {a.Type}",
                        Description = a.Notes,
                        Status = a.Status
                    })
                    .Concat(tp.Customer.MedicalRecords.Select(mr => new
                    {
                        Date = mr.RecordDate,
                        Type = "MedicalRecord",
                        Title = "Hồ sơ y tế",
                        Description = mr.Diagnosis,
                        Status = "Completed"
                    }))
                    .Concat(tp.Customer.TestResults.Select(tr => new
                    {
                        Date = tr.TestDate,
                        Type = "TestResult",
                        Title = $"Kết quả xét nghiệm - {tr.TestName}",
                        Description = tr.Results,
                        Status = tr.Status
                    }))
                    .OrderBy(t => t.Date)
                })
                .ToListAsync();

            return timeline;
        }

        public async Task<object> GetNextTreatmentStepsAsync(int treatmentPlanId)
        {
            var treatmentPlan = await _context.TreatmentPlans
                .Include(tp => tp.TreatmentService)
                .FirstOrDefaultAsync(tp => tp.Id == treatmentPlanId);

            if (treatmentPlan == null)
            {
                throw new InvalidOperationException("Không tìm thấy kế hoạch điều trị");
            }

            var nextSteps = new List<object>();

            // 根据治疗类型和当前阶段确定下一步
            switch (treatmentPlan.TreatmentType.ToUpper())
            {
                case "IVF":
                    nextSteps = GetIVFNextSteps(treatmentPlan.CurrentPhase);
                    break;
                case "IUI":
                    nextSteps = GetIUINextSteps(treatmentPlan.CurrentPhase);
                    break;
                default:
                    nextSteps = GetGeneralNextSteps(treatmentPlan.CurrentPhase);
                    break;
            }

            return new
            {
                CurrentPhase = treatmentPlan.CurrentPhase,
                PhaseDescription = treatmentPlan.PhaseDescription,
                NextSteps = nextSteps,
                EstimatedCompletion = treatmentPlan.NextPhaseDate
            };
        }

        private List<object> GetIVFNextSteps(int currentPhase)
        {
            var steps = new List<object>();

            switch (currentPhase)
            {
                case 1:
                    steps.Add(new { Step = 1, Title = "Khám sức khỏe tổng quát", Description = "Kiểm tra sức khỏe và xét nghiệm cơ bản", Duration = "1-2 ngày" });
                    steps.Add(new { Step = 2, Title = "Kích thích buồng trứng", Description = "Tiêm thuốc kích thích buồng trứng", Duration = "8-12 ngày" });
                    break;
                case 2:
                    steps.Add(new { Step = 3, Title = "Chọc hút trứng", Description = "Thủ thuật chọc hút trứng", Duration = "1 ngày" });
                    steps.Add(new { Step = 4, Title = "Thụ tinh trong ống nghiệm", Description = "Thụ tinh trứng với tinh trùng", Duration = "3-5 ngày" });
                    break;
                case 3:
                    steps.Add(new { Step = 5, Title = "Chuyển phôi", Description = "Chuyển phôi vào tử cung", Duration = "1 ngày" });
                    steps.Add(new { Step = 6, Title = "Theo dõi thai", Description = "Theo dõi và kiểm tra thai", Duration = "14 ngày" });
                    break;
            }

            return steps;
        }

        private List<object> GetIUINextSteps(int currentPhase)
        {
            var steps = new List<object>();

            switch (currentPhase)
            {
                case 1:
                    steps.Add(new { Step = 1, Title = "Khám sức khỏe", Description = "Kiểm tra sức khỏe và xét nghiệm", Duration = "1-2 ngày" });
                    steps.Add(new { Step = 2, Title = "Kích thích rụng trứng", Description = "Tiêm thuốc kích thích rụng trứng", Duration = "5-7 ngày" });
                    break;
                case 2:
                    steps.Add(new { Step = 3, Title = "Thụ tinh nhân tạo", Description = "Thực hiện thụ tinh nhân tạo", Duration = "1 ngày" });
                    steps.Add(new { Step = 4, Title = "Theo dõi thai", Description = "Theo dõi và kiểm tra thai", Duration = "14 ngày" });
                    break;
            }

            return steps;
        }

        private List<object> GetGeneralNextSteps(int currentPhase)
        {
            return new List<object>
            {
                new { Step = currentPhase + 1, Title = "Giai đoạn tiếp theo", Description = "Thực hiện các bước điều trị tiếp theo", Duration = "Theo lịch trình" }
            };
        }
    }
} 