using System.ComponentModel.DataAnnotations;

namespace SmartEnergyHub.UI.Models.AuthCustomer
{
    public class CustomerLoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
