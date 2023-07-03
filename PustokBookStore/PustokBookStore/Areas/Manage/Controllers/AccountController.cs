using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokBookStore.Areas.Manage.ViewModels;
using PustokBookStore.Entities;

namespace PustokBookStore.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        /*public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Member"));

            return Content("Created");
        }*/

        public async Task<IActionResult> CreateAdmin()
        {
            AppUser admin = new AppUser
            {
                FullName = "Super Admin",
                UserName = "SuperAdmin"
            };

            await _userManager.CreateAsync(admin, "Admin123");

            await _userManager.AddToRoleAsync(admin, "SuperAdmin");
            return Content("Yaradildi");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVM adminVM)
        {
            AppUser admin = await _userManager.FindByNameAsync(adminVM.UserName);

            if(admin == null)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(admin, adminVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }

            return RedirectToAction("index", "dashboard");
        }
    }
}
