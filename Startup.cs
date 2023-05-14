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

namespace feast_mansion_project
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
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

