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
    [HttpPost("RegisterAdmin")]
    public IActionResult RegisterAdmin(AdminRegistration adminRegistration)
    {

        if (adminRegistration.Password == adminRegistration.ConfirmPassword)
        {
            string sqlCheckDExists = @"SELECT UserName FROM HospitalSchema.AdminLogin WHERE UserName = '" + adminRegistration.UserName + "'";
            IEnumerable<string> existingAdmins = _dapper.LoadData<string>(sqlCheckDExists);
            DynamicParameters sqlParameters = new DynamicParameters();
            if (existingAdmins.Count() == 0)
            {
                AdminRegistration newAdminRegister = new AdminRegistration()
                {

                    UserName = adminRegistration.UserName,
                    Password = adminRegistration.Password,
                    AdminRole = adminRegistration.AdminRole,

                };
                if (_authHelper.SetPasswordAdmin(newAdminRegister))
                {
                    return StatusCode(200, "Success");

                }
                throw new Exception("Failed To Add Admin");

            }
            throw new Exception("Admin Already Exists");
        }
        throw new Exception("Password Do not Match");
    }
    [HttpPost("AdminLogin")]
    public IActionResult AdminLogin(AdminLogin adminLogin)
    {
        string sql = @"SELECT * FROM HospitalSchema.AdminLogin WHERE UserName = '" + adminLogin.UserName + "' AND AdminRole =  '" + adminLogin.AdminRole + "'";
        Console.WriteLine(sql);
        IEnumerable<string> existingUser = _dapper.LoadData<string>(sql);
        if (existingUser.Count() > 0)
        {
            return StatusCode(200, "Success");
        }
        return StatusCode(404, "Admin Not Found");

    }

}