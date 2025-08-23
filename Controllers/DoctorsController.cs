using MediCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly AppDbContext _db;
        public DoctorsController(AppDbContext db) { _db = db; }

        // List/search doctors
        public async Task<IActionResult> Index(string? q, int? specialtyId, int? clinicId, string? dayOfWeek)
        {
            var doctors = _db.DOCTORs
                .Include(d => d.SPECIALTY)
                .Include(d => d.CLINIC)
                .Include(d => d.USER)
                .Include(d => d.SCHEDULEs)
                .AsQueryable();

            // Search by name
            if (!string.IsNullOrEmpty(q))
                doctors = doctors.Where(d => d.USER.FULL_NAME.Contains(q));

            // Filter by specialty
            if (specialtyId.HasValue)
                doctors = doctors.Where(d => d.SPECIALTY_ID == specialtyId);

            // Filter by clinic/location
            if (clinicId.HasValue)
                doctors = doctors.Where(d => d.CLINIC_ID == clinicId);

            // Filter by availability (day of week)
            if (!string.IsNullOrEmpty(dayOfWeek))
            {
                doctors = doctors.Where(d => d.SCHEDULEs.Any(s => s.DAY_OF_WEEK.Equals(dayOfWeek)));
            }

            // Load dropdown data
            ViewBag.Specialties = await _db.SPECIALTies.ToListAsync();
            ViewBag.Clinics = await _db.CLINICs.ToListAsync();

            // Pass current filter values to maintain state
            ViewBag.CurrentQuery = q;
            ViewBag.CurrentSpecialtyId = specialtyId;
            ViewBag.CurrentClinicId = clinicId;
            ViewBag.CurrentDayOfWeek = dayOfWeek;

            return View(await doctors.ToListAsync());
        }

        // Doctor details + schedule
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _db.DOCTORs
                .Include(d => d.SPECIALTY)
                .Include(d => d.CLINIC)
                .Include(d => d.USER)
                .Include(d => d.SCHEDULEs)
                .FirstOrDefaultAsync(d => d.DOCTOR_ID == id);

            if (doctor == null) return NotFound();
            return View(doctor);
        }

    }
}