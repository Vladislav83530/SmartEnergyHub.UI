using System.ComponentModel.DataAnnotations;

namespace SmartEnergyHub.UI.Models.Customer
{
    public class UpdateCustomerModel
    {
        public string CustomerId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^(?:\+38)?(0[5-9][0-9]\d{7})$", ErrorMessage = "Incorrect phone number")]
        public string PhoneNumber { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? FlatNumber { get; set; }
    }
}
