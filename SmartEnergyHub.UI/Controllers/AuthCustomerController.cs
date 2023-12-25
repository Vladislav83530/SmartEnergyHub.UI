using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartEnergyHub.DAL.Entities;
using SmartEnergyHub.UI.Models.AuthCustomer;
using SmartEnergyHub.UI.Providers.NetworkProvider;
using SmartEnergyHub.UI.Settings;

namespace SmartEnergyHub.API.Controllers
{
    public class AuthCustomerController : Controller
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly INetworkProvider _networkProvider;
        private readonly ApiSettings _apiSettings;

        public AuthCustomerController(
            UserManager<Customer> userManager, 
            SignInManager<Customer> signInManager,
            INetworkProvider networkProvider,
            IOptions<ApiSettings> apiSettings)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _networkProvider = networkProvider ?? throw new ArgumentNullException(nameof(_networkProvider));
            _apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CustomerRegisterModel model)
        {
            if (ModelState.IsValid)
            {             
                Customer customer = new Customer
                {
                    FistName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToLower().Trim(),
                    UserName = model.Email.ToLower().Trim(),
                    PhoneNumber = model.PhoneNumber,
                };

                IdentityResult result = await _userManager.CreateAsync(customer, model.Password);
                string customerId = await _userManager.GetUserIdAsync(customer);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

                string url = $"/api/customer/add-residence/{customerId}";
                Response<string> response = await this._networkProvider.PostAsync<string>(this._apiSettings, url);

                if (!response.Successful)
                {
                    string deleteUrl = $"/api/customer/delete/{customerId}";
                    _ = await this._networkProvider.DeleteAsync<string>(this._apiSettings, deleteUrl);

                    return RedirectToAction("Register", "Customer");
                }

                await _signInManager.SignInAsync(customer, false);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(CustomerLoginModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("Login error", "Incorrect login or password");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
