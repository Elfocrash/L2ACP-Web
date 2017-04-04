using System.Threading.Tasks;
using L2ACP.Models;
using L2ACP.Responses;

namespace L2ACP.Services
{
    public interface IRequestService
    {
        Task<L2Response> LoginUser(string username, string password);

        Task<L2Response> RegisterUser(string username, string password);

        Task<L2Response> GetAllCharNames(string getUsername);

        Task<L2Response> GetInventory(string player);
    }
}