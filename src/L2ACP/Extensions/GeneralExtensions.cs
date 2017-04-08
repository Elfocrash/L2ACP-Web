using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using L2ACP.Requests;
using L2ACP.Responses;
using L2ACP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace L2ACP.Extensions
{
    public static class GeneralExtensions
    {
        private const string ApiUrl = "http://localhost:8000/api";
        public static string ToL2Password(this string str)
        {
            SHA1 shA1 = SHA1.Create();
            byte[] bytes = new ASCIIEncoding().GetBytes(str);
            str = Convert.ToBase64String(shA1.ComputeHash(bytes));
            return str;
        }

        public static string GetUsername(this HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.Claims.FirstOrDefault(x => x.Type == "username").Value;
            }
            return string.Empty;
        }

        public static async Task<T> SendPostRequest<T>(this L2Request req) where T : L2Response
        {
            var request = await new HttpClient().PostAsync(ApiUrl, new JsonContent(req));

            var result = await request.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<T>(result);
            return responseObject;
        }

        public static GetAccountInfoResponse GetAccountInfo(this HttpContext context)
        {
            return (GetAccountInfoResponse)context.Items["AccountInfo"];
        }

        public static void InjectInfoToContext(this HttpContext context, GetAccountInfoResponse accountInfo)
        {
            context.Items["AccountInfo"] = accountInfo;
        }

        public static GetAccountInfoResponse RetrieveAccountInfo(this HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var username = context.User.Claims.FirstOrDefault(x => x.Type == "username").Value;
                var chars = context.RequestServices.GetRequiredService<IRequestService>().GetAccountInfo(username).Result;
                var allCharsResponse = chars as GetAccountInfoResponse;
                if (allCharsResponse?.ResponseCode == 200)
                {
                    return allCharsResponse;
                }
                return null;
            }
            return null;
        }

    }
}