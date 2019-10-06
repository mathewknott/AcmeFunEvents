using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AcmeFunEvents.Web.Middleware
{
    public class SwaggerAuthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IHostingEnvironment _hostingEnvironment;

        public SwaggerAuthorizedMiddleware(RequestDelegate next, IHostingEnvironment env)
        {
            _next = next;
            _hostingEnvironment = env;
        }

        //[Authorize(Policy = "RequireAdministratorRole")]
        public async Task Invoke(HttpContext context)
        {
            if (!_hostingEnvironment.IsDevelopment())
            {
                //if (context.Request.Path.StartsWithSegments("/swagger") && !context.User.Identity.IsAuthenticated)
                //{
                //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //    return;
                //}
            }
            await _next.Invoke(context);
        }
    }
}