using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.DAL.Entities;
using SmartEnergyHub.UI.Models.Customer;
using SmartEnergyHub.UI.Models.Device;
using SmartEnergyHub.UI.Models.Residence;
using SmartEnergyHub.UI.Providers.NetworkProvider;
using SmartEnergyHub.UI.Settings;
using System.Security.Claims;

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
        [Authorize]
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
        [Authorize]
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
                    string updateStatusUrl = $"/api/residence/set-connected-status/{residenceId}";
                    Response<string> updateStatusResponse = await this._networkProvider.PutAsync<string>(this._apiSettings, updateStatusUrl);

                    if (!updateStatusResponse.Successful)
                    {
                        return Json(new { Error = "Bad request" });
                    }

                    return PartialView("_Devices", devicesResponse.Data);
                }
            }

            return Json(new { Error = "Bad request error" });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(string residenceId)
        {
            if (string.IsNullOrEmpty(residenceId))
            {
                return Json(new { Error = "Not found error" });
            }

            string url = $"/api/residence/residence-by-id/{residenceId}";

            Response<ResidenceModel> response = await this._networkProvider.GetAsync<ResidenceModel>(this._apiSettings, url);

            if (response.Successful && response.Data != null)
            {
                UpdateResidenceModel updateModel = new UpdateResidenceModel
                {
                    Id = response.Data.Id,
                    Area = response.Data.Area,
                    Region = response.Data.Region,
                    City = response.Data.City,
                    Street = response.Data.Street,
                    HouseNumber = response.Data.HouseNumber,
                    FlatNumber = response.Data.FlatNumber
                };


                return View("UpdateResidence", updateModel);
            }

            return Json(new { Error = "Bad request error" });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateResidence(UpdateResidenceModel model)
        {
            if (model == null)
            {
                RedirectToAction("NotFound", "Error");
            }

            string url = $"/api/residence/update-residence";
            Response<string> response = await this._networkProvider.PutAsync<string>(this._apiSettings, url, model);


            if (response.Successful)
            {
                return RedirectToAction("Index", "Device");
            }

            return RedirectToAction("BadRequest", "Error");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ClearResidenceInfo(int residenceId)
        {
            if (residenceId <= 0)
            {
                RedirectToAction("NotFound", "Error");
            }

            string customerId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string url = $"/api/residence/{residenceId}/{customerId}";
            Response<string> response = await this._networkProvider.DeleteAsync<string>(this._apiSettings, url);

            if(response.Successful)
            {
                return RedirectToAction("Index", "Device");
            }

            return RedirectToAction("BadRequest", "Error");
        }
    } 
}
