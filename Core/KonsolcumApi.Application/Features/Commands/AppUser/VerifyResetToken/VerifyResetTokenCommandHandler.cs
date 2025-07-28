using KonsolcumApi.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.AppUser.VerifyResetToken
{
    public class VerifyResetTokenCommandHandler : IRequestHandler<VerifyResetTokenCommandRequest, VerifyResetTokenCommandResponse>
    {
        readonly IAuthService _authService;

        public VerifyResetTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<VerifyResetTokenCommandResponse> Handle(VerifyResetTokenCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Debug için log ekle
                Console.WriteLine($"VerifyResetToken - UserId: {request.UserId}, ResetToken: {request.ResetToken?.Substring(0, Math.Min(20, request.ResetToken.Length))}...");

                bool state = await _authService.VerifyResetTokenAsync(request.ResetToken, request.UserId);

                Console.WriteLine($"VerifyResetToken sonucu: {state}");

                return new VerifyResetTokenCommandResponse
                {
                    State = state
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VerifyResetToken hatası: {ex.Message}");
                return new VerifyResetTokenCommandResponse
                {
                    State = false
                };
            }
        }
    }
}
