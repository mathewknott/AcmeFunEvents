using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.Extensions;
using AcmeFunEvents.Web.Helpers;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models.Configuration;
using AcmeFunEvents.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace AcmeFunEvents.Web
{
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        public IConfigurationRoot Configuration { get; }
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
            _hostingEnvironment = env;
            Configuration = builder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // *If* you need access to generic IConfiguration this is **required**
            //services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton<IStringLocalizerFactory, SingleFileResourceManagerStringLocalizerFactory>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");

           // services.AddEntityFramework();
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(Configuration.GetConnectionString("dataContext")));
            
            services.ConfigureMvc();
            services.ConfigureLocalization();

            services.AddSingleton(ctx => FileHelpers.ResolveFileProvider(ctx.GetService<IHostingEnvironment>())); //for image controller
            services.AddSingleton(_hostingEnvironment.ContentRootFileProvider);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IHeaderService, HeaderService>();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<IRegistrationService, RegistrationService>();
            services.AddTransient<IUserService, UserService>();

            services.AddScoped<LookupCacheService>();

            services.AddResponseCaching();
            services.AddMemoryCache();

            services.ConfigureHsts();
            services.ConfigureSwagger();
            services.ConfigureSession();

            // Add application services.
            services.AddTransient<IEmailSender, MessagingServices>();
            
            services.Configure<AppOptions>(Configuration.GetSection("App"));
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext context, IOptions<AppOptions> optionsAccessor)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //context.Database.Migrate(); //this will generate the db
            }
            else
            {
                //HSTS is a protocol that instructs browsers to access the site via HTTPS.
                //The protocol has allowances for specifying how long the policy should be
                //enforced (max age) and whether the policy applies to subdomains or not.
                //You can also enable support for your domain to be added to the HSTS preload list.
                app.UseHsts();
            }

            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            app.UseSecurityHeadersMiddleware(new SecurityHeadersBuilder()
                .AddDefaultSecurePolicy()
                .AddCustomHeader("Version", optionsAccessor.Value.Version)
                .AddCustomHeader("X-Robots-Tag", optionsAccessor.Value.Robots)
            );

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.UseResponseCaching();

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                   const int durationInSeconds = 60 * 60 * 24;
                   ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
                }
            });

            app.UseStaticFiles();

            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
            //app.UseSwaggerAuthorized();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Acme Fun Events API");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "errorCode",
                    "error/{statusCode}",
                    new
                    {
                        controller = "Home",
                        action = "Error",
                    });
                routes.MapRoute("default","{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}