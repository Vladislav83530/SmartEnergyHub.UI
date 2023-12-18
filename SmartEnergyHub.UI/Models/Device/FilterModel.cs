using SmartEnergyHub.DAL.Entities.Enums;

namespace SmartEnergyHub.UI.Models.Device
{
    public class FilterModel
    {
        public string? Name { get; set; }
        public string? SerialNumber { get; set; }
        public DeviceType? DeviceType { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsConnected { get; set; }
        public DateTime? LastAccessTime { get; set; }
        public RoomType? RoomType { get; set; }
    }
}
