using System.Net.Http;
using System.Text;
using L2ACP.Requests;
using Newtonsoft.Json;

namespace L2ACP.Models
{
    public class L2Response
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public string ResponseContent { get; set; }
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        { }
    }
}