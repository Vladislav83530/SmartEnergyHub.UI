using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.UI.Models.Device;
using SmartEnergyHub.UI.Providers.NetworkProvider;
using SmartEnergyHub.UI.Settings;

namespace SmartEnergyHub.UI.Controllers
{
    public class DeviceController : Controller
    {
        private readonly INetworkProvider _networkProvider;
        private readonly ApiSettings _apiSettings;

        public DeviceController(
            INetworkProvider networkProvider,
            IOptions<ApiSettings> apiSettings
        )
        {
            this._networkProvider = networkProvider ?? throw new ArgumentNullException(nameof(networkProvider));
            this._apiSettings = apiSettings.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> FilterDevices(int residenceId, FilterRequestModel filterModel)
        {
            string getDevicesUrl = $"/api/device/get-devices/{residenceId}";

            Response<List<DeviceModel>> devicesResponse = await this._networkProvider.PostAsync<List<DeviceModel>>(this._apiSettings, getDevicesUrl, filterModel);

            if (devicesResponse.Successful && devicesResponse.Data != null && devicesResponse.Data.Any())
            {
                return PartialView("~/Views/Residence/_Devices.cshtml", devicesResponse.Data);
            }

            return Json(new { Error = "Bad request error" });
        }
    }
}
