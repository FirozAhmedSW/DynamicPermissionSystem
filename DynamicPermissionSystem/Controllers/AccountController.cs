using DynamicPermissionSystem.Helpers;
using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicPermissionSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly JwtTokenGenerator _tokenService;
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _tokenService = new JwtTokenGenerator(config);
        }


        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
            if (user == null) { ViewBag.Error = "Invalid credentials"; return View(); }

            //if (username == "admin" && password == "123")
            //{
            //    var token = _tokenService.GenerateToken(username, "Admin");
            //    return Json(new { Token = token });
            //}

            //json formate a user data ke session a rakha hossa
            var userJson = System.Text.Json.JsonSerializer.Serialize(user);
            HttpContext.Session.SetString("User", userJson);


            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetInt32("RoleId", user.RoleId);
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
