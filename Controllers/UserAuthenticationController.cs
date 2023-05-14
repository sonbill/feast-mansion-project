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


        public UserAuthenticationController(     
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IAccountRepository accountRepository
            )
        {    
            _dbContext = dbContext;
            context = httpContextAccessor;
            _accountRepository = accountRepository;

        }

        //public static string GetMD5(string str)
        //{
        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] fromData = Encoding.UTF8.GetBytes(str);
        //    byte[] targetData = md5.ComputeHash(fromData);
        //    string byte2String = null;

        //    for (int i = 0; i < targetData.Length; i++)
        //    {
        //        byte2String += targetData[i].ToString("x2");
        //    }
        //    return byte2String;
        //}

        // GET: /<controller>/
        public IActionResult Register()
        {

            //ClaimsPrincipal claimsUser = HttpContext.User;

            //if (claimsUser.Identity.IsAuthenticated)
            //    return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkUser = _dbContext.Users.FirstOrDefault(s => s.Email == model.Email);
                if (checkUser == null)
                {
                    var user = new User
                    {
                        Username = model.Username,
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
                        Address = model.Address,
                        Phone = model.Phone,
                        CreatedAt = DateTime.Now,
                        User = user
                    };

                    _dbContext.Customers.Add(customer);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
            }
            Console.WriteLine(JsonConvert.SerializeObject(model));
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            //ClaimsPrincipal claimsUser = HttpContext.User;

            //if (claimsUser.Identity.IsAuthenticated)
            //    return RedirectToAction("Index", "Home");
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
                    HttpContext.Session.SetString("UserId", user.userId.ToString());
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
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }
        

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _accountRepository.LogoutAsync();

            return RedirectToAction("Login");
        }
    }
}

