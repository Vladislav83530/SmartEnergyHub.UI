using Microsoft.AspNetCore.Mvc;

namespace SmartEnergyHub.UI.Controllers
{
    public class DeviceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
