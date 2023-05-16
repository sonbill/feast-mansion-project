using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using feast_mansion_project.Models.DTO;
using feast_mansion_project.Models.Domain;
using feast_mansion_project.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace feast_mansion_project.Repositories
{

    public class AccountRepository : Controller, IAccountRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;



        public AccountRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, INotificationService notificationService
)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;


        }

        public async Task<User> GetUserById(int userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.userId == userId);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("userId", user.userId.ToString());
                    _httpContextAccessor.HttpContext.Session.SetString("Username", user.Username);
                    _httpContextAccessor.HttpContext.Session.SetString("IsAdmin", user.IsAdmin ? "true" : "false");
                    _httpContextAccessor.HttpContext.Session.SetString("Email", user.Email);

                    return user;
                }
            }
            return null;
        }




        //public async Task<User> AuthenticateAsync(string email, string password)
        //{
        //    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        //    try
        //    {
        //        if (user != null)
        //        {
        //            var passwordHasher = new PasswordHasher<User>();
        //            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

        //            if (result == PasswordVerificationResult.Success)
        //            {
        //                _httpContextAccessor.HttpContext.Session.SetString("userId", user.userId.ToString());
        //                _httpContextAccessor.HttpContext.Session.SetString("Username", user.Username);
        //                _httpContextAccessor.HttpContext.Session.SetString("IsAdmin", user.IsAdmin ? "true" : "false");
        //                _httpContextAccessor.HttpContext.Session.SetString("Email", user.Email);

        //                // Set the IsAdmin property based on the value of IsAdmin
        //                //user.IsAdmin = user.IsAdmin ?? false;

        //                TempData["SuccessMessage"] = "User authenticated successfully.";

        //                return user;
        //            }
        //        }
        //    } catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Error authenticating user: " + ex.Message;

        //        return null;
        //    }
        //    TempData["ErrorMessage"] = "Invalid email or password.";

        //    return null;
        //}

        public async Task LogoutAsync()
        {
            await Task.Run(() =>
            {
                _httpContextAccessor.HttpContext.Session.Clear();
            });
        }
    }
}

