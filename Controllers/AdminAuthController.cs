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
        string sqlCheckUserExists = @"SELECT UserName FROM HospitalSchema.AdminLogin  WHERE UserName = '" + adminLogin.UserName + "'";
        string sqlCommand = @"EXEC spLoginConfirmation_GetForAdmins
        @UserName = @UserNameParam";
        DynamicParameters sqlParameter = new DynamicParameters();
        sqlParameter.Add("@UserNameParam", adminLogin.UserName, DbType.String);
        IEnumerable<string> existingUsers = _dapper.LoadData<string>
        (sqlCheckUserExists);
        if (existingUsers.Count() != 0)
        {
            UserForLoginConfirmation userForConfirmation = _dapper
                            .LoadDataSingleWithParameters<UserForLoginConfirmation>(sqlCommand, sqlParameter);
            byte[] passwordHash = _authHelper.GetPasswordHash(adminLogin.Password, userForConfirmation.PasswordSalt);
            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect password!");
                }
                string adminIdSql = @"
                SELECT AdminID FROM HospitalSchema.AdminLogin  WHERE UserName = '" + adminLogin.UserName + "'";
                int adminId = _dapper.LoadSingleData<int>(adminIdSql);
                return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(adminId)}
            });

            }
        }
        return StatusCode(404, " Admin Do Not Exists");
    }
    [HttpDelete("DeleteDoctor")]
    public IActionResult DeleteDoctor(int id, string userName)
    {
        string storedProcedure = "spAdminDeleteDoctors";
        var parameters = new { DoctorsId = id, userName = userName };
        IEnumerable<dynamic> doctor = _dapper.Connection().Query(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        string unauthorizedMessage = "Unauthorized User";
        if (doctor.AsList().Count > 0 && doctor.AsList()[0].ToString() != unauthorizedMessage)
        {
            return Ok(doctor);
        }
        else
        {
            return Unauthorized();
        }
    }
    [HttpGet("GetAllDoctors")]
    public IActionResult GetAllDoctors(string userName)
    {
        string storedProcedure = "spAdminViewDoctors";
        var parameters = new { UserName = userName };
        IEnumerable<dynamic> allDoctors = _dapper.Connection().Query(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        string unauthorizedMessage = "Unauthorized User";
        if (allDoctors.AsList().Count > 0 && allDoctors.AsList()[0].ToString() != unauthorizedMessage)
        {
            return Ok(allDoctors);
        }
        else
        {
            return Unauthorized();
        }
    }
    [HttpPut("VerifyDoctors")]
    public IActionResult VerifyDoctors(string userName, int id)
    {
        string storedProcedure = "spAdminVerifyDoctors";
        var parameters = new { UserName = userName, DoctorsId = id };
        IEnumerable<dynamic> doctor = _dapper.Connection().Query(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        string unauthorizedMessage = "Unauthorized User";
        if (doctor.AsList().Count > 0 && doctor.AsList()[0].ToString() != unauthorizedMessage)
        {
            return Ok(doctor);
        }
        else
        {
            return Unauthorized();
        }
    }
    [HttpGet("ViewPatientsDetails")]
    public IActionResult ViewPatientsDetails(string userName)
    {
        string storedProcedure = "spAdminViewPatientRecords";
        var parameters = new { UserName = userName };
        IEnumerable<dynamic> patientsDetails = _dapper.Connection().Query(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        string unauthorizedMessage = "Unauthorized User";
        if (patientsDetails.AsList().Count > 0 && patientsDetails.AsList()[0].ToString() != unauthorizedMessage)
        {
            return Ok(patientsDetails);
        }
        else
        {
            return Unauthorized();
        }
    }
}