using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace L2ACP.Services
{
    public interface IAuthService
    {
        Task SignInUser(string username, HttpContext context);
    }
}