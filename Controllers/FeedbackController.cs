using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediCare.Models;
using System.Security.Claims;

namespace MediCare.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Feedback/Create/5 (appointmentId)
        [HttpGet]
        public async Task<IActionResult> Create(int appointmentId)
        {
            // Get current patient ID from claims
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _context.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var patientId = patient.PATIENT_ID;

            // Get the appointment with doctor details
            var appointment = await _context.APPOINTMENTs
                .Include(a => a.DOCTOR)
                .ThenInclude(d => d.USER)
                .Include(a => a.DOCTOR.SPECIALTY)
                .FirstOrDefaultAsync(a => a.APPOINTMENT_ID == appointmentId
                                       && a.PATIENT_ID == patientId
                                       && a.STATUS == "Completed");

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found or not eligible for feedback.";
                return RedirectToAction("MyPayments", "Payments");
            }

            // Check if feedback already exists
            var existingFeedback = await _context.FEEDBACKs
                .FirstOrDefaultAsync(f => f.PATIENT_ID == patientId
                                       && f.DOCTOR_ID == appointment.DOCTOR_ID
                                       && f.APPOINTMENT_ID == appointment.APPOINTMENT_ID);

            if (existingFeedback != null)
            {
                TempData["InfoMessage"] = "You have already provided feedback for this doctor.";
                return RedirectToAction("MyAppointments", "Appointments");
            }

            ViewBag.Appointment = appointment;
            return View();
        }

        // POST: Feedback/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int appointmentId, int rating, string? message)
        {
            // Get current patient ID from claims
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _context.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var patientId = patient.PATIENT_ID;

            // Get the appointment
            var appointment = await _context.APPOINTMENTs
                .FirstOrDefaultAsync(a => a.APPOINTMENT_ID == appointmentId
                                       && a.PATIENT_ID == patientId
                                       && a.STATUS == "Completed");

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found or not eligible for feedback.";
                return RedirectToAction("MyPayments", "Payments");
            }


            // Create new feedback
            int rating1 = rating;

            var feedback = new FEEDBACK
            {
                PATIENT_ID = (decimal)patientId,
                DOCTOR_ID = appointment.DOCTOR_ID,
                APPOINTMENT_ID = appointment.APPOINTMENT_ID,
                MSG = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
                CREATED_AT = DateTime.Now
            };

            try
            {
                _context.FEEDBACKs.Add(feedback);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you for your feedback! It helps us improve our services.";
                return RedirectToAction("MyAppointments", "Appointments");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while saving your feedback. Please try again.";
                return RedirectToAction("Create", new { appointmentId });
            }
        }

        // GET: Check if feedback exists for a doctor
        [HttpGet]
        public async Task<IActionResult> CheckFeedbackExists(int doctorId)
        {
            var patientIdClaim = User.FindFirst("PatientId")?.Value;
            if (string.IsNullOrEmpty(patientIdClaim) || !decimal.TryParse(patientIdClaim, out decimal patientId))
            {
                return Json(new { exists = false });
            }

            var exists = await _context.FEEDBACKs
                .AnyAsync(f => f.PATIENT_ID == patientId && f.DOCTOR_ID == doctorId);

            return Json(new { exists });
        }

        // GET: Get patient's feedback for a doctor
        [HttpGet]
        public async Task<IActionResult> GetFeedback(int doctorId)
        {
            var patientIdClaim = User.FindFirst("PatientId")?.Value;
            if (string.IsNullOrEmpty(patientIdClaim) || !decimal.TryParse(patientIdClaim, out decimal patientId))
            {
                return Json(null);
            }

            var feedback = await _context.FEEDBACKs
                .FirstOrDefaultAsync(f => f.PATIENT_ID == patientId && f.DOCTOR_ID == doctorId);

            if (feedback == null)
                return Json(null);

            return Json(new
            {
                message = feedback.MSG,
                createdAt = feedback.CREATED_AT.ToString("MMM dd, yyyy")
            });
        }
    }
}