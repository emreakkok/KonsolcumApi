using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.Abstractions.Token;
using KonsolcumApi.Application.DTOs;
using KonsolcumApi.Application.Exceptions;
using KonsolcumApi.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Services
{

    public class AuthService : IAuthService
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly SignInManager<Domain.Entities.Identity.AppUser> _signInManager;
        readonly IUserService _userService;
        readonly IMailService _mailService;

        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, SignInManager<AppUser> signInManager, IUserService userService, IMailService mailService)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _userService = userService;
            _mailService = mailService;
        }

        public async Task<Token> LoginAsync(string usernameOrEmail, string password , int accessTokenLifeTime)
        {
            Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(usernameOrEmail);
            }

            if (user == null)
                throw new NotFoundUserException("Kullanıcı veya şifre hatalı...");


            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (result.Succeeded)
            {
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime, user);

                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 300);
                return token;

            }
            //return new LoginUserErrorCommandResponse() 
            //{ 
            //    Message = "Kullanıcı adı veya şifre hatalı..."
            //};
            throw new AuthenticationErrorException();
        }

        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user != null && user?.RefreshTokenEndDate > DateTime.UtcNow)
            {
                Token token = _tokenHandler.CreateAccessToken(900, user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 300);
                return token;
            }
            else
                throw new NotFoundUserException();
        }

        public async Task PasswordResetAsync(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if(user != null)
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                byte[] tokenBytes = Encoding.UTF8.GetBytes(resetToken);
                resetToken = WebEncoders.Base64UrlEncode(tokenBytes);

                await _mailService.SendPasswordResetMailAsync(email, user.Id, resetToken);
            }
        }

        public async Task<bool> VerifyResetTokenAsync(string resetToken, string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                byte[] tokenBytes = WebEncoders.Base64UrlDecode(resetToken);
                resetToken = Encoding.UTF8.GetString(tokenBytes);

                return await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);
            }
            return false;

        }
    }
}
