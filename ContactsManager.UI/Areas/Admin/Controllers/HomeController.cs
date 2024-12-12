using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Ui.Areas.Admin.Controllers
{
    [Area("Admin")]
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
