using AcmeFunEvents.Web.Helpers;
using AcmeFunEvents.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AcmeFunEvents.Web.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app, SecurityHeadersBuilder builder)
        {
            var policy = builder.Build();
            return app.UseMiddleware<SecurityHeadersMiddleware>(policy);
        }
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerAuthorizedMiddleware>();
        }
    }
}