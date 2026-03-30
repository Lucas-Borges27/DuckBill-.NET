using DuckBill.Domain.Entities;
using DuckBill.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;

namespace DuckBill.IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"DuckBillIntegrationTests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Authentication:Enabled"] = "true",
                ["Authentication:ApiKey"] = "test-key",
                ["Integrations:AwesomeApi:BaseUrl"] = "https://awesomeapi.test/"
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<DuckBillDbContext>));
            services.RemoveAll(typeof(IHttpClientFactory));

            services.AddDbContext<DuckBillDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            services.AddSingleton<IHttpClientFactory>(new FakeHttpClientFactory());
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DuckBillDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        db.Usuarios.Add(new Usuario { Nome = "Ana", Email = "ana@duckbill.com", Senha = "123" });
        await db.SaveChangesAsync();
    }

    private sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient(new FakeSuccessHandler())
            {
                BaseAddress = new Uri("https://awesomeapi.test/")
            };
        }
    }

    private sealed class FakeSuccessHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = new StringContent("{\"status\":\"ok\"}", Encoding.UTF8, "application/json");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });
        }
    }
}
