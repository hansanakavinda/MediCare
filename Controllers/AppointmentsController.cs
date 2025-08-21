using MediCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _db;

        public AppointmentsController(AppDbContext db)
        {
            _db = db;
        }

        // Show booking form for a specific doctor
        public async Task<IActionResult> Book(int doctorId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var doctor = await _db.DOCTORs
                .Include(d => d.USER)
                .Include(d => d.SPECIALTY)
                .Include(d => d.CLINIC)
                .Include(d => d.SCHEDULEs)
                .FirstOrDefaultAsync(d => d.DOCTOR_ID == doctorId);

            if (doctor == null)
                return NotFound("Doctor not found");

            return View(doctor);
        }

        // Process the booking
        [HttpPost]
        public async Task<IActionResult> BookAppointment(int doctorId, DateTime appointmentDate, TimeSpan appointmentTime, string notes)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _db.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            // Combine date and time
            var scheduledDateTime = appointmentDate.Date.Add(appointmentTime);

            // Check if the appointment slot is already taken
            var existingAppointment = await _db.APPOINTMENTs
                .FirstOrDefaultAsync(a =>
                    a.DOCTOR_ID == doctorId &&
                    a.SCHEDULED_AT == scheduledDateTime &&
                    a.STATUS != "Cancelled");

            if (existingAppointment != null)
            {
                TempData["Error"] = "This appointment slot is already booked. Please select another time.";
                return RedirectToAction("Book", new { doctorId });
            }

            var appt = new APPOINTMENT
            {
                DOCTOR_ID = doctorId,
                PATIENT_ID = patient.PATIENT_ID,
                SCHEDULED_AT = scheduledDateTime,
                STATUS = "Pending",
                CREATED_AT = DateTime.Now,
                NOTES = notes
            };

            _db.APPOINTMENTs.Add(appt);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("MyAppointments");
        }

        // Show patient's appointments
        public async Task<IActionResult> MyAppointments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _db.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var appts = await _db.APPOINTMENTs
                .Include(a => a.DOCTOR).ThenInclude(d => d.USER)
                .Include(a => a.DOCTOR).ThenInclude(d => d.SPECIALTY)
                .Include(a => a.DOCTOR).ThenInclude(d => d.CLINIC)
                .Where(a => a.PATIENT_ID == patient.PATIENT_ID)
                .OrderByDescending(a => a.SCHEDULED_AT)
                .ToListAsync();

            return View(appts);
        }

        // Cancel appointment
        [HttpPost]
        public async Task<IActionResult> Cancel(int appointmentId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _db.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var appointment = await _db.APPOINTMENTs
                .FirstOrDefaultAsync(a => a.APPOINTMENT_ID == appointmentId && a.PATIENT_ID == patient.PATIENT_ID);

            if (appointment == null) return NotFound();

            if (appointment.SCHEDULED_AT <= DateTime.Now.AddHours(2))
            {
                TempData["Error"] = "Cannot cancel appointment less than 2 hours before scheduled time.";
                return RedirectToAction("MyAppointments");
            }

            appointment.STATUS = "Cancelled";
            appointment.UPDATED_AT = DateTime.Now;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Appointment cancelled successfully.";
            return RedirectToAction("MyAppointments");
        }
    }
}
