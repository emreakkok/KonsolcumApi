﻿using KonsolcumApi.Application.DTOs.User;
using KonsolcumApi.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KonsolcumApi.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateAsync(CreateUser model);

        Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate);
    
        Task UpdatePasswordAsync(string userId, string resetToken,string newPassword);

        Task<List<ListUser>> GetAllUsersAsync(int page,int size);

        int TotalUserCount {  get; }

        Task AssignRoleToUser(string userId, string[] roles);

        Task<string[]> GetRolesToUserAsync(string userIdOrName);
        Task <bool> HasRolePermissionToEndpointAsync(string name, string code);

    }
}
