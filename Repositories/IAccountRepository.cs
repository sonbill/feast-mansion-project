using System;
using Microsoft.AspNetCore.Identity;
using feast_mansion_project.Models.DTO;
using feast_mansion_project.Models;

namespace feast_mansion_project.Repositories
{
    public interface IAccountRepository
    {
        Task<User> AuthenticateAsync(string email, string password);

        Task<User> GetUserById(int userId);

        Task LogoutAsync();


    }
}

