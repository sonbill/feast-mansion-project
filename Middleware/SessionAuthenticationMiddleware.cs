using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace feast_mansion_project.Middlewares
{
    public class SessionAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userId = context.Session.GetString("UserId");

            if (userId == null)
            {
                context.Response.Redirect("/login");
                return;
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var identity = new ClaimsIdentity(claims, "session");

            context.User = new ClaimsPrincipal(identity);

            await _next(context);
        }
    }
}