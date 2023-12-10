using Newtonsoft.Json;
using Serilog;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartEnergyHub.UI.Providers.NetworkProvider
{
    public class NetworkProvider : INetworkProvider
    {
        private const int DefaultMaxLogLength = 5000;
        private const int DefaultTimeoutInSeconds = 100;
        private const string UrlSeparator = "/";
        private const string ContentString = "application/json";

        private static readonly HttpClient Client = new HttpClient();
        private readonly ConcurrentDictionary<string, AuthenticationData> _authenticationDict = new ConcurrentDictionary<string, AuthenticationData>();

        static NetworkProvider()
        {
            Client.Timeout = Timeout.InfiniteTimeSpan;
        }

        public async Task<Response<string>> GetAsync(
            INetworkProviderSettings settings, 
            string path, 
            IDictionary<string, string> headers = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return await SendAsync(settings, path, headers, HttpMethod.Get);
        }

        public async Task<Response<string>> PostAsync(
            INetworkProviderSettings settings, 
            string path, 
            object parameters = null, 
            IDictionary<string, string> headers = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return await SendAsync(settings, path, headers, HttpMethod.Post, parameters);
        }

        public async Task<Response<string>> PutAsync(
            INetworkProviderSettings settings, 
            string path, 
            object parameters = null, 
            IDictionary<string, string> headers = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return await SendAsync(settings, path, headers, HttpMethod.Put, parameters);
        }

        public async Task<Response<string>> DeleteAsync(
            INetworkProviderSettings settings, 
            string path, 
            IDictionary<string, string> headers = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return await SendAsync(settings, path, headers, HttpMethod.Delete);
        }

        public async Task<Response<T>> GetAsync<T>(
            INetworkProviderSettings settings, 
            string path, 
            IDictionary<string, string> headers = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return await SendAsync<T>(settings, path, headers, HttpMethod.Get);
        }

        public async Task<Response<T>> PostAsync<T>(
            INetworkProviderSettings settings, 
            string path, 
            object parameters = null, 
            IDictionary<string, string> headers = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return await SendAsync<T>(settings, path, headers, HttpMethod.Post, parameters);
        }

        public async Task<Response<T>> PutAsync<T>(
            INetworkProviderSettings settings, 
            string path, 
            object parameters = null, 
            IDictionary<string, string> headers = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return await SendAsync<T>(settings, path, headers, HttpMethod.Put, parameters);
        }

        public async Task<Response<T>> DeleteAsync<T>(
            INetworkProviderSettings settings, 
            string path, 
            IDictionary<string, string> headers = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return await SendAsync<T>(settings, path, headers, HttpMethod.Delete);
        }

        #region Private methods

        private async Task<Response<string>> SendAsync(
            INetworkProviderSettings settings, 
            string path, 
            IDictionary<string, string> headers, 
            HttpMethod method,  
            object parameters = null)
        {
            Response<string> responseResult = new Response<string>();

            try
            {
                using (HttpResponseMessage response = await GetResponseAsync(settings, path, headers, method, parameters))
                {
                    string returnValue = await response.Content.ReadAsStringAsync();

                    FillResponse(responseResult, response, returnValue);

                    if (IsResponseSuccessfull(response))
                    {
                        responseResult.Data = returnValue;
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog("NetworkProvider:SendAsync", ex, settings, path, method, parameters);

                responseResult.Successful = false;
                responseResult.StatusCode = HttpStatusCode.InternalServerError;
                responseResult.Message = ex.Message;
            }

            return responseResult;
        }

        private async Task<Response<T>> SendAsync<T>(INetworkProviderSettings settings, string path, IDictionary<string, string> headers, HttpMethod method, object parameters = null)
        {
            var responseResult = new Response<T>();

            try
            {
                using (HttpResponseMessage response = await GetResponseAsync(settings, path, headers, method, parameters))
                {
                    string returnValue = await response.Content.ReadAsStringAsync();

                    FillResponse(responseResult, response, returnValue);

                    if (IsResponseSuccessfull(response))
                    {
                        responseResult.Data = JsonConvert.DeserializeObject<T>(returnValue);
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog("NetworkProvider:SendAsync<T>", ex, settings, path, method, parameters);

                responseResult.Successful = false;
                responseResult.StatusCode = HttpStatusCode.InternalServerError;
                responseResult.Message = ex.Message;
            }

            return responseResult;
        }

        private async Task<HttpResponseMessage> GetResponseAsync(
            INetworkProviderSettings settings,
            string path,
            IDictionary<string, string> headers,
            HttpMethod method,
            object parameters = null)
        {
            using (HttpRequestMessage requestMessage = await CreateRequest(settings, path, headers, method, parameters))
            {
                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                {
                    TimeSpan timeOut = TimeSpan.FromSeconds(settings.TimeoutInSeconds <= 0 ? DefaultTimeoutInSeconds : settings.TimeoutInSeconds);
                    cancellationTokenSource.CancelAfter(timeOut);
                    CancellationToken token = cancellationTokenSource.Token;

                    HttpResponseMessage response = await Client.SendAsync(requestMessage, token);

                    if (response.StatusCode == HttpStatusCode.Unauthorized
                        && this._authenticationDict.ContainsKey($"{settings.BaseUrl}{settings.Login}"))
                    {
                        this._authenticationDict.Remove($"{settings.BaseUrl}{settings.Login}", out AuthenticationData oldToken);

                        using HttpRequestMessage secondRequestMessage = await CreateRequest(settings, path, headers, method, parameters);
                        response = await Client.SendAsync(secondRequestMessage, token);
                    }

                    return response;
                }
            }
        }

        private async Task<HttpRequestMessage> CreateRequest(
            INetworkProviderSettings settings,
            string path,
            IDictionary<string, string> headers,
            HttpMethod method,
            object parameters)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, Combine(settings.BaseUrl, path))
            {
                Content = new StringContent(
                    parameters == null ? string.Empty : JsonConvert.SerializeObject(parameters),
                    Encoding.UTF8,
                    ContentString)
            };

            string token = await GetToken(settings);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if(headers?.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            return requestMessage;
        }

        private async Task<string> GetToken(INetworkProviderSettings settings)
        {
            string tokenKey = settings.BaseUrl + settings.Login;

            if (this._authenticationDict.TryGetValue(tokenKey, out AuthenticationData tokenData)
                && tokenData.Token != null
                && tokenData.TokenExpirationDate > DateTime.Now) 
            {
                return tokenData.Token;
            }

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, Combine(settings.BaseUrl, settings.AuthificationPath))
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            username = settings.Login,
                            password = settings.Password
                        }),
                            Encoding.UTF8,
                            "application/json"
                        )
                })
            {
                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource()) 
                {
                    TimeSpan timeOut = TimeSpan.FromSeconds(settings.TimeoutInSeconds <= 0 ? DefaultTimeoutInSeconds : settings.TimeoutInSeconds);
                    cancellationTokenSource.CancelAfter(timeOut);
                    CancellationToken ctoken = cancellationTokenSource.Token;

                    using (HttpResponseMessage response = await Client.SendAsync(requestMessage, ctoken))
                    {
                        if (response.StatusCode == HttpStatusCode.OK) 
                        {
                            string token = await response.Content.ReadAsStringAsync();

                            if (string.IsNullOrEmpty(token))
                            {
                                throw new Exception("Authentication route returned NULL JWT token");
                            }

                            AuthenticationData newToken = new AuthenticationData
                            {
                                Token = token,
                                TokenExpirationDate = DateTime.Now + new TimeSpan(0, settings.TokenLifeTime, 0)
                            };

                            if (this._authenticationDict.ContainsKey(tokenKey))
                            {
                                this._authenticationDict.Remove(tokenKey, out AuthenticationData oldToken);
                            }

                            if (this._authenticationDict.TryAdd(tokenKey, newToken))
                            {
                                return token;
                            }

                            throw new Exception($"Authentication::TryAdd error: tokenKey:{tokenKey}");
                        }

                        throw new Exception($"Authentication route returned {response.StatusCode} status");
                    }
                }
            }
        }

        private void FillResponse<T>(Response<T> responseData, HttpResponseMessage response, string returnValue)
        {
            responseData.Successful = response.StatusCode == HttpStatusCode.OK;
            responseData.StatusCode = response.StatusCode;

            if (IsResponseSuccessfull(response))
            {
                return;
            }

            responseData.Message = returnValue;
        }

        private bool IsResponseSuccessfull(HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.OK ||
                   response.StatusCode == HttpStatusCode.Created ||
                   response.StatusCode == HttpStatusCode.Accepted;
        }

        private void SaveLog(
         string methodName,
         Exception ex,
         INetworkProviderSettings settings,
         string path,
         HttpMethod httpMethod,
         object parameters = null)
        {
            string data = parameters == null ? string.Empty : JsonConvert.SerializeObject(parameters);
            int maxLogLength = settings.MaxLogLength == 0 ? DefaultMaxLogLength : settings.MaxLogLength;

            if (data.Length > maxLogLength)
            {
                data = $"Data size exceeds the set limit of {maxLogLength} symbols";
            }

            Log.Warning(
                $@"{methodName}: 
                    Error {ex.Message};
                    Url: {Combine(settings.BaseUrl, path)};
                    Data: {data};
                    contentType: {ContentString}
                    HttpMethod: {httpMethod}
                    Trace: {ex.StackTrace}");
        }

        private string Combine(string baseUrl, string path)
        {
            if (baseUrl.EndsWith(UrlSeparator) && path.StartsWith(UrlSeparator))
            {
                return string.Concat(baseUrl, path.AsSpan(1));
            }

            if (baseUrl.EndsWith(UrlSeparator) || path.StartsWith(UrlSeparator))
            {
                return baseUrl + path;
            }

            return $"{baseUrl}/{path}";
        }

        #endregion
    }
}
