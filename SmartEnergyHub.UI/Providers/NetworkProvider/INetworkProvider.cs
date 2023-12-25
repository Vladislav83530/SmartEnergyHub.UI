namespace SmartEnergyHub.UI.Providers.NetworkProvider
{
    public interface INetworkProvider
    {
        Task<Response<string>> GetAsync(INetworkProviderSettings settings, string path, IDictionary<string, string> headers = null);
        Task<Response<string>> PostAsync(INetworkProviderSettings settings, string path, object parameters = null, IDictionary<string, string> headers = null);
        Task<Response<string>> PutAsync(INetworkProviderSettings settings, string path, object parameters = null, IDictionary<string, string> headers = null);
        Task<Response<string>> DeleteAsync(INetworkProviderSettings settings, string path, IDictionary<string, string> headers = null);
        Task<Response<T>> GetAsync<T>(INetworkProviderSettings settings, string path, IDictionary<string, string> headers = null);
        Task<Response<T>> PostAsync<T>(INetworkProviderSettings settings, string path, object parameters = null, IDictionary<string, string> headers = null);
        Task<Response<T>> PutAsync<T>(INetworkProviderSettings settings, string path, object parameters = null, IDictionary<string, string> headers = null);
        Task<Response<T>> DeleteAsync<T>(INetworkProviderSettings settings, string path, IDictionary<string, string> headers = null);
    }
}
