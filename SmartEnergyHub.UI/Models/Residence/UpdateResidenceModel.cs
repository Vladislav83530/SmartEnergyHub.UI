using System.ComponentModel.DataAnnotations;

namespace SmartEnergyHub.UI.Models.Residence
{
    public class UpdateResidenceModel
    {
        public int Id { get; set; }
        [Required] 
        public int RoomCount { get; set; }
        [Required]
        public double Area { get; set; }
        [Required]
        public string? Region { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Street { get; set; }
        [Required]
        public string? HouseNumber { get; set; }
        [Required]
        public string? FlatNumber { get; set; }
    }
}
