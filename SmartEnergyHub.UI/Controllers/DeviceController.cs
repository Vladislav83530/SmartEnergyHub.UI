using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.DAL.Entities.Enums;
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
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FilterDevices(int residenceId, int pageNumber, string name, string deviceTypes, string roomTypes, bool? isActive = null, bool? isAutonomous = null, int pageSize = 5)
        {
            string getDevicesUrl = $"/api/device/get-devices/{residenceId}";

            FilterRequestModel filterModel = new FilterRequestModel
            {
                FilterModel = new FilterModel
                {
                    Name = name,
                    DeviceType = deviceTypes,
                    RoomType = roomTypes,
                    IsActive = isActive,
                    IsAutonomous = isAutonomous,
                },
                PaginationModel = new PaginationModel 
                { 
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }
            };

            Response<List<DeviceModel>> devicesResponse = await this._networkProvider.PostAsync<List<DeviceModel>>(this._apiSettings, getDevicesUrl, filterModel);

            if (devicesResponse.Successful && devicesResponse.Data != null)
            {
                return PartialView("~/Views/Residence/_Devices.cshtml", devicesResponse.Data);
            }

            return Json(new { Error = "Bad request error" });
        }
    }
}
