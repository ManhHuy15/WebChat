using BusinessObjects;
using DTOs.AuthenDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.UserRepository;
using Services.AuthenServices.InterfaceAuthen;
using Services.UserServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services.AuthenServices
{
    public class JWTService : IJWTService
    {
        public IConfiguration _configuration { get; }
        public IUserRepository _userRepository { get; }
        private string _issuer;
        private string _audience;
        private string _key;
        private int _expiry;
        private DateTime _expiryTimesStamp;

        public JWTService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _issuer = _configuration["JwtConfig:Issuer"];
            _audience = _configuration["JwtConfig:Audience"];
            _key = _configuration["JwtConfig:Key"];
            _expiry = int.Parse(_configuration["JwtConfig:ExpiryInMinutes"]);
            _expiryTimesStamp = DateTime.Now.AddMinutes(_expiry);
        }


        public async Task<UserLoginResponseDTOs> AuthenticateUser(User loginUser)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, loginUser.Email),
                    new Claim(ClaimTypes.Role, Enums.GetRoleName(loginUser.Role))
                }),
                Expires = _expiryTimesStamp,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            loginUser.RefreshToken = GenerateRefreshToken();
            loginUser.TokenExpires = DateTime.Now.AddDays(7);

            await _userRepository.Update(loginUser);

            return new UserLoginResponseDTOs
            {
                Email = loginUser.Email,
                AccessToken = jwtToken,
                TokenExpires = (int)_expiryTimesStamp.Subtract(DateTime.Now).TotalSeconds,
                RefreshToken = loginUser.RefreshToken,
            };
        }

        //public async Task<LoginResponseUserDTO> AuthenticateUserWithRefreshToken(string RequsetRefreshToken)
        //{
        //    var refreshToken = await MyLearnContext.INSTANCE.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == RequsetRefreshToken);
        //    if (refreshToken == null || refreshToken.ExpiresAt < DateTime.Now)
        //    {
        //        throw new ApplicationException("The refresh token has expired");
        //    }

        //    var issuer = _configuration["JwtConfig:Issuer"];
        //    var audience = _configuration["JwtConfig:Audience"];
        //    var key = _configuration["JwtConfig:Key"];
        //    var expiry = _configuration.GetValue<int>("JwtConfig:ExpiryInMinutes");
        //    var expiryTimesStamp = DateTime.Now.AddMinutes(expiry);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {

        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(JwtRegisteredClaimNames.Name, refreshToken.User.UserName),
        //            new Claim(ClaimTypes.Role, "Admin")
        //        }),
        //        Expires = expiryTimesStamp,
        //        Issuer = issuer,
        //        Audience = audience,
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        //        SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var jwtToken = tokenHandler.WriteToken(token);

        //    refreshToken.Token = GenerateRefreshToken();
        //    refreshToken.ExpiresAt = DateTime.Now.AddDays(7);
        //    await MyLearnContext.INSTANCE.SaveChangesAsync();

        //    return new LoginResponseUserDTO
        //    {
        //        Username = refreshToken.User.UserName,
        //        AccessToken = jwtToken,
        //        ExpiresIn = (int)expiryTimesStamp.Subtract(DateTime.Now).TotalSeconds,
        //        RefreshToken = refreshToken.Token,
        //    };
        //}
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public Task<UserLoginResponseDTOs> AuthenticateUserWithRefreshToken(string RequsetRefreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
