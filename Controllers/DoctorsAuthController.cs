using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DoctorAuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly AuthHelper _authHelper;

    public DoctorAuthController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _authHelper = new AuthHelper(configuration);
    }
    [HttpPost("RegisterDoctors")]
    public IActionResult RegisterDoctors(DoctorsRegistration doctorsRegistration)
    {

        if (doctorsRegistration.Password == doctorsRegistration.ConfirmPassword)
        {
            string sqlCheckDExists = @"SELECT EmailAddress FROM HospitalSchema.DoctorsRegistration WHERE EmailAddress = '" + doctorsRegistration.EmailAddress + "'";
            IEnumerable<string> existingDoctors = _dapper.LoadData<string>(sqlCheckDExists);
            DynamicParameters sqlParameters = new DynamicParameters();
            if (existingDoctors.Count() == 0)
            {
                DoctorsRegistration newDoctorRegister = new DoctorsRegistration()
                {
                    FirstName = doctorsRegistration.FirstName,
                    LastName = doctorsRegistration.LastName,
                    EmailAddress = doctorsRegistration.EmailAddress,
                    Password = doctorsRegistration.Password,
                    PhoneNumber = doctorsRegistration.PhoneNumber,
                    Gender = doctorsRegistration.Gender,
                    SpecialistIn = doctorsRegistration.SpecialistIn,
                    Qualification = doctorsRegistration.Qualification,
                    Verified = doctorsRegistration.Verified
                };
                if (_authHelper.SetPasswordDoctors(newDoctorRegister))
                {
                    return StatusCode(200, "Success");

                }
                throw new Exception("Failed To Add Doctor");

            }
            throw new Exception("Doctor Already Exists");
        }
        throw new Exception("Password Do not Match");
    }
    [HttpPost("DoctorsLogin")]
    public IActionResult UserLogin(UserLogin userLogin)
    {
        string sqlCheckUserExists = @"SELECT EmailAddress FROM HospitalSchema.DoctorsRegistration  WHERE EmailAddress = '" + userLogin.EmailAddress + "'";
        string sqlCommand = @"EXEC spLoginConfirmation_GetForDoctors
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
                string doctorsIdSql = @"
                SELECT DoctorsID FROM HospitalSchema.DoctorsRegistration  WHERE EmailAddress = '" + userLogin.EmailAddress + "'";
                int doctorsId = _dapper.LoadSingleData<int>(doctorsIdSql);
                return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(doctorsId)}
            });

            }
        }
        return StatusCode(404, " Doctor Do Not Exists");
    }
    [HttpGet("ViewPatientsDetails")]
    public IActionResult ViewPatientsDetails(string emailAddress)
    {
        string storedProcedure = "spViewPatientRecords";
        var parameters = new { UserName = emailAddress };
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
    [HttpGet("DoctorsAppointment")]
    public IActionResult DoctorsAppointment(string emailAddress)
    {
        string storedProcedure = "spDoctorAppointment";
        var parameters = new { EmailAddress = emailAddress };
        IEnumerable<dynamic> appointmentDetails = _dapper.Connection().Query(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        string unauthorizedMessage = "Unauthorized User";
        if (appointmentDetails.AsList().Count > 0 && appointmentDetails.AsList()[0].ToString() != unauthorizedMessage)
        {
            return Ok(appointmentDetails);
        }
        else
        {
            return Unauthorized();
        }
    }
}