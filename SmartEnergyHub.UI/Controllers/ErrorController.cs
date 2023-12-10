using Microsoft.AspNetCore.Mvc;

namespace SmartEnergyHub.UI.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult BadRequest()
        {
            return View();
        }
    }
}
