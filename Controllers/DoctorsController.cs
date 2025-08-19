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
        public async Task<IActionResult> Index(string? q, int? specialtyId)
        {
            var doctors = _db.DOCTORs
            .Include(d => d.SPECIALTY)
            .Include(d => d.CLINIC)
            .Include(d => d.USER)   
            .AsQueryable();


            if (!string.IsNullOrEmpty(q))
                doctors = doctors.Where(d => d.USER.FULL_NAME.Contains(q));

            if (specialtyId.HasValue)
                doctors = doctors.Where(d => d.SPECIALTY_ID == specialtyId);

            ViewBag.Specialties = await _db.SPECIALTies.ToListAsync();
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
