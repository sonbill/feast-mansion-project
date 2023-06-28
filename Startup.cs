using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using feast_mansion_project.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using feast_mansion_project.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using feast_mansion_project.Models.Domain;
using feast_mansion_project.Models;
using Microsoft.AspNetCore.Identity;
using feast_mansion_project.Middlewares;
using feast_mansion_project.Middleware;
using feast_mansion_project.Repositories;
using Microsoft.Extensions.Options;


using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace feast_mansion_project
{
    public class CustomRequestCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); // Time zone ID for Vietnam
            var currentDateTimeUtc = DateTime.UtcNow;
            var vietnamDateTime = TimeZoneInfo.ConvertTimeFromUtc(currentDateTimeUtc, vietnamTimeZone);

            var cultureResult = new ProviderCultureResult("vi-VN", "vi-VN");

            httpContext.Items["TimeZoneInfo"] = vietnamTimeZone;
            httpContext.Items["VietnamDateTime"] = vietnamDateTime;

            return Task.FromResult(cultureResult);           
        }
    }

    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));


            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;               
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("vi-VN") // Replace "en-US" with the culture you want to use
                };

                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); // Time zone ID for Vietnam

                options.DefaultRequestCulture = new RequestCulture("vi-VN", "vi-VN"); // Replace "en-US" with the culture you want to use
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //app.UseMiddleware<AuthenticationMiddleware>();
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseMiddleware<SessionAuthenticationMiddleware>();

            app.UseMiddleware<AdminAuthenticationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "category",
                    pattern: "Admin/Category/{action=Index}/{id?}",
                    defaults: new { controller = "Category" });
            });
        }
    }
}

