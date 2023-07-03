using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokBookStore.Entities;
using PustokBookStore.ViewModels;

namespace PustokBookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterVM memberVM)
        {
            if(!ModelState.IsValid) return View();

            AppUser user = new AppUser
            {
                FullName = memberVM.FullName,
                Email = memberVM.Email,
                UserName = memberVM.UserName,
                PhoneNumber = memberVM.Phone,
            };

            var result = await _userManager.CreateAsync(user, memberVM.Password);
            if(!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginVM memberVM)
        {
            if(!ModelState.IsValid) return View();

            AppUser member = await _userManager.FindByNameAsync(memberVM.UserName);

            if (member == null)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(member, memberVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);
            ProfileVM vm = new ProfileVM()
            {
                Member = new MemberUpdateVM
                {
                    FullName = member.FullName,
                    UserName = member.UserName,
                    Email = member.Email,
                    Phone = member.PhoneNumber,
                }
            };

            return View(vm);
        }

        public async Task<IActionResult> MemberUpdate(MemberUpdateVM memberVM)
        {
            if (!ModelState.IsValid)
            {
                ProfileVM vm = new ProfileVM { Member = memberVM };
                return View("profile",vm);
            }

            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);

            member.FullName = memberVM.FullName;
            member.UserName = memberVM.UserName;
            member.Email = memberVM.Email;
            member.PhoneNumber = memberVM.Phone;

            var result = await _userManager.UpdateAsync(member);

            if(!result.Succeeded)
            {
                foreach (var item in result.Errors) ModelState.AddModelError("", item.Description);
                ProfileVM vm = new ProfileVM { Member = memberVM };
                return View("profile", vm);
            }

            await _signInManager.SignInAsync(member, false);

            return RedirectToAction("profile");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
    }
}
