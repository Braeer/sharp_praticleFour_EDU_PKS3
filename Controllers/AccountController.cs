using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_web.Data;
using Practice_web.Models;

namespace Practice_web.Controllers
{
    public class AccountController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Login = model.Login!,
                    Password = model.Password!,
                    FullName = model.FullName!
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(RegisterModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

            if (user is not null)
                return RedirectToAction("Index", "Messages", new { userId = user.Id });

            ViewBag.ErrorMessage = "Неверный логин или пароль";
            return View(model);
        }
    }
}
