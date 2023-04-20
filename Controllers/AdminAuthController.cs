using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminAuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly AuthHelper _authHelper;

    public AdminAuthController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _authHelper = new AuthHelper(configuration);
    }
    [HttpPut("VerifyUser")]
    public IActionResult VerifyUser(AdminUserVerification userVerification)
    {
        string sqlCommand = @"EXEC spUserVerification
        @UserId = @UserIdParam
        @EmailAddress = @EmailAddressParam";
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@EmailAddressParam", userVerification.EmailAddress, DbType.String);
        sqlParameters.Add("@UserIdParam", userVerification.UserId, DbType.Int32);
        if (_dapper.ExecuteSqlWithParameters(sqlCommand, sqlParameters))
        {
            return Ok();
        }
        throw new Exception("Only Admin can verify users");
    }
}