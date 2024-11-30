using ContactsManager.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountsController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDto registerDto)
        {
            return RedirectToAction(nameof(PersonsController.Index), nameof(PersonsController));
        }
    }
}
