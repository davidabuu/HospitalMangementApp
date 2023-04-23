using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;
// [Microsoft.AspNetCore.Authorization.Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly AuthHelper _authHelper;

    public AuthController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _authHelper = new AuthHelper(configuration);
    }
    [HttpPost("SolarDetailsPost")]
    public IActionResult AddSolarDetails(SolarModel solarModel)
    {
        string sqlCommand = "spSolarDetailsPerUser";
        SolarModel solar = new SolarModel()
        {
            UserId = solarModel.UserId,
            Current = solarModel.Current,
            Voltage = solarModel.Voltage,
            Radiance = solarModel.Radiance,
            GetDate = solarModel.GetDate,
            Status = solarModel.Status
        };
        if(_dapper.ExecuteSqlWithParametersAndScalar(sqlCommand, solar) == "User is not verified yet")
        {
            throw new Exception("User is not verified yet");
        }
        else if (_dapper.ExecuteSqlWithParametersAndScalar(sqlCommand, solar) == "User is not found")
        {
            throw new Exception("User is not found");
        }
        else{
            return Ok();
        }
    }
}