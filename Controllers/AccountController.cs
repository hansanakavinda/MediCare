using MediCare.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) { _db = db; }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _db.USERs
                .FirstOrDefaultAsync(u => u.EMAIL == email && u.PWD == password && u.IS_ACTIVE == "Y");
            if (user == null)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            // Save session
            HttpContext.Session.SetInt32("UserId", (int)user.USER_ID);
            HttpContext.Session.SetString("Role", user.ROLE);
            HttpContext.Session.SetString("UserName", user.FULL_NAME);

            // Redirect based on role
            return user.ROLE switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Doctor" => RedirectToAction("Dashboard", "Doctor"),
                "Patient" => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string fullName, string phone, DateTime? dob, string gender, string address)
        {
            // Check if email already exists
            if (await _db.USERs.AnyAsync(u => u.EMAIL == email))
            {
                ViewBag.Error = "Email already exists";
                return View();
            }

            // Create new user
            var user = new USER
            {
                EMAIL = email,
                PWD = password, // In production, hash this password
                FULL_NAME = fullName,
                PHONE = phone,
                ROLE = "Patient",
                IS_ACTIVE = "Y",
                CREATED_AT = DateTime.Now
            };

            _db.USERs.Add(user);
            await _db.SaveChangesAsync();

            // Create patient record
            var patient = new PATIENT
            {
                USER_ID = user.USER_ID,
                DOB = dob,
                GENDER = gender,
                ADDRESS = address
            };

            _db.PATIENTs.Add(patient);
            await _db.SaveChangesAsync();

            // Auto login after registration
            HttpContext.Session.SetInt32("UserId", (int)user.USER_ID);
            HttpContext.Session.SetString("Role", user.ROLE);
            HttpContext.Session.SetString("UserName", user.FULL_NAME);

            TempData["Success"] = "Registration successful! Welcome to MediCare.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}