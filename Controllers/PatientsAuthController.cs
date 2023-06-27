// using System.Data;
// using Dapper;
// using DotnetAPI.Data;
// using DotnetAPI.Helpers;
// using DotnetAPI.Model;
// using Microsoft.AspNetCore.Mvc;

// namespace DotnetAPI.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class PatientsAuthController : ControllerBase
// {
//     private readonly DataContextDapper _dapper;

//     private readonly AuthHelper _authHelper;

//     public PatientsAuthController(IConfiguration configuration)
//     {
//         _dapper = new DataContextDapper(configuration);
//         _authHelper = new AuthHelper(configuration);
//     }
//     [HttpPost("RegisterPatients")]
//     public IActionResult RegisterPatients(PatientsRegistration patientsRegistration)
//     {

//         if (patientsRegistration.Password == patientsRegistration.ConfirmPassword)
//         {
//             string sqlCheckDExists = @"SELECT EmailAddress FROM HospitalSchema.PatientsRegistration WHERE EmailAddress = '" + patientsRegistration.EmailAddress + "'";
//             IEnumerable<string> existingDoctors = _dapper.LoadData<string>(sqlCheckDExists);
//             DynamicParameters sqlParameters = new DynamicParameters();
//             if (existingDoctors.Count() == 0)
//             {
//                 PatientsRegistration newPatientsRegister = new PatientsRegistration()
//                 {
//                     FirstName = patientsRegistration.FirstName,
//                     LastName = patientsRegistration.LastName,
//                     EmailAddress = patientsRegistration.EmailAddress,
//                     Password = patientsRegistration.Password,
//                     PhoneNumber = patientsRegistration.PhoneNumber,
//                     Gender = patientsRegistration.Gender,
//                     PatientsAddress = patientsRegistration.PatientsAddress,
//                     PatientsState = patientsRegistration.PatientsState
//                 };
//                 if (_authHelper.SetPassword(newDoctorRegister))
//                 {
//                     return StatusCode(200, "Success");

//                 }
//                 throw new Exception("Failed To Add Patient");

//             }
//             throw new Exception("Patient Already Exists");
//         }
//         throw new Exception("Password Do not Match");
//     }
//     [HttpPost("PatientsLogin")]
//     public IActionResult UserLogin(UserLogin userLogin)
//     {
//         string sqlCheckUserExists = @"SELECT EmailAddress FROM HospitalSchema.PatientsRegistration  WHERE EmailAddress = '" + userLogin.EmailAddress + "'";
//         string sqlCommand = @"EXEC spLoginConfirmation_GetForPatients
//         @EmailAddress = @EmailAddressParam";
//         DynamicParameters sqlParameter = new DynamicParameters();
//         sqlParameter.Add("@EmailAddressParam", userLogin.EmailAddress, DbType.String);
//         IEnumerable<string> existingUsers = _dapper.LoadData<string>
//         (sqlCheckUserExists);
//         if (existingUsers.Count() != 0)
//         {
//             UserForLoginConfirmation userForConfirmation = _dapper
//                             .LoadDataSingleWithParameters<UserForLoginConfirmation>(sqlCommand, sqlParameter);
//             byte[] passwordHash = _authHelper.GetPasswordHash(userLogin.Password, userForConfirmation.PasswordSalt);
//             for (int index = 0; index < passwordHash.Length; index++)
//             {
//                 if (passwordHash[index] != userForConfirmation.PasswordHash[index])
//                 {
//                     return StatusCode(401, "Incorrect password!");
//                 }
//                 string patientIdSql = @"
//                 SELECT PatientsID FROM HospitalSchema.PatientsRegistration  WHERE EmailAddress = '" + userLogin.EmailAddress + "'";
//                 int patientId = _dapper.LoadSingleData<int>(patientIdSql);
//                 return Ok(new Dictionary<string, string> {
//                 {"token", _authHelper.CreateToken(patientId)}
//             });

//             }
//         }
//         return StatusCode(404, " Patient Do Not Exists");
//     }
// }