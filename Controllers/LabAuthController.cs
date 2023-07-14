using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LabAuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly AuthHelper _authHelper;

    public LabAuthController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _authHelper = new AuthHelper(configuration);
    }
    [HttpPost("RegisterLabTesters")]
    public IActionResult RegisterDoctors(DoctorsRegistration labRegistration)
    {

        if (labRegistration.Password == labRegistration.ConfirmPassword)
        {
            string sqlCheckDExists = @"SELECT EmailAddress FROM HospitalSchema.LabRegistration WHERE EmailAddress = '" + labRegistration.EmailAddress + "'";
            IEnumerable<string> existingLabTester = _dapper.LoadData<string>(sqlCheckDExists);
            DynamicParameters sqlParameters = new DynamicParameters();
            if (existingLabTester.Count() == 0)
            {
                DoctorsRegistration newLabRegister = new DoctorsRegistration()
                {
                    FirstName = labRegistration.FirstName,
                    LastName = labRegistration.LastName,
                    EmailAddress = labRegistration.EmailAddress,
                    Password = labRegistration.Password,
                    PhoneNumber = labRegistration.PhoneNumber,
                    Gender = labRegistration.Gender,
                    SpecialistIn = labRegistration.SpecialistIn,
                    Qualification = labRegistration.Qualification,
                    Verified = labRegistration.Verified
                };
                if (_authHelper.SetPasswordDoctors(newLabRegister))
                {
                    return StatusCode(200, "Success");

                }
                throw new Exception("Failed To Add Lab Tester");

            }
            throw new Exception("Lab Tester Already Exists");
        }
        throw new Exception("Password Do not Match");
    }
    [HttpPost("LabTestersLogin")]
    public IActionResult UserLogin(UserLogin userLogin)
    {
        string sqlCheckUserExists = @"SELECT EmailAddress FROM HospitalSchema.LabRegistration  WHERE EmailAddress = '" + userLogin.EmailAddress + "'";
        string sqlCommand = @"EXEC spLoginConfirmation_GetForLabTesters
        @EmailAddress = @EmailAddressParam";
        DynamicParameters sqlParameter = new DynamicParameters();
        sqlParameter.Add("@EmailAddressParam", userLogin.EmailAddress, DbType.String);
        IEnumerable<string> existingUsers = _dapper.LoadData<string>
        (sqlCheckUserExists);
        if (existingUsers.Count() != 0)
        {
            UserForLoginConfirmation userForConfirmation = _dapper
                            .LoadDataSingleWithParameters<UserForLoginConfirmation>(sqlCommand, sqlParameter);
            byte[] passwordHash = _authHelper.GetPasswordHash(userLogin.Password, userForConfirmation.PasswordSalt);
            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect password!");
                }
                string labIdSql = @"
                SELECT DoctorsID FROM HospitalSchema.LabRegistration  WHERE EmailAddress = '" + userLogin.EmailAddress + "'";
                int labId = _dapper.LoadSingleData<int>(labIdSql);
                return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(labId)}
            });

            }
        }
        return StatusCode(404, " Lab Tester Do Not Exists");
    }
    [HttpGet("ViewPatientsDetails")]
    public IActionResult ViewPatientsDetails(string emailAddress)
    {
        string storedProcedure = "spViewPatientRecords";
        var parameters = new { EmailAddress = emailAddress };
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