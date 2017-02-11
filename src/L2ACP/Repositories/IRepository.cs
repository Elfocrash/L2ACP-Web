using System.Threading.Tasks;

namespace L2ACP.Repositories
{
    public interface IRepository
    {
        Task<bool> IsValidLogin(string username, string password);

        Task<bool> IsValidRegister(string username);

        Task RegisterUser(string username, string password);
    }
}