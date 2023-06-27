using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;

        public AuthHelper(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }
        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }


        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""
                    )
                );

            SigningCredentials credentials = new SigningCredentials(
                    tokenKey,
                    SecurityAlgorithms.HmacSha512Signature
                );

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);

        }

        public bool SetPasswordAdmin(AdminRegistration adminRegistration)
        {

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(adminRegistration.Password, passwordSalt);
            string sql = @"EXEC spAdminLogin
            @UserName = @UserNameParam,
            @PasswordSalt = @PasswordSaltParam,
            @PasswordHash = @PasswordHashParam,
            @AdminRole = @AdminRoleParam";
            DynamicParameters sqlParamters = new DynamicParameters();
            Console.WriteLine(sql);
            sqlParamters.Add("@UserNameParam", adminRegistration.UserName, DbType.String);
            sqlParamters.Add("@AdminRoleParam", adminRegistration.AdminRole, DbType.String);
            sqlParamters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParamters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);
            return _dapper.ExecuteSqlWithParameters(sql, sqlParamters);

        }
        public bool SetPasswordDoctors(DoctorsRegistration doctorsRegistration)
        {

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(doctorsRegistration.Password, passwordSalt);
            string sql = @"EXEC spAddDoctors
            @FirstName = @FirstNameParam,
            @LastName = @LastNameParam,
            @EmailAddress = @EmailAddressParam,
            @PasswordHash = @PasswordHashParam,
            @PasswordSalt = @PasswordSaltParam,
            @Gender = @GenderParam,
            @Qualification = @QualificationParam,
            @SpecialistIn = @SpecialistInParam,
            @Verified = @VerifiedParam,
            @PhoneNumber = @PhoneNumberParam";
            DynamicParameters sqlParamters = new DynamicParameters();
            sqlParamters.Add("@FirstNameParam", doctorsRegistration.FirstName, DbType.String);
            sqlParamters.Add("@LastNameParam", doctorsRegistration.LastName, DbType.String);
            sqlParamters.Add("@EmailAddressParam", doctorsRegistration.EmailAddress, DbType.String);
            sqlParamters.Add("@GenderParam", doctorsRegistration.Gender, DbType.String);
            sqlParamters.Add("@QualificationParam", doctorsRegistration.Qualification, DbType.String);
            sqlParamters.Add("@PhoneNumberParam", doctorsRegistration.PhoneNumber, DbType.String);
            sqlParamters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParamters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);
            sqlParamters.Add("@SpecialistInParam", doctorsRegistration.SpecialistIn, DbType.String);
            sqlParamters.Add("@VerifiedParam", doctorsRegistration.Verified, DbType.Int16);
            return _dapper.ExecuteSqlWithParameters(sql, sqlParamters);

        }
        public bool SetPasswordForPatients(PatientsRegistration patientsRegistration)
        {

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(patientsRegistration.Password, passwordSalt);
            string sql = @"EXEC spAddpatients
            @FirstName = @FirstNameParam,
            @LastName = @LastNameParam,
            @EmailAddress = @EmailAddressParam,
            @PasswordHash = @PasswordHashParam,
            @PasswordSalt = @PasswordSaltParam,
            @Age = @AgeParam,
            @Gender = @GenderParam,
            @PatientsAddress = @PatientsStateParam,
            @PatientsState = @PatientsStateParam,
            @PhoneNumber = @PhoneNumberParam";
            DynamicParameters sqlParamters = new DynamicParameters();
            sqlParamters.Add("@FirstNameParam", patientsRegistration.FirstName, DbType.String);
            sqlParamters.Add("@LastNameParam", patientsRegistration.LastName, DbType.String);
            sqlParamters.Add("@EmailAddressParam", patientsRegistration.EmailAddress, DbType.String);
            sqlParamters.Add("@GenderParam", patientsRegistration.Gender, DbType.String);
            sqlParamters.Add("@PatientsStateParam", patientsRegistration.PatientsState, DbType.String);
            sqlParamters.Add("@PhoneNumberParam", patientsRegistration.PhoneNumber, DbType.String);
            sqlParamters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParamters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);
            sqlParamters.Add("@PatientsStateParam", patientsRegistration.PatientsState, DbType.String);
            sqlParamters.Add("@AgeParam", patientsRegistration.Age, DbType.Int16);

            return _dapper.ExecuteSqlWithParameters(sql, sqlParamters);

        }

    }


}
    

