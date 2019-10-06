using System;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AcmeFunEvents.Test
{
    public class ServicesBase
    {
        public readonly IServiceProvider ServiceProvider;

        public ServicesBase()
        {
            var services = new ServiceCollection();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRegistrationService, RegistrationService>();

            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddOptions();
            services.AddDbContext<ApplicationDbContext>(b => b.UseInMemoryDatabase("Scratch").UseInternalServiceProvider(efServiceProvider));
            services.AddMvc();
            services.AddLogging();

            // IHttpContextAccessor is required for SignInManager, and UserManager
            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor
                {
                    HttpContext = context,
                });

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}