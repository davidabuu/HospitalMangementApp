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
        string sqlCommand = @"EXEC spAdminVerifyUser
        @UserId = @UserIdParam,
        @EmailAddress = @EmailAddressParam";
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@EmailAddressParam", userVerification.EmailAddress, DbType.String);
        sqlParameters.Add("@UserIdParam", userVerification.UserId, DbType.Int32);
        if (_dapper.ExecuteSqlWithParameters(sqlCommand, sqlParameters))
        {
            return StatusCode(200, "Success");
        }
        throw new Exception("Only Admin can verify users");
    }
    [HttpPost("AdminLogin")]
    public IActionResult AdminLogin(AdminVerification adminVerification)
    {
        string sql = @"SELECT * FROM SolarAppSchema.AdminRegistration WHERE EmailAddress = '" + adminVerification.EmailAddress + "' AND AdminCode =  " + adminVerification.AdminCode + "";
        Console.WriteLine(sql);
        IEnumerable<string> existingUser = _dapper.LoadData<string>(sql);
        if (existingUser.Count() > 0)
        {
            return StatusCode(200, "Success");
        }
        return StatusCode(404, "Admin Not Found");

    }
    [HttpGet("GetAllUsers")]
    public IEnumerable<UserForRegistrationInfo> GetUsers()
    {
        string sqlCommand = "SELECT * FROM SolarAppSchema.UserRegistration";

        return _dapper.LoadData<UserForRegistrationInfo>(sqlCommand);



    }
    [HttpPost("UserMonitoringData")]
    public IActionResult AddUserMonitoringData(UserMonitoringDataModel userMonitoringDataModel)
    {
        string sqlCommand = @"EXEC spUserMonitoringDevice
        @UserId = @UserIdParam,
        @EmailAddress = @EmailAddressParam,
        @MacAddress = @MacAddressParam,
        @IpAddress = @IpAddressParam,
        @Port = @PortParam";
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@EmailAddressParam", userMonitoringDataModel.EmailAddress, DbType.String);
        sqlParameters.Add("@UserIdParam", userMonitoringDataModel.UserId, DbType.Int32);
        sqlParameters.Add("@MacAddressParam", userMonitoringDataModel.MacAddress, DbType.String);
        sqlParameters.Add("@IpAddressParam", userMonitoringDataModel.IpAddress, DbType.String);
        sqlParameters.Add("@PortParam", userMonitoringDataModel.Port, DbType.Int32);
        if (_dapper.ExecuteSqlWithParameters(sqlCommand, sqlParameters))
        {
            return StatusCode(200, "Success");
        }
        return StatusCode(401, "Only Admin can call this function for users");
    }
    [HttpGet("MonitoringDetails/{userId}")]
    public IEnumerable<UserMonitoringDataModel> GetUserDetails(int userId)
    {
        string sqlCommand = "SELECT * FROM SolarAppSchema.UserMonitoringDevice WHERE UserId = " + userId + "";

        return _dapper.LoadData<UserMonitoringDataModel>(sqlCommand);



    }
    [HttpPost("UserPowerData")]
    public IActionResult AddPowerData(UserPowePlantDataModel userPowePlantDataModel)
    {
        string sqlCommand = @"EXEC spUserPowerPlantData
        @UserId = @UserIdParam,
        @EmailAddress = @EmailAddressParam,
       @Capacity = @CapacityParam,
       @ShortCircuitVoltage = @ShortParam,
       @InverterCapactity = @InverterParam";
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@EmailAddressParam", userPowePlantDataModel.EmailAddress, DbType.String);
        sqlParameters.Add("@UserIdParam", userPowePlantDataModel.UserId, DbType.Int32);
        sqlParameters.Add("@CapacityParam", userPowePlantDataModel.Capacity, DbType.Decimal);
        sqlParameters.Add("@ShortParam", userPowePlantDataModel.ShortCircuitVoltage, DbType.Decimal);
        sqlParameters.Add("@InverterParam", userPowePlantDataModel.InverterCapactity, DbType.Decimal);
        Console.WriteLine(sqlCommand);
        if (_dapper.ExecuteSqlWithParameters(sqlCommand, sqlParameters))
        {
            return StatusCode(200, "Success");
        }
        return StatusCode(401, "Only Admin can call this function for users");
    }
    [HttpGet("PowerPlant/{userId}")]
    public IEnumerable<UserPowePlantDataModel> GetPowerPlantDetails(int userId)
    {
        string sqlCommand = "SELECT * FROM SolarAppSchema.UserSolarPowerPlant WHERE UserId = " + userId + "";

        return _dapper.LoadData<UserPowePlantDataModel>(sqlCommand);



    }
}