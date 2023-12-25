using SmartEnergyHub.UI.Providers.NetworkProvider;

namespace SmartEnergyHub.UI.Settings
{
    public class ApiSettings : INetworkProviderSettings
    {
        public int TimeoutInSeconds { get; set; }
        public int TokenLifeTime { get; set; }
        public int MaxLogLength { get; set; }
        public string BaseUrl { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string AuthificationPath { get; set; }
    }
}
