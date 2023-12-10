using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.UI.Models.Customer;
using SmartEnergyHub.UI.Providers.NetworkProvider;
using SmartEnergyHub.UI.Settings;

namespace SmartEnergyHub.UI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly INetworkProvider _networkProvider;
        private readonly ApiSettings _apiSettings;

        public CustomerController(
            INetworkProvider networkProvider,
            IOptions<ApiSettings> apiSettings
        )
        {
            this._networkProvider = networkProvider ?? throw new ArgumentNullException(nameof(networkProvider));
            this._apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("NotFound", "Error");
            }

            string url = $"/api/customer/{id}";

            Response<CustomerModel> response = await this._networkProvider.GetAsync<CustomerModel>(this._apiSettings, url);

            if (response.Successful && response.Data != null)
            {
                return View(response.Data);
            }

            return View("BadRequest", "Error");
        }
    }
}
