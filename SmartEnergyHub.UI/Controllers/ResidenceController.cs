using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.UI.Models.Residence;
using SmartEnergyHub.UI.Providers.NetworkProvider;
using SmartEnergyHub.UI.Settings;

namespace SmartEnergyHub.UI.Controllers
{
    public class ResidenceController : Controller
    {
        private readonly INetworkProvider _networkProvider;
        private readonly ApiSettings _apiSettings;

        public ResidenceController(
            INetworkProvider networkProvider,
            IOptions<ApiSettings> apiSettings
        )
        {
            this._networkProvider = networkProvider ?? throw new ArgumentNullException(nameof(networkProvider));
            this._apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        [HttpGet]
        public async Task<IActionResult> GetResidence(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("NotFound", "Error");
            }

            string url = $"/api/residence/{customerId}";

            Response<ResidenceModel> response = await this._networkProvider.GetAsync<ResidenceModel>(this._apiSettings, url);

            if (response.Successful && response.Data != null)
            {
                return PartialView("_Residence", response.Data);
            }

            return RedirectToAction("BadRequest", "Error");
        }
    }
}
