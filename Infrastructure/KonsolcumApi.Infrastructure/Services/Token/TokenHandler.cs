using KonsolcumApi.Application.Abstractions.Token;
using KonsolcumApi.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;
        readonly UserManager<AppUser> _userManager;

        public TokenHandler(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public Application.DTOs.Token CreateAccessToken(int second, AppUser appUser)
        {
            Application.DTOs.Token token = new();

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            token.Expiration = DateTime.UtcNow.AddSeconds(second);

            // Kullanıcının rollerini senkron al (async'i bloklayarak)
            var roles = _userManager.GetRolesAsync(appUser).GetAwaiter().GetResult();

            // Temel claim'leri ekle
            List<Claim> claims = new()
    {
        new Claim(ClaimTypes.Name, appUser.UserName)
    };

            // Rolleri de claim olarak ekle
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            JwtSecurityToken securityToken = new(
                audience: _configuration["Token:Audience"],
                issuer: _configuration["Token:Issuer"],
                expires: token.Expiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                claims: claims
            );

            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(securityToken);
            token.RefreshToken = CreateRefreshToken();

            return token;
        }


        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(number);
            return Convert.ToBase64String(number);
        }
    }
}
// rolleri alıp sidebarı denemek için değiştirdim eski hali
//using KonsolcumApi.Application.Abstractions.Token;
//using KonsolcumApi.Domain.Entities.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace KonsolcumApi.Infrastructure.Services.Token
//{
//    public class TokenHandler : ITokenHandler
//    {
//        readonly IConfiguration _configuration;

//        public TokenHandler(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public Application.DTOs.Token CreateAccessToken(int second , AppUser appUser)
//        {
//            Application.DTOs.Token token = new();

//            // sec keyin simetriğini alıyoruz
//            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

//            //şifrelenmiş kimliği oluşturuyoruz.
//            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

//            //oluşturulacak token ayarlarını veriyoruz
//            token.Expiration = DateTime.UtcNow.AddSeconds(second);
//            JwtSecurityToken securityToken = new(
//                audience: _configuration["Token:Audience"],
//                issuer: _configuration["Token:Issuer"],
//                expires: token.Expiration,
//                notBefore: DateTime.UtcNow,
//                signingCredentials: signingCredentials,
//                claims: new List<Claim> { new(ClaimTypes.Name,appUser.UserName)}
//                );

//            // token oluşturucu sınıfından bir örnek alalım
//            JwtSecurityTokenHandler tokenHandler = new();
//            token.AccessToken = tokenHandler.WriteToken(securityToken);

//            token.RefreshToken = CreateRefreshToken();

//            return token;

//        }

//        public string CreateRefreshToken()
//        {
//            byte[] number = new byte[32];
//            using RandomNumberGenerator random = RandomNumberGenerator.Create();
//            random.GetBytes(number);
//            return Convert.ToBase64String(number);
//        }
//    }
//}
