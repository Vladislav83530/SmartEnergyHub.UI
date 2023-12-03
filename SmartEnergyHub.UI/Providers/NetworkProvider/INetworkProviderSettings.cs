namespace SmartEnergyHub.UI.Providers.NetworkProvider
{
    public interface INetworkProviderSettings
    {
        int TimeoutInSeconds { get; set; }
        int TokenLifeTime { get; set; }
        int MaxLogLength { get; set; }
        string BaseUrl { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        string AuthificationPath { get; set; }

    }
}
