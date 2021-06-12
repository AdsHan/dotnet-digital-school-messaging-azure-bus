﻿using MED.Auth.Domain.Entities;
using MED.Core.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MED.Auth.Domain.Repositories
{
    public interface IUserRepository : IRepository<UserModel>
    {
        Task<UserModel> GetByUserNameAsync(string userName);
        Task<IdentityResult> CreateAsync(UserModel usiario, string userName);
        Task<SignInResult> SignInAsync(string userName, string password);
        Task<bool> CheckPasswordAsync(string userName, string password);
    }
}
