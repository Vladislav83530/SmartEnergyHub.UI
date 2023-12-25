namespace SmartEnergyHub.UI.Models.Device
{
    public class FilterRequestModel
    {
        public FilterModel FilterModel { get; set; } = new FilterModel();
        public PaginationModel PaginationModel { get; set; } = new PaginationModel();
    }
}
