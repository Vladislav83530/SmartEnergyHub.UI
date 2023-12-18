using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.UI.Models.Device;
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
                return Json(new { Error = "Not found error" });
            }

            string url = $"/api/residence/{customerId}";

            Response<ResidenceModel> response = await this._networkProvider.GetAsync<ResidenceModel>(this._apiSettings, url);

            if (response.Successful && response.Data != null)
            {
                return PartialView("_Residence", response.Data);
            }

            return Json(new { Error = "Bad request error" });
        }

        [HttpGet]
        public async Task<IActionResult> ConnectToMainHub(int residenceId)
        {
            if (residenceId <= 0)
            {
                return Json(new { Error = "Not found error" });
            }

            string url = $"/api/device/create-devices/{residenceId}";

            Response<string> response = await this._networkProvider.PostAsync<string>(this._apiSettings, url);

            if (response.Successful)
            {
                string getDevicesUrl = $"/api/device/get-devices/{residenceId}";

                Response<List<DeviceModel>> devicesResponse = await this._networkProvider.PostAsync<List<DeviceModel>>(this._apiSettings, getDevicesUrl, new FilterRequestModel());

                if (devicesResponse.Successful && devicesResponse.Data != null && devicesResponse.Data.Any())
                {
                    return PartialView("_Devices", devicesResponse.Data);
                }
            }

            return Json(new { Error = "Bad request error" });
        }
    }
}
