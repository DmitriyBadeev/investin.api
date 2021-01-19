using System.Collections.Generic;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using InvestIn.Core.Interfaces;
using InvestIn.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InvestIn.Identity.Services
{
    public class SeedService : ISeedDataService
    {
        private readonly ILogger<SeedService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly IConfiguration _configuration;

        public SeedService(ILogger<SeedService> logger, UserManager<ApplicationUser> userManager,
            ApplicationDbContext applicationDbContext, PersistedGrantDbContext persistedGrantDbContext,
            ConfigurationDbContext configurationDbContext, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _configurationDbContext = configurationDbContext;
            _configuration = configuration;
        }

        public void Initialise()
        {
            _logger.LogInformation("Seeding database");
            
            if (_applicationDbContext.Database.GetPendingMigrations().Any())
            {
                _logger.LogInformation("Migrating identity database");
                _applicationDbContext.Database.Migrate();
                _logger.LogInformation("Database identity has migrated successfully");
            }

            if (_persistedGrantDbContext.Database.GetPendingMigrations().Any())
            {
                _logger.LogInformation("Migrating persisted grant database");
                _persistedGrantDbContext.Database.Migrate();
                _logger.LogInformation("Database persisted grant has migrated successfully");
            }

            if (_configurationDbContext.Database.GetPendingMigrations().Any())
            {
                _logger.LogInformation("Migrating configuration database");
                _configurationDbContext.Database.Migrate();
                _logger.LogInformation("Database configuration has migrated successfully");
            }
            
            var email = _configuration["email"];
            var password = _configuration["password"];
            
            var financeClientId = _configuration["financeClientId"];
            var financeRedirects = _configuration.GetSection("finance_redirect_uris").Get<List<string>>();
            var apiFinance = _configuration["apiFinance"];
            
            if (!_configurationDbContext.Clients.Any())
            {
                foreach (var client in Config.GetSpaClient(financeClientId, financeRedirects, apiFinance))
                {
                    _configurationDbContext.Clients.Add(client.ToEntity());
                }
                _configurationDbContext.SaveChanges();
            }

            if (!_configurationDbContext.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    _configurationDbContext.IdentityResources.Add(resource.ToEntity());
                }
                _configurationDbContext.SaveChanges();
            }
            
            if (!_configurationDbContext.ApiResources.Any())
            {
                foreach (var resource in Config.GetApiResources(apiFinance))
                {
                    _configurationDbContext.ApiResources.Add(resource.ToEntity());
                }
                _configurationDbContext.SaveChanges();
            }
            
            var user = _userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
            
            if (user == null)
            {
                _logger.LogInformation("Creating user");
                
                var userEntity = new ApplicationUser
                {
                    Email = email,
                    UserName = "Dmitriy Badeev",
                    EmailConfirmed = true,
                };
                
                var result = _userManager.CreateAsync(userEntity, password).GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created");
                }
            }
            else
            {
                _logger.LogInformation("User already created");
            }
            
            _logger.LogInformation("Database has seeded successfully");
        }
    }
}