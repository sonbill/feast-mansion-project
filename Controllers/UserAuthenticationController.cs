using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using feast_mansion_project.Models.Domain;
using feast_mansion_project.Models.DTO;
using feast_mansion_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using feast_mansion_project.Controllers;
using Microsoft.AspNetCore.Authorization;
using feast_mansion_project.Repositories;
using feast_mansion_project.Helper;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor context;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;


        public UserAuthenticationController(     
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IAccountRepository accountRepository,
            INotificationService notificationService
            )
        {    
            _dbContext = dbContext;
            context = httpContextAccessor;
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }        

        // GET: /<controller>/
        public IActionResult Register()
        {
            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            if (currentUser != 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkUser = _dbContext.Users.FirstOrDefault(s => s.Email == model.Email);
                try
                {
                    if (checkUser == null)
                    {
                        var user = new User
                        {
                            //Username = model.Username,
                            Email = model.Email,
                            IsAdmin = false
                        };
                        //user.Password = GetMD5(user.Password);
                        var passwordHasher = new PasswordHasher<User>();
                        user.Password = passwordHasher.HashPassword(user, model.Password);

                        _dbContext.Users.Add(user);
                        await _dbContext.SaveChangesAsync();

                        var customer = new Customer
                        {
                            FullName = model.FullName,
                            Address = model.Address + " Tp. Hồ Chí Minh",
                            Phone = model.Phone,
                            CreatedAt = DateTime.Now,
                            User = user
                        };

                        _dbContext.Customers.Add(customer);

                        await _dbContext.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Đăng ký tài khoản thành công.";

                        return RedirectToAction("Login", "UserAuthentication");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Email đã tồn tại, vui lòng chọn email khác.";

                        return RedirectToAction("Register", "UserAuthentication");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error authenticating user: " + ex.Message;
                }
            }
            //Console.WriteLine(JsonConvert.SerializeObject(model));

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            if (currentUser != 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _accountRepository.AuthenticateAsync(model.Email, model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("IsAdmin", user.IsAdmin ? "true" : "false");
                    //HttpContext.Session.SetUserId(user.userId);
                    //HttpContext.Session.SetIsAuthenticated(true);

                    if (user.IsAdmin == true)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(model);
        }



        public async Task<IActionResult> Logout()
        {
            await _accountRepository.LogoutAsync();

            return RedirectToAction("Login");
        }
    }
}

