using MediCare.Helpers;
using MediCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Controllers
{
    [AuthorizeRole("Doctor")]
    public class DoctorController : Controller
    {
        private readonly AppDbContext _db;
        public DoctorController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var doctor = await _db.DOCTORs.FirstOrDefaultAsync(d => d.USER_ID == userId);

            if (doctor == null) return NotFound();

            // Get today's appointments
            var todayAppointments = await _db.APPOINTMENTs
                .Include(a => a.PATIENT).ThenInclude(p => p.USER)
                .Where(a => a.DOCTOR_ID == doctor.DOCTOR_ID && a.SCHEDULED_AT.Date == DateTime.Today)
                .OrderBy(a => a.SCHEDULED_AT)
                .ToListAsync();

            var stats = new
            {
                TodayAppointments = todayAppointments.Count,
                PendingAppointments = await _db.APPOINTMENTs
                    .CountAsync(a => a.DOCTOR_ID == doctor.DOCTOR_ID && a.STATUS == "Pending"),
                ConfirmedAppointments = await _db.APPOINTMENTs
                    .CountAsync(a => a.DOCTOR_ID == doctor.DOCTOR_ID && a.STATUS == "Confirmed"),
                TotalPatients = await _db.APPOINTMENTs
                    .Where(a => a.DOCTOR_ID == doctor.DOCTOR_ID)
                    .Select(a => a.PATIENT_ID)
                    .Distinct()
                    .CountAsync()
            };

            ViewBag.Stats = stats;
            ViewBag.TodayAppointments = todayAppointments;
            return View();
        }

        public async Task<IActionResult> Appointments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var doctor = await _db.DOCTORs.FirstOrDefaultAsync(d => d.USER_ID == userId);

            if (doctor == null) return NotFound();

            var appointments = await _db.APPOINTMENTs
                .Include(a => a.PATIENT).ThenInclude(p => p.USER)
                .Where(a => a.DOCTOR_ID == doctor.DOCTOR_ID)
                .OrderByDescending(a => a.SCHEDULED_AT)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, string status)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var doctor = await _db.DOCTORs.FirstOrDefaultAsync(d => d.USER_ID == userId);

            if (doctor == null) return Forbid();

            var appointment = await _db.APPOINTMENTs
                .FirstOrDefaultAsync(a => a.APPOINTMENT_ID == appointmentId && a.DOCTOR_ID == doctor.DOCTOR_ID);

            if (appointment != null)
            {
                appointment.STATUS = status;
                appointment.UPDATED_AT = DateTime.Now;
                await _db.SaveChangesAsync();

                TempData["Success"] = $"Appointment {status.ToLower()} successfully!";
            }
            else
            {
                TempData["Error"] = "Appointment not found or access denied.";
            }

            return RedirectToAction("Appointments");
        }

        [HttpPost]
        public async Task<IActionResult> AddConsultationNotes(int appointmentId, string notes)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var doctor = await _db.DOCTORs.FirstOrDefaultAsync(d => d.USER_ID == userId);

            if (doctor == null) return Forbid();

            var appointment = await _db.APPOINTMENTs
                .FirstOrDefaultAsync(a => a.APPOINTMENT_ID == appointmentId && a.DOCTOR_ID == doctor.DOCTOR_ID);

            if (appointment != null)
            {
                appointment.NOTES = notes;
                appointment.UPDATED_AT = DateTime.Now;
                await _db.SaveChangesAsync();

                TempData["Success"] = "Consultation notes added successfully!";
            }

            return RedirectToAction("Appointments");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var doctor = await _db.DOCTORs
                .Include(d => d.USER)
                .Include(d => d.SPECIALTY)
                .Include(d => d.CLINIC)
                .FirstOrDefaultAsync(d => d.USER_ID == userId);

            if (doctor == null) return NotFound();

            ViewBag.Specialties = await _db.SPECIALTies.ToListAsync();
            ViewBag.Clinics = await _db.CLINICs.ToListAsync();
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string phone, string bio, decimal fee, decimal specialtyId, decimal clinicId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var doctor = await _db.DOCTORs.Include(d => d.USER).FirstOrDefaultAsync(d => d.USER_ID == userId);

            if (doctor != null)
            {
                doctor.USER.FULL_NAME = fullName;
                doctor.USER.PHONE = phone;
                doctor.BIO = bio;
                doctor.FEE = fee;
                doctor.SPECIALTY_ID = specialtyId;
                doctor.CLINIC_ID = clinicId;

                await _db.SaveChangesAsync();

                // Update session with new name
                HttpContext.Session.SetString("UserName", fullName);

                TempData["Success"] = "Profile updated successfully!";
            }

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> Schedule()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var doctor = await _db.DOCTORs
                .Include(d => d.SCHEDULEs)
                .FirstOrDefaultAsync(d => d.USER_ID == userId);

            if (doctor == null) return NotFound();

            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(List<ScheduleViewModel> schedules)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var doctor = await _db.DOCTORs.FirstOrDefaultAsync(d => d.USER_ID == userId);

            if (doctor == null) return NotFound();

            // Remove existing schedules
            var existingSchedules = await _db.SCHEDULEs.Where(s => s.DOCTOR_ID == doctor.DOCTOR_ID).ToListAsync();
            _db.SCHEDULEs.RemoveRange(existingSchedules);

            // Add new schedules
            foreach (var schedule in schedules.Where(s => s.IsAvailable))
            {
                var newSchedule = new SCHEDULE
                {
                    DOCTOR_ID = doctor.DOCTOR_ID,
                    DAY_OF_WEEK = schedule.DayOfWeek,
                    START_TIME = schedule.StartTime,
                    END_TIME = schedule.EndTime,
                    SLOT_MINUTES = 30 // Default 30 minutes per slot
                };
                _db.SCHEDULEs.Add(newSchedule);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Schedule updated successfully!";
            return RedirectToAction("Schedule");
        }
    }

    // Helper class for schedule form
    public class ScheduleViewModel
    {
        public bool DayOfWeek { get; set; }
        public string DayName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}