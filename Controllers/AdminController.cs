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
    }
}