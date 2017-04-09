using System.Net.Http;
using System.Text;
using L2ACP.Cryptography;
using L2ACP.Extensions;
using Newtonsoft.Json;

namespace L2ACP.Responses
{
    public class L2Response
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object obj) : base(AesCrypto.EncryptRijndael(JsonConvert.SerializeObject(obj), Constants.Salt), Encoding.UTF8, "application/json")
        { }
    }
}