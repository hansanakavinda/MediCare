using MediCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _db;
        public AppointmentsController(AppDbContext db) { _db = db; }

        [HttpPost]
        public async Task<IActionResult> Book(int doctorId, DateTime dateTime)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _db.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var appt = new APPOINTMENT
            {
                DOCTOR_ID = doctorId,
                PATIENT_ID = patient.PATIENT_ID,
                SCHEDULED_AT = dateTime,
                STATUS = "Pending"
            };

            _db.APPOINTMENTs.Add(appt);
            await _db.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

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
                .Where(a => a.PATIENT_ID == patient.PATIENT_ID)
                .OrderByDescending(a => a.SCHEDULED_AT)
                .ToListAsync();

            return View(appts);
        }
    }
}
