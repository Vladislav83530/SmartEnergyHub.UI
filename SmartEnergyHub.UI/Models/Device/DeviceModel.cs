using SmartEnergyHub.DAL.Entities.Enums;

namespace SmartEnergyHub.UI.Models.Device
{
    public class DeviceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string AccessToken { get; set; }
        public string MACAddress { get; set; }
        public DeviceType DeviceType { get; set; }
        public bool IsActive { get; set; }
        public bool IsConnected { get; set; }
        public bool IsAutonomous { get; set; }
        public DateTime LastAccessTime { get; set; }
        public RoomType RoomType { get; set; }
    }
}
