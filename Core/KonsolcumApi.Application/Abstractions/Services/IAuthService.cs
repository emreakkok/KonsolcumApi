using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Abstractions.Services
{
    public interface IAuthService
    {
        Task<DTOs.Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime);
        Task<DTOs.Token> RefreshTokenLoginAsync(string refreshToken);

        Task PasswordResetAsync(string email);

        Task<bool> VerifyResetTokenAsync(string resetToken, string userId);

    }
}
