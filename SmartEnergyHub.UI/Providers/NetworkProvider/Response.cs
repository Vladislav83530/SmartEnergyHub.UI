using System.Net;

namespace SmartEnergyHub.UI.Providers.NetworkProvider
{
    public class Response<T>
    {
        public bool Successful { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
