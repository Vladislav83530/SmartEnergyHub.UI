using SmartEnergyHub.DAL.Entities.Enums;

namespace SmartEnergyHub.UI.Models.Device
{
    public class FilterModel
    {
        public string? Name { get; set; }
        public string? DeviceType { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAutonomous { get; set; }
        public string? RoomType { get; set; }
    }
}
