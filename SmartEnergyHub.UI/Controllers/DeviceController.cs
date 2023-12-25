using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.DAL.Entities;
using SmartEnergyHub.DAL.Entities.Enums;
using SmartEnergyHub.UI.Models.Device;
using SmartEnergyHub.UI.Providers.NetworkProvider;
using SmartEnergyHub.UI.Settings;
using System.Collections.Generic;

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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeviceOnOff(int deviceId, bool isActive)
        {
            if (deviceId <= 0) 
            {
                return Json(new { Error = "Not found" });
            }

            string url = $"/api/device/update-active-status/{deviceId}/{isActive}";

            Response<string> response = await this._networkProvider.PutAsync<string>(this._apiSettings, url);

            if (response.Successful)
            {
                return Ok();
            }

            return Json(new { Error = "Bad request error" });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage(int deviceId)
        {
            if (deviceId <= 0)
            {
                return RedirectToAction("NotFound", "Error");
            }

            string url = $"/api/device/{deviceId}";
            Response<DeviceModel> response = await this._networkProvider.GetAsync<DeviceModel>(this._apiSettings, url);

            if (response.Successful && response.Data != null)
            {
                return View(response.Data);
            }

            return RedirectToAction("BadRequest", "Error");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSessions(int deviceId, int period)
        {
            if (deviceId <= 0)
            {
                 return Json(new { Error = "Not found" });
            }

            string url = $"/api/device/get-sessions/{deviceId}/{period}";
            Response<List<ActivitySession>> response = await this._networkProvider.GetAsync<List<ActivitySession>> (this._apiSettings, url);

            if (response.Successful && response.Data.Any())
            {
                return Ok(response.Data);
            }

            return Json(new { Error = "Bad request error" }); ;
        }
    }
}
