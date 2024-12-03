using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Ui.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }
    }
}
