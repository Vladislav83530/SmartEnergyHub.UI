using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.DAL.Entities;
using SmartEnergyHub.UI.Models.Customer;
using SmartEnergyHub.UI.Providers.NetworkProvider;
using SmartEnergyHub.UI.Settings;

namespace SmartEnergyHub.UI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly INetworkProvider _networkProvider;
        private readonly SignInManager<Customer> _signInManager;
        private readonly ApiSettings _apiSettings;

        public CustomerController(
            INetworkProvider networkProvider,
            SignInManager<Customer> signInManager,
            IOptions<ApiSettings> apiSettings
        )
        {
            this._networkProvider = networkProvider ?? throw new ArgumentNullException(nameof(networkProvider));
            this._signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this._apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            Response<CustomerModel> response = await GetCustomerModelAsync(id);

            if (response.Successful && response.Data != null)
            {
                return View(response.Data);
            }

            return RedirectToAction("BadRequest", "Error");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            Response<CustomerModel> response = await GetCustomerModelAsync(id);

            if (response.Successful && response.Data != null)
            {
                UpdateCustomerModel updateModel = new UpdateCustomerModel
                {
                    CustomerId = response.Data.CustomerId,
                    FirstName = response.Data.FirstName,
                    LastName = response.Data.LastName,
                    PhoneNumber = response.Data.PhoneNumber,
                    Region = response.Data.Region,
                    City = response.Data.City,
                    Street = response.Data.Street,
                    HouseNumber = response.Data.HouseNumber,
                    FlatNumber = response.Data.FlatNumber
                };


                return PartialView("_Update", updateModel);
            }

            return RedirectToAction("BadRequest", "Error");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(UpdateCustomerModel model)
        {
            if (model == null)
            {
                return RedirectToAction("BadRequest", "Error");
            }

            string url = $"/api/customer/update";

            Response<string> response = await this._networkProvider.PostAsync<string>(this._apiSettings, url, model);

            if (response.Successful)
            {
                return RedirectToAction("Index", "Customer", new { id = model.CustomerId });
            }

            return RedirectToAction("BadRequest", "Error");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("NotFound", "Error");
            }

            await _signInManager.SignOutAsync();

            string url = $"/api/customer/delete/{id}";
            Response<string> response = await this._networkProvider.DeleteAsync<string>(this._apiSettings, url);          

            if (response.Successful)
            {        
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("BadRequest", "Error");
        }

        private async Task<Response<CustomerModel>> GetCustomerModelAsync(string id)
        {
            string url = $"/api/customer/{id}";

            Response<CustomerModel> response = await this._networkProvider.GetAsync<CustomerModel>(this._apiSettings, url);

            return response;
        }
    }
}
