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


        public async Task<UserLoginResponseDTO> AuthenticateUser(User user, bool isRefreshToken)
        {
            string jwtToken = GenerateJwtToken(user);

            if (!isRefreshToken) {
                user.RefreshToken = GenerateRefreshToken();
                user.TokenExpires = DateTime.Now.AddDays(7);
               //user.TokenExpires = _expiryTimesStamp;
            }

            await _userRepository.Update(user);

            return new UserLoginResponseDTO
            {
                Email = user.Email,
                AccessToken = jwtToken,
                TokenExpires = (int)_expiryTimesStamp.Subtract(DateTime.Now).TotalSeconds,
                RefreshToken = user.RefreshToken,
            };
        }
        public string GenerateJwtToken(User user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, Enums.GetRoleName(user.Role))
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

            return jwtToken;
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
