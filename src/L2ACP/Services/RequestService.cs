using System.Net.Http;
using System.Threading.Tasks;
using L2ACP.Models;
using L2ACP.Requests;
using Newtonsoft.Json;

namespace L2ACP.Services
{
    public class RequestService : IRequestService
    {
        public async Task<L2Response> LoginUser(string username, string password)
        {
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var request = await new HttpClient().PostAsync("http://localhost:8000/api", new JsonContent(loginRequest));

            var result = await request.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<L2Response>(result);

            return responseObject;
        }

        public async Task<L2Response> RegisterUser(string username, string password)
        {
            var loginRequest = new RegisterRequest
            {
                Username = username,
                Password = password
            };

            var request = await new HttpClient().PostAsync("http://localhost:8000/api", new JsonContent(loginRequest));

            var result = await request.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<L2Response>(result);

            return responseObject;
        }
    }
}