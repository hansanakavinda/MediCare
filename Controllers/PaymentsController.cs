using MediCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _db;
        public PaymentsController(AppDbContext db) { _db = db; }

        // Show payment page for an appointment
        public async Task<IActionResult> Pay(int appointmentId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _db.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var appointment = await _db.APPOINTMENTs
                .Include(a => a.DOCTOR).ThenInclude(d => d.USER)
                .Include(a => a.DOCTOR).ThenInclude(d => d.SPECIALTY)
                .Include(a => a.DOCTOR).ThenInclude(d => d.CLINIC)
                .FirstOrDefaultAsync(a => a.APPOINTMENT_ID == appointmentId && a.PATIENT_ID == patient.PATIENT_ID);

            if (appointment == null) return NotFound("Appointment not found");

            // Check if payment already exists
            var existingPayment = await _db.PAYMENTs
                .FirstOrDefaultAsync(p => p.APPOINTMENT_ID == appointmentId);

            if (existingPayment != null && existingPayment.STATUS == "Completed")
            {
                TempData["Info"] = "This appointment has already been paid for.";
                return RedirectToAction("MyAppointments", "Appointments");
            }

            return View(appointment);
        }

        // Process payment
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int appointmentId, string paymentMethod, decimal amount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _db.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var appointment = await _db.APPOINTMENTs
                .Include(a => a.DOCTOR)
                .FirstOrDefaultAsync(a => a.APPOINTMENT_ID == appointmentId && a.PATIENT_ID == patient.PATIENT_ID);

            if (appointment == null) return NotFound("Appointment not found");

            // Check if payment already exists
            var existingPayment = await _db.PAYMENTs
                .FirstOrDefaultAsync(p => p.APPOINTMENT_ID == appointmentId);

            if (existingPayment != null && existingPayment.STATUS == "Completed")
            {
                TempData["Error"] = "Payment has already been processed for this appointment.";
                return RedirectToAction("Pay", new { appointmentId = appointmentId });
            }

            // Generate transaction reference
            string transactionRef = $"TXN_{DateTime.Now:yyyyMMddHHmmss}_{appointmentId}";

            // Create or update payment record
            PAYMENT payment;
            if (existingPayment != null)
            {
                // Update existing payment
                payment = existingPayment;
                payment.AMOUNT = amount;
                payment.METHOD = paymentMethod;
                payment.STATUS = "Completed";
                payment.PAID_AT = DateTime.Now;
                payment.TXN_REF = transactionRef;
            }
            else
            {
                // Create new payment
                payment = new PAYMENT
                {
                    APPOINTMENT_ID = appointmentId,
                    AMOUNT = amount,
                    METHOD = paymentMethod,
                    STATUS = "Completed",
                    PAID_AT = DateTime.Now,
                    TXN_REF = transactionRef
                };
                _db.PAYMENTs.Add(payment);
            }

            appointment.UPDATED_AT = DateTime.Now;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Payment processed successfully! Your appointment has been confirmed.";
            return RedirectToAction("PaymentSuccess", new { paymentId = payment.PAYMENT_ID });
        }

        // Payment success page
        public async Task<IActionResult> PaymentSuccess(int paymentId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _db.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var payment = await _db.PAYMENTs
                .Include(p => p.APPOINTMENT).ThenInclude(a => a.DOCTOR).ThenInclude(d => d.USER)
                .Include(p => p.APPOINTMENT).ThenInclude(a => a.DOCTOR).ThenInclude(d => d.SPECIALTY)
                .Include(p => p.APPOINTMENT).ThenInclude(a => a.DOCTOR).ThenInclude(d => d.CLINIC)
                .FirstOrDefaultAsync(p => p.PAYMENT_ID == paymentId && p.APPOINTMENT.PATIENT_ID == patient.PATIENT_ID);

            if (payment == null) return NotFound("Payment not found");

            return View(payment);
        }

        // View payment history
        public async Task<IActionResult> MyPayments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role != "Patient")
                return RedirectToAction("Login", "Account");

            var patient = await _db.PATIENTs.FirstOrDefaultAsync(p => p.USER_ID == userId);
            if (patient == null) return BadRequest("Not a patient");

            var payments = await _db.PAYMENTs
                .Include(p => p.APPOINTMENT).ThenInclude(a => a.DOCTOR).ThenInclude(d => d.USER)
                .Include(p => p.APPOINTMENT).ThenInclude(a => a.DOCTOR).ThenInclude(d => d.SPECIALTY)
                .Where(p => p.APPOINTMENT.PATIENT_ID == patient.PATIENT_ID)
                .OrderByDescending(p => p.PAID_AT)
                .ToListAsync();

            return View(payments);
        }
    }
}