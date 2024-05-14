using static MagicVilla_Utility.StaticDetails;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        public CRUDOpertion CURDOperation { get; set; }

        public string Url {  get; set; }

        public object Data {  get; set; }
    }
}
