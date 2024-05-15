using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using MagicVilla_Utility;
using Newtonsoft.Json;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse _responseModel { get; set; } = new();

        public IHttpClientFactory _httpClient { get; set; }

        public BaseService(IHttpClientFactory httpClientFactory) 
        {
            _httpClient = httpClientFactory;
        }



        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            string? APIContent;
            try
            {
                HttpRequestMessage message = new();
                message.Headers.Add(Strings.Accept, Strings.AppJson);
                message.RequestUri = apiRequest.Url.AsUri();
                message.Method = apiRequest.CRUDOperation.AsHttpMethod();
                message.Content = apiRequest.Data == null 
                    ? null
                    : apiRequest.Data.AsStringContent();

                var client = _httpClient.CreateClient(Strings.MagicAPI);
                HttpResponseMessage apiResponse = await client.SendAsync(message);

                APIContent = await apiResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex) 
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { ex.Message },
                    IsSuccess = true
                }; ;

                APIContent = JsonConvert.SerializeObject(dto);
            }

            var APIResponse = JsonConvert.DeserializeObject<T>(APIContent);
            return APIResponse;
        }
    }
}
