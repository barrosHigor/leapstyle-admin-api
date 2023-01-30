namespace leap_dashboard_admin_api.Configuracoes
{
    using Domain.Entities;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                var username = credentials[0];
                var password = credentials[1];

                context.Items["User"] = null;
                // authenticate credentials with user service and attach user to http context
                if (username == Environment.GetEnvironmentVariable("UserName") && password == Environment.GetEnvironmentVariable("Password"))
                {
                    context.Items["User"] = new UserCredential()
                    {
                        UserName = username
                    };
                }
            }
            catch
            {
                // do nothing if invalid auth header
                // user is not attached to context so request won't have access to secure routes
            }

            await _next(context);
        }
    }
}
