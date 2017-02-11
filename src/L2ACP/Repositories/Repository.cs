using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace L2ACP.Repositories
{
    public class Repository : IRepository
    {
        private readonly IDbConnection _con = new MySqlConnection(Startup.Configuration.GetConnectionString("DefaultConnection"));

        public async Task<bool> IsValidLogin(string username, string password)
        {
            var result = await _con.QueryAsync<bool>("select count(*) from accounts where login = @username and password = @password", new {username = username,password = password});
            return result.FirstOrDefault();
        }

        public async Task<bool> IsValidRegister(string username)
        {
            var result = await _con.QueryAsync<bool>("select count(*) from accounts where login = @username", new { username = username });
            return !result.FirstOrDefault();
        }

        public async Task RegisterUser(string username, string password)
        {
            await _con.ExecuteAsync(
                "insert into accounts (login,password,lastactive,access_level,lastServer) values (@username,@password,0,0,1)",
                new {username = username, password = password});
        }
    }
}