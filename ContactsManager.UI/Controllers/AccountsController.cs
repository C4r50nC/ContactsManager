using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.Dto;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Ui.Controllers
{
    // [AllowAnonymous] // Removed to resolve conficts with NotAuthenticated policy in Authorize attribute
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
        [Authorize("NotAuthenticated")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthenticated")]
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
                if (await _roleManager.FindByNameAsync(nameof(UserTypeOptions.Admin)) == null)
                {
                    ApplicationRole adminRole = new()
                    {
                        Name = nameof(UserTypeOptions.Admin),
                    };
                    await _roleManager.CreateAsync(adminRole);
                }

                await _userManager.AddToRoleAsync(user, nameof(UserTypeOptions.Admin));
            }
            else
            {
                if (await _roleManager.FindByNameAsync(nameof(UserTypeOptions.User)) == null)
                {
                    ApplicationRole userRole = new()
                    {
                        Name = nameof(UserTypeOptions.User),
                    };
                    await _roleManager.CreateAsync(userRole);
                }

                await _userManager.AddToRoleAsync(user, nameof(UserTypeOptions.User));
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [HttpGet]
        [Authorize("NotAuthenticated")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthenticated")]
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
            bool isUserAdmin = await _userManager.IsInRoleAsync(user, nameof(UserTypeOptions.Admin));
            if (isUserAdmin)
            {
                return RedirectToAction(nameof(Areas.Admin.Controllers.HomeController.Index), "Home", new { area = "Admin" });
            }

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [Authorize("NotAuthenticated")]
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
