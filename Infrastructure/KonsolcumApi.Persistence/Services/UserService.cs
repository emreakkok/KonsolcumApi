using Azure.Core;
using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.DTOs.User;
using KonsolcumApi.Application.Exceptions;
using KonsolcumApi.Application.Features.Commands.AppUser.CreateUser;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
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
    public class UserService : IUserService
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly RoleManager<AppRole> _roleManager;
        readonly IEndpointRepository _endpointRepository;


        public UserService(UserManager<AppUser> userManager, IEndpointRepository endpointRepository, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _endpointRepository = endpointRepository;
            _roleManager = roleManager;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUser model)
        {
            // 1. Kullanıcıyı oluştur
            var newUser = new AppUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Email = model.Email,
                NameSurname = model.NameSurname,
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);

            CreateUserResponse response = new() { Succeded = result.Succeeded };

            if (result.Succeeded)
            {
                // 2. Kullanıcı başarıyla oluşturulduysa, otomatik rol ata
                await AssignDefaultUserRole(newUser);

                response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    response.Message += $"{error.Code} - {error.Description}\n";
                }
            }
            return response;
        }
        private async Task AssignDefaultUserRole(AppUser user)
        {
            try
            {
                const string DEFAULT_ROLE_NAME = "Genel User Yetkisi";

                // Rolün var olup olmadığını kontrol et
                if (!await _roleManager.RoleExistsAsync(DEFAULT_ROLE_NAME))
                {
                    // Rol yoksa oluştur
                    var role = new AppRole
                    {
                        Name = DEFAULT_ROLE_NAME,
                        Id = Guid.NewGuid().ToString()
                    };
                    await _roleManager.CreateAsync(role);
                    Console.WriteLine($"'{DEFAULT_ROLE_NAME}' rolü oluşturuldu.");
                }

                // Kullanıcıya rolü ata
                var roleResult = await _userManager.AddToRoleAsync(user, DEFAULT_ROLE_NAME);

                if (roleResult.Succeeded)
                {
                    Console.WriteLine($"'{user.UserName}' kullanıcısına '{DEFAULT_ROLE_NAME}' rolü başarıyla atandı.");
                }
                else
                {
                    Console.WriteLine($"Rol atama hatası: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Otomatik rol atama sırasında hata: {ex.Message}");
                // Hata olsa bile kullanıcı oluşturma işlemi devam etsin
            }
        }


        public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = accessTokenDate.AddSeconds(addOnAccessTokenDate);
                await _userManager.UpdateAsync(user);
            }
            else
                throw new NotFoundUserException();
        }

        public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                byte[] tokenBytes = WebEncoders.Base64UrlDecode(resetToken);
                resetToken = Encoding.UTF8.GetString(tokenBytes);

                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken,newPassword);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }
                else
                    throw new PasswordChangeFailedException();

            }
            
        }

        public async Task<List<ListUser>> GetAllUsersAsync(int page, int size)
        {
            var users = await _userManager.Users.Skip(page*size).Take(size).ToListAsync();

            return users.Select(user => new ListUser
            {
               Id = user.Id,
               Email = user.Email,
               NameSurname = user.NameSurname,
               TwoFactorEnabled = user.TwoFactorEnabled,
               UserName = user.UserName
            }).ToList();
        
        }

        public int TotalUserCount => _userManager.Users.Count();

        public async Task AssignRoleToUser(string userId, string[] roles)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);

            if(user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                await _userManager.AddToRolesAsync(user, roles);
            }
        }

        public async Task<string[]> GetRolesToUserAsync(string userIdOrName)
        {
            AppUser user = await _userManager.FindByIdAsync(userIdOrName);
            if(user == null)
            {
                user = await _userManager.FindByNameAsync(userIdOrName);
            }
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                return userRoles.ToArray();
            }
            return new string[] { };

        }

        public async Task<bool> HasRolePermissionToEndpointAsync(string name, string code)
        {
            var userRoles = await GetRolesToUserAsync(name);
            if (!userRoles.Any())
                return false;

            Endpoint? endpoint = await _endpointRepository.GetEndpointWithRolesByCodeAsync(code);
            if (endpoint == null)
                return false;

            var endpointRoles = endpoint.Roles.Select(r => r.Name);

            var endpointRoleSet = new HashSet<string>(endpointRoles);

            return userRoles.Any(userRole => endpointRoleSet.Contains(userRole));
        }
    }
}
