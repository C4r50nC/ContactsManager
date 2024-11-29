using ContactsManager.Infrastructure.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsManager.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Find service descripter for AddDbContext() which uses SQL server from Program.cs
                ServiceDescriptor? descripter = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                // Remove AddDbContext() service to disable database connection
                if (descripter != null)
                {
                    services.Remove(descripter);
                }
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("AspNetCrudTesting");
                });
            });
        }
    }
}
