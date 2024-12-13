using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Ui.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(UserTypeOptions.Admin))]
    public class HomeController : Controller
    {
        // [Route("Admin/[controller]/[action]")]
        // Both attribute routing and conventional routing can be applied
        public IActionResult Index()
        {
            return View();
        }
    }
}
