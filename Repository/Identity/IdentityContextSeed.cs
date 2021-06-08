using Entities.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Identity
{
    public static class IdentityContextSeed
    {
        public static async Task<IHost> IdentityContextMigrateAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var identityContext = services.GetRequiredService<AppIdentityDbContext>();
                    await identityContext.Database.MigrateAsync();
                    await SeedUserAsync(userManager);
                }
                catch (Exception e)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(e, "An error ocurred during migration");
                }
            }

            return host;
        }

        private static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Chuck",
                    Email = "chuck@hotmail.com",
                    UserName = "chuck@hotmail.com",
                    Address = new Address
                    {
                        FirstName = "Chuck",
                        LastName = "Norris",
                        Street = "Roundhouse Kick 15",
                        City = "My City",
                        State = "Owner",
                        ZipCode = "01010"

                    }
                };

                await userManager.CreateAsync(user, "!23chucK");
            }
        }
    }
}
