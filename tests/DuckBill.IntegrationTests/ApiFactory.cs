using DuckBill.Domain.Entities;
using DuckBill.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DuckBill.IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Authentication:Enabled"] = "true",
                ["Authentication:ApiKey"] = "test-key"
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DuckBillDbContext>));
            if (dbDescriptor != null)
            {
                services.Remove(dbDescriptor);
            }

            services.AddDbContext<DuckBillDbContext>(options =>
            {
                options.UseInMemoryDatabase("DuckBillIntegrationTests");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DuckBillDbContext>();
            db.Database.EnsureCreated();

            if (!db.Usuarios.Any())
            {
                db.Usuarios.Add(new Usuario { Nome = "Ana", Email = "ana@duckbill.com", Senha = "123" });
                db.SaveChanges();
            }
        });
    }
}
