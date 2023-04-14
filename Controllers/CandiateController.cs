using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CandidateController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly AuthHelper _authHelper;

    public CandidateController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
        _authHelper = new AuthHelper(configuration);
    }


    [HttpPost("RegisterCandidate")]
    public IActionResult RegisterCandidate(CandidateModel candidateModel)
    {
        string sqlCheckAdminExists = @"SELECT EmailAddress FROM VotingAppSchema.AdminRegistration WHERE EmailAddress = '" + candidateModel.AdminEmailAddress + "'";
        Console.WriteLine(sqlCheckAdminExists);
        if (sqlCheckAdminExists.Count() > 0)
        {
            string sqlCommand = @"EXEC spCandidateRegistrationAndUpdate
        @FirstName = @FirstNameParam,
        @LastName = @LastNameParam,
        @CandidateRole = @CandidateRoleParam,
        @EmailAddress = @AdminEmailAddressParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@FirstNameParam", candidateModel.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParam", candidateModel.LastName, DbType.String);
            sqlParameters.Add("@CandidateRoleParam", candidateModel.CandidateRole, DbType.String);
            sqlParameters.Add("@AdminEmailAddressParam", candidateModel.AdminEmailAddress, DbType.String);
            if (_dapper.ExecuteSqlWithParameters(sqlCommand, sqlParameters))
            {
                return Ok();
            }
        }
        return StatusCode(401, "Unauthorized");
    }
}