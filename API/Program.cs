using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Repository.Identity;
using Repository.SeedData;
using System.Threading.Tasks;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.StoreContextMigrateAsync();
            await host.IdentityContextMigrateAsync();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
