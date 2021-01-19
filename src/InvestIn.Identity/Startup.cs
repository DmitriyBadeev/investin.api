using System;
using System.Reflection;
using System.Security.Claims;
using InvestIn.Core.Interfaces;
using InvestIn.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InvestIn.Identity.Services;
using Microsoft.AspNetCore.Authentication;

namespace InvestIn.Identity
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<ApplicationDbContext>(config =>
                {
                    config.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                })
                .AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredLength = 6;
                    config.User.RequireUniqueEmail = true;
                    config.User.AllowedUserNameCharacters = 
                        config.User.AllowedUserNameCharacters.Insert(0, "абвгдеёжзийклмнопрстуфхцчшщъыьэюя АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ");
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            var rsa = new RsaKeyService(_environment, TimeSpan.FromDays(120));
            services.AddSingleton(provider => rsa);
            services.AddScoped<ISeedDataService, SeedService>();
            services.AddMvc();
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services
                .AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddSigningCredential(rsa.GetKey())
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600 * 10;
                });
            
                services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        options.ClientId = Configuration["google_client_id"];
                        options.ClientSecret = Configuration["google_secret"];
                        options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                        options.ClaimActions.Clear();
                        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                        options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                        options.ClaimActions.MapJsonKey("urn:google:profile", "link");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
