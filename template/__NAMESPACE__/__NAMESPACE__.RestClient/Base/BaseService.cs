using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace __NAMESPACE__.RestClient.Base
{
    public class BaseService
    {
        protected IMemoryCache Cache;
        protected ServiceOptions Options { get; }
        protected string BaseUrl { get; }
        protected Dictionary<string, string> Headers { get; }

        protected JsonSerializerSettings SerializerSettings = new();

        protected virtual string ApiController { get; } = null!;
        protected virtual bool RequiresAuthorization { get; set; } = true;

        public BaseService(IServiceProvider serviceProvider)
        {
            var resolver = serviceProvider.GetRequiredService<ServiceOptionsResolver>();
            var options = resolver?.GetOptions(serviceProvider).Result ?? new ServiceOptions();

            Options = options;
            BaseUrl = !options.BaseUrl.EndsWith('/') ? $"{options.BaseUrl}/" : options.BaseUrl;
            Headers = [];
            Cache = new MemoryCache(new MemoryCacheOptions());
            SerializerSettings.Converters.Add(new IsoDateTimeConverter());
        }

        protected async Task<TResponse>? Get<TResponse>(string resource = "")
            => await Request<TResponse>((client) => client.GetAsync($"{BaseUrl}{ApiController}{resource}"));

        protected async Task<HttpResponseMessage> Get(string resource = "")
            => await Request((client) => client.GetAsync($"{BaseUrl}{ApiController}{resource}"));

        protected async Task<TResponse>? Post<TRequest, TResponse>(string resource = "", TRequest body = default!)
            => await Request<TRequest, TResponse>((client, body) => client.PostAsJsonAsync($"{BaseUrl}{ApiController}{resource}", body), body);

        protected async Task<HttpResponseMessage> Post<TRequest>(string resource = "", TRequest body = default!)
            => await Request((client, body) => client.PostAsJsonAsync($"{BaseUrl}{ApiController}{resource}", body), body);

        protected async Task<TResponse>? Put<TRequest, TResponse>(string resource = "", TRequest? body = default)
            => await Request<TRequest, TResponse>((client, body) => client.PutAsJsonAsync($"{BaseUrl}{ApiController}{resource}", body), body);

        protected async Task<HttpResponseMessage> Put<TRequest>(string resource = "", TRequest? body = default)
            => await Request((client, body) => client.PutAsJsonAsync($"{BaseUrl}{ApiController}{resource}", body), body);

        protected async Task<TResponse>? Patch<TResponse>(string resource = "", HttpContent? body = default)
            => await Request<HttpContent, TResponse>((client, body) => client.PatchAsync($"{BaseUrl}{ApiController}{resource}", body), body);

        protected async Task<HttpResponseMessage> Patch(string resource = "", HttpContent? body = default)
            => await Request((client, body) => client.PatchAsync($"{BaseUrl}{ApiController}{resource}", body), body);

        protected async Task<TResponse>? Delete<TResponse>(string resource = "")
            => await Request<TResponse>((client) => client.DeleteAsync($"{BaseUrl}{ApiController}{resource}"));

        protected async Task<HttpResponseMessage> Delete(string resource = "")
            => await Request((client) => client.DeleteAsync($"{BaseUrl}{ApiController}{resource}"));

        private async Task<HttpResponseMessage> Request(Func<HttpClient, Task<HttpResponseMessage>> func)
        {
            var httpClient = await GetHttpClient(RequiresAuthorization);
            return await func.Invoke(httpClient);
        }

        private async Task<TResponse> Request<TResponse>(Func<HttpClient, Task<HttpResponseMessage>> func)
        {
            var httpClient = await GetHttpClient(RequiresAuthorization);
            var response = await func.Invoke(httpClient);
            return await Deserialize<TResponse>(response);
        }

        private async Task<HttpResponseMessage> Request<TRequest>(Func<HttpClient, TRequest, Task<HttpResponseMessage>> func, TRequest? body)
        {
            var httpClient = await GetHttpClient(RequiresAuthorization);
            return await func.Invoke(httpClient, body!);
        }

        private async Task<TResponse> Request<TRequest, TResponse>(Func<HttpClient, TRequest, Task<HttpResponseMessage>> func, TRequest? body)
        {
            var httpClient = await GetHttpClient(RequiresAuthorization);
            var response = await func.Invoke(httpClient, body!);
            return await Deserialize<TResponse>(response);
        }

        private async Task<TResponse> Deserialize<TResponse>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception(responseContent);

            return JsonConvert.DeserializeObject<TResponse>(responseContent!, SerializerSettings)!;
        }


        private async Task<HttpClient> GetHttpClient(bool requiresAuthorization = true)
        {
            if (requiresAuthorization) await Authenticate();

            var httpClient = new HttpClient();

            if (httpClient.DefaultRequestHeaders != null && Headers != null)
            {
                Headers.ToList().ForEach(h =>
                {
                    httpClient.DefaultRequestHeaders.Add(h.Key, h.Value);
                });
            }

            return httpClient;
        }

        private async Task Authenticate()
        {
            AddTokenHeader(string.Empty);

            await Task.CompletedTask;
        }

        private void AddTokenHeader(string token)
        {
            if (Headers.Any(x => x.Key == "Authorization"))
                Headers.Remove("Authorization");

            Headers.Add("Authorization", $"Bearer {token}");
        }
    }
}
