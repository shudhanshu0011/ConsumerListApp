using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
namespace WebCalculator.DapperContext
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("Con");
        }
        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}