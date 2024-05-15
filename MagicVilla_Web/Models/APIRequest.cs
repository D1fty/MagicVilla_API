using MagicVilla_Utility;
using Newtonsoft.Json;
using System.Text;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        public CRUDOpertion CRUDOperation { get; set; }

        public string Url { get; set; }

        public object Data { get; set; }
    }

    public static class APIRequestExtensions
    {
        public static Uri AsUri(this string Url)
            => new Uri(Url);

        public static StringContent AsStringContent(this object data)
            => new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, Strings.AppJson);

        public static HttpMethod AsHttpMethod(this CRUDOpertion crudOperation)
        {
            switch (crudOperation)
            {
                case CRUDOpertion.POST:
                    return HttpMethod.Post;
                case CRUDOpertion.PUT:
                    return HttpMethod.Put;
                case CRUDOpertion.DELETE:
                    return HttpMethod.Delete;
                default:
                    return HttpMethod.Get;
            }
        }
    }
}
