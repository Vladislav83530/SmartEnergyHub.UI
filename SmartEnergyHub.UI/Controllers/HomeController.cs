using Microsoft.AspNetCore.Mvc;

namespace SmartEnergyHub.UI.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}