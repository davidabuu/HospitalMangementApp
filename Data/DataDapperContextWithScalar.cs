// using System.Data;
// using Dapper;
// using Microsoft.Data.SqlClient;
// namespace DotnetAPI.Data
// {
//     class DataContextDapperWtihScalar
//     {
//         private readonly IConfiguration _configuration;

//         public DataContextDapperWtihScalar(IConfiguration configuration)
//         {
//             _configuration = configuration;
//         }
//         public string ExecuteSqlWithParametersAndScalar(string sqlCommand)
//         {
//             IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//             connection.Open();
//             SqlCommand command = new SqlCommand(sqlCommand, (SqlConnection)connection);
//             return (string)command.ExecuteScalar();
//         }

//         pub

//     }
// }