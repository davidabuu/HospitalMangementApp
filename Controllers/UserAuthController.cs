using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserAuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly AuthHelper _authHelper;

    public UserAuthController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _authHelper = new AuthHelper(configuration);
    }
    [HttpPost("RegisterUser")]
    public IActionResult RegisterUser(UserForRegistration userRegister)
    {

        if (userRegister.Password == userRegister.ConfirmPassword)
        {
            string sqlCheckUserExists = @"SELECT EmailAddress FROM SolarAppSchema.UserRegistration WHERE EmailAddress = '" + userRegister.EmailAddress + "'";
            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
            DynamicParameters sqlParameters = new DynamicParameters();
            if (existingUsers.Count() == 0)
            {
                UserForRegistration newUserRegister = new UserForRegistration()
                {
                    FirstName = userRegister.FirstName,
                    LastName = userRegister.LastName,
                    EmailAddress = userRegister.EmailAddress,
                    Password = userRegister.Password,
                    PhoneNumber = userRegister.PhoneNumber,
                    Latitude = userRegister.Latitude,
                    Longitude = userRegister.Longitude
                };
                if (_authHelper.SetPassword(newUserRegister))
                {
                    return Ok();

                }
                throw new Exception("Failed To Add User");

            }
            throw new Exception("User Already Exists");
        }
        throw new Exception("Password Do not Match");
    }
    [HttpPost("UserLogin")]
    public IActionResult UserLogin(UserLogin userLogin)
    {
        string sqlCheckUserExists = @"SELECT EmailAddress FROM SolarAppSchema.UserRegistration  WHERE EmailAddress = '" + userLogin.EmailAddress + "'";
        string sqlCommand = @"EXEC spLoginUserConfirmation
        @EmailAddress = @EmailAddressParam";
        DynamicParameters sqlParameter = new DynamicParameters();
        sqlParameter.Add("@EmailAddressParam", userLogin.EmailAddress, DbType.String);
        IEnumerable<string> existingUsers = _dapper.LoadData<string>
        (sqlCheckUserExists);
        Console.WriteLine(sqlCommand);
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
                string userIdSql = @"
                SELECT UserId FROM SolarAppSchema.UserRegistration  WHERE EmailAddress = '" + userLogin.EmailAddress + "'";
                int userId = _dapper.LoadSingleData<int>(userIdSql);
                return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId)}
            });

            }
        }
        return StatusCode(404, " User Do Not Exists");
    }
  
}