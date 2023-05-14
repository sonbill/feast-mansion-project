using System;
namespace feast_mansion_project.Middleware
{
	public class AdminAuthenticationMiddleware
	{
        private readonly RequestDelegate _next;

        public AdminAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Session.GetString("UserId") == null || context.Session.GetString("IsAdmin") != "true")
            {
                context.Response.Redirect("/home");
                return;
            }

            await _next.Invoke(context);
        }
    }
}

