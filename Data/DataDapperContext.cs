using System.Data;
using Dapper;
using DotnetAPI.Model;
using Microsoft.Data.SqlClient;
namespace DotnetAPI.Data
{
    class DataContextDapper
    {
        private readonly IConfiguration _configuration;

        public DataContextDapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public System.Data.IDbConnection Connection()
        {
            IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return connection;
        }
        public IEnumerable<T> LoadData<T>(string sqlCommand)
        {
            IDbConnection connection = Connection();
            return connection.Query<T>(sqlCommand);
        }
        public T LoadSingleData<T>(string sqlCommand)
        {
            IDbConnection connection = Connection();
            return connection.QuerySingle<T>(sqlCommand);
        }
        public bool ExecuteSqlWithParameters(string sqlCommand, DynamicParameters parameters)
        {
            IDbConnection connection = Connection();
            return connection.Execute(sqlCommand, parameters) > 0;
        }
        public IEnumerable<T> LoadDataWithParameters<T>(string sqlCommand, DynamicParameters parameters)
        {
            IDbConnection connection = Connection();
            return connection.Query<T>(sqlCommand, parameters);
        }

        public T LoadDataSingleWithParameters<T>(string sqlCommand, DynamicParameters parameters)
        {
            IDbConnection connection = Connection();
            return connection.QuerySingle<T>(sqlCommand, parameters);
        }
        public string ExecuteSqlWithParametersAndScalar(string sql, SolarModel solarModel)
        {
            IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            SqlCommand command = new SqlCommand(sql, (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = solarModel.UserId;
            command.Parameters.Add("@GetCurrent", SqlDbType.Decimal).Value = solarModel.Current;
            command.Parameters.Add("@Voltage", SqlDbType.Decimal).Value = solarModel.Voltage;
            command.Parameters.Add("@Radiance", SqlDbType.Decimal).Value = solarModel.Radiance;
            command.Parameters.Add("@GetDate", SqlDbType.Decimal).Value = solarModel.GetDate;
            command.Parameters.Add("@GetStatus", SqlDbType.Int).Value = solarModel.Status;
            return (string)command.ExecuteScalar();
        }

     
    }
}