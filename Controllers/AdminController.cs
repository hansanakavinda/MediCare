using MediCare.Helpers;
using MediCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Dashboard()
        {
            var stats = new
            {
                TotalPatients = await _db.PATIENTs.CountAsync(),
                TotalDoctors = await _db.DOCTORs.CountAsync(),
                TotalAppointments = await _db.APPOINTMENTs.CountAsync(),
                PendingAppointments = await _db.APPOINTMENTs.CountAsync(a => a.STATUS == "Pending")
            };

            ViewBag.Stats = stats;
            return View();
        }

        // Manage Doctors
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _db.DOCTORs
                .Include(d => d.USER)
                .Include(d => d.SPECIALTY)
                .Include(d => d.CLINIC)
                .ToListAsync();
            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> AddDoctor()
        {
            ViewBag.Specialties = await _db.SPECIALTies.ToListAsync();
            ViewBag.Clinics = await _db.CLINICs.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(string email, string password, string fullName, string phone,
            decimal specialtyId, decimal clinicId, string bio, decimal fee)
        {
            if (await _db.USERs.AnyAsync(u => u.EMAIL == email))
            {
                ViewBag.Error = "Email already exists";
                ViewBag.Specialties = await _db.SPECIALTies.ToListAsync();
                ViewBag.Clinics = await _db.CLINICs.ToListAsync();
                return View();
            }

            // Create user
            var user = new USER
            {
                EMAIL = email,
                PWD = password,
                FULL_NAME = fullName,
                PHONE = phone,
                ROLE = "Doctor",
                IS_ACTIVE = "Y",
                CREATED_AT = DateTime.Now
            };

            _db.USERs.Add(user);
            await _db.SaveChangesAsync();

            // Create doctor
            var doctor = new DOCTOR
            {
                USER_ID = user.USER_ID,
                SPECIALTY_ID = specialtyId,
                CLINIC_ID = clinicId,
                BIO = bio,
                FEE = fee
            };

            _db.DOCTORs.Add(doctor);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Doctor added successfully!";
            return RedirectToAction("Doctors");
        }

        [HttpGet]
        public async Task<IActionResult> EditDoctor(int id)  // Changed from doctorId to id
        {
            var doctor = await _db.DOCTORs
                .Include(d => d.USER)
                .Include(d => d.SPECIALTY)
                .Include(d => d.CLINIC)
                .FirstOrDefaultAsync(d => d.DOCTOR_ID == id);

            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.Specialties = await _db.SPECIALTies.ToListAsync();
            ViewBag.Clinics = await _db.CLINICs.ToListAsync();

            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctor(int id, string fullName, string phone,
            decimal specialtyId, decimal clinicId, string bio, decimal fee)  // Changed doctorId to id
        {
            var doctor = await _db.DOCTORs
                .Include(d => d.USER)
                .FirstOrDefaultAsync(d => d.DOCTOR_ID == id);

            if (doctor == null)
            {
                return NotFound();
            }

            // Update user information
            doctor.USER.FULL_NAME = fullName;
            doctor.USER.PHONE = phone;

            // Update doctor information
            doctor.SPECIALTY_ID = specialtyId;
            doctor.CLINIC_ID = clinicId;
            doctor.BIO = bio;
            doctor.FEE = fee;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Doctor updated successfully!";
            return RedirectToAction("Doctors");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            var doctor = await _db.DOCTORs.Include(d => d.USER).FirstOrDefaultAsync(d => d.DOCTOR_ID == doctorId);
            if (doctor != null)
            {
                doctor.USER.IS_ACTIVE = "N";
                await _db.SaveChangesAsync();
                TempData["Success"] = "Doctor deactivated successfully!";
            }
            return RedirectToAction("Doctors");
        }

        // Manage Specialties
        public async Task<IActionResult> Specialties()
        {
            var specialties = await _db.SPECIALTies.ToListAsync();
            return View(specialties);
        }

        [HttpPost]
        public async Task<IActionResult> AddSpecialty(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var specialty = new SPECIALTY { NAME = name };
                _db.SPECIALTies.Add(specialty);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Specialty added successfully!";
            }
            return RedirectToAction("Specialties");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSpecialty(decimal specialtyId)
        {
            var specialty = await _db.SPECIALTies.FindAsync(specialtyId);
            if (specialty != null)
            {
                // Check if specialty is used by any doctors before deleting
                bool isUsed = await _db.DOCTORs.AnyAsync(d => d.SPECIALTY_ID == specialtyId);

                if (isUsed)
                {
                    TempData["Error"] = "Cannot delete specialty as it is assigned to one or more doctors.";
                }
                else
                {
                    _db.SPECIALTies.Remove(specialty);
                    await _db.SaveChangesAsync();
                    TempData["Success"] = "Specialty deleted successfully!";
                }
            }
            return RedirectToAction("Specialties");
        }

        // Manage Patients
        public async Task<IActionResult> Patients()
        {
            var patients = await _db.PATIENTs
                .Include(p => p.USER)
                .ToListAsync();
            return View(patients);
        }

        // View All Appointments
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _db.APPOINTMENTs
                .Include(a => a.PATIENT).ThenInclude(p => p.USER)
                .Include(a => a.DOCTOR).ThenInclude(d => d.USER)
                .OrderByDescending(a => a.CREATED_AT)
                .ToListAsync();
            return View(appointments);
        }

        // Manage Doctor Schedules
        public async Task<IActionResult> DoctorSchedules()
        {
            var doctors = await _db.DOCTORs
                .Include(d => d.USER)
                .Include(d => d.SPECIALTY)
                .Include(d => d.SCHEDULEs)
                .Where(d => d.USER.IS_ACTIVE == "Y")
                .ToListAsync();
            return View(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDoctorSchedule(int doctorId, List<ScheduleViewModel> schedules)
        {
            var doctor = await _db.DOCTORs.FirstOrDefaultAsync(d => d.DOCTOR_ID == doctorId);

            if (doctor == null) return NotFound();

            try
            {
                // Get existing schedules
                var existingSchedules = await _db.SCHEDULEs
                    .Where(s => s.DOCTOR_ID == doctor.DOCTOR_ID)
                    .Include(s => s.APPOINTMENTs) // Include appointments to check constraints
                    .ToListAsync();

                // Keep track of schedules we've processed
                var processedScheduleIds = new List<decimal>();

                // Update or create schedules
                foreach (var scheduleVM in schedules.Where(s => s.IsAvailable))
                {
                    var existingSchedule = existingSchedules.FirstOrDefault(s => s.DAY_OF_WEEK == scheduleVM.DayOfWeek);

                    if (existingSchedule != null)
                    {
                        // Update existing schedule
                        existingSchedule.START_TIME = scheduleVM.StartTime;
                        existingSchedule.END_TIME = scheduleVM.EndTime;
                        processedScheduleIds.Add(existingSchedule.SCHEDULE_ID);
                    }
                    else
                    {
                        // Create new schedule
                        var newSchedule = new SCHEDULE
                        {
                            DOCTOR_ID = doctor.DOCTOR_ID,
                            DAY_OF_WEEK = scheduleVM.DayOfWeek,
                            START_TIME = scheduleVM.StartTime,
                            END_TIME = scheduleVM.EndTime,
                            SLOT_MINUTES = 30 // Default 30 minutes per slot
                        };
                        _db.SCHEDULEs.Add(newSchedule);
                    }
                }

                // Find schedules to delete (schedules that exist but weren't processed)
                var schedulesToDelete = existingSchedules
                    .Where(s => !processedScheduleIds.Contains(s.SCHEDULE_ID) && !s.APPOINTMENTs.Any())
                    .ToList();

                // Only delete schedules that don't have appointments
                _db.SCHEDULEs.RemoveRange(schedulesToDelete);

                // Save changes
                await _db.SaveChangesAsync();
                TempData["Success"] = "Doctor schedule updated successfully!";
            }
            catch (Exception ex)
            {
                // Log the error and provide a more user-friendly message
                TempData["Error"] = "Could not update schedule. Some time slots may have appointments scheduled.";
            }

            return RedirectToAction("DoctorSchedules");
        }

        [HttpGet]
        public async Task<IActionResult> EditDoctorSchedule(int id)
        {
            var doctor = await _db.DOCTORs
                .Include(d => d.USER)
                .Include(d => d.SPECIALTY)
                .Include(d => d.SCHEDULEs)
                    .ThenInclude(s => s.APPOINTMENTs)
                .FirstOrDefaultAsync(d => d.DOCTOR_ID == id);

            if (doctor == null)
            {
                return NotFound();
            }

            // Prepare the schedule view models
            var scheduleViewModels = new List<ScheduleViewModel>();

            // Create entries for all days of the week
            for (int i = 0; i < 7; i++)
            {
                bool dayOfWeek = Convert.ToBoolean(i);
                string dayName = GetDayName(i);

                var existingSchedule = doctor.SCHEDULEs.FirstOrDefault(s => s.DAY_OF_WEEK == dayOfWeek);

                scheduleViewModels.Add(new ScheduleViewModel
                {
                    DayOfWeek = dayOfWeek,
                    DayName = dayName,
                    StartTime = existingSchedule?.START_TIME ?? "09:00",
                    EndTime = existingSchedule?.END_TIME ?? "17:00",
                    IsAvailable = existingSchedule != null,
                    ScheduleId = existingSchedule?.SCHEDULE_ID ?? 0,
                    HasAppointments = existingSchedule?.APPOINTMENTs.Any() ?? false
                });
            }

            ViewBag.ScheduleViewModels = scheduleViewModels;
            return View(doctor);
        }

        private string GetDayName(int day)
        {
            return day switch
            {
                0 => "Sunday",
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                6 => "Saturday",
                _ => "Unknown"
            };
        }

        // Helper class for schedule form - updated to include schedule ID and appointment info
        public class ScheduleViewModel
        {
            public bool DayOfWeek { get; set; }
            public string DayName { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public bool IsAvailable { get; set; }
            public decimal ScheduleId { get; set; }
            public bool HasAppointments { get; set; }
        }
    }
}