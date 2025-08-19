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

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
