using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
namespace DotnetAPI.Data
{
    class DataContextDapperWtihScalar
    {
        private readonly IConfiguration _configuration;

        public DataContextDapperWtihScalar(IConfiguration configuration)
        {
            _configuration = configuration;
        }
      

    }
}