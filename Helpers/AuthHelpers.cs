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

        public bool SetPassword(UserForRegistration userRegistration)
        {

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userRegistration.Password, passwordSalt);
            string sql = @"EXEC spUserRegistration
            @FirstName = @FirstNameParam,
            @LastName = @LastNameParam,
            @EmailAddress = @EmailAddressParam,
            @PasswordHash = @PasswordHashParam,
            @PasswordSalt = @PasswordSaltParam,
            @Latitude = @LatitudeParam,
            @Longitude = @LongitudeParam,
            @PhoneNumber = @PhoneNumberParam";
            DynamicParameters sqlParamters = new DynamicParameters();
            sqlParamters.Add("@FirstNameParam", userRegistration.FirstName, DbType.String);
            sqlParamters.Add("@LastNameParam", userRegistration.LastName, DbType.String);
            sqlParamters.Add("@EmailAddressParam", userRegistration.EmailAddress, DbType.String);
            sqlParamters.Add("@LatitudeParam", userRegistration.Latitude, DbType.Double);
            sqlParamters.Add("@LongitudeParam", userRegistration.Longitude, DbType.Double);
            sqlParamters.Add("@PhoneNumberParam", userRegistration.PhoneNumber, DbType.String);
            sqlParamters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParamters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);
            return _dapper.ExecuteSqlWithParameters(sql, sqlParamters);

        }

    }

}