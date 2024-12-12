using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.Dto;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Ui.Controllers
{
    [AllowAnonymous]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage);
                return View(registerDto);
            }

            ApplicationUser user = new()
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.Phone,
                UserName = registerDto.Email,
                PersonName = registerDto.PersonName,
            };

            IdentityResult identityResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return View(registerDto);
            }

            if (registerDto.UserType == UserTypeOptions.Admin)
            {
                if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) == null)
                {
                    ApplicationRole adminRole = new()
                    {
                        Name = UserTypeOptions.Admin.ToString(),
                    };
                    await _roleManager.CreateAsync(adminRole);
                }

                await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
            }
            else
            {
                if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) == null)
                {
                    ApplicationRole userRole = new()
                    {
                        Name = UserTypeOptions.User.ToString(),
                    };
                    await _roleManager.CreateAsync(userRole);
                }

                await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage);
                return View(loginDto);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("Login", "Invalid email or password");
                return View(signInResult);
            }

            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return LocalRedirect(returnUrl);
            }
            bool isUserAdmin = await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString());
            if (isUserAdmin)
            {
                return RedirectToAction(nameof(Areas.Admin.Controllers.HomeController.Index), "Home", new { area = "Admin" });
            }

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        public async Task<IActionResult> IsRegisteredEmail(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }

            return Json(false);
        }
    }
}
