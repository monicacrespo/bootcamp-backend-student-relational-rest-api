using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using BookManager;
using BookManager.Application;
using BookManager.Persistence.SqlServer;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace BookManager.FunctionalTests.TestSupport { 
    public abstract class IntegrationTest
        : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _uniqueDatabaseName;
        private readonly string? storedUsername;
        private readonly string? storedPassword;
        protected HttpClient HttpClient { get; }
        protected IntegrationTest()
        {
            var server =
                new TestServer(
                    new WebHostBuilder()
                        .UseStartup<Startup>()
                        .UseEnvironment("Test")
                        .UseCommonConfiguration()
                        .ConfigureTestServices(ConfigureTestServices));

            HttpClient = server.CreateClient();

            // Strategy to use a unique DB schema per test execution
            _serviceProvider = server.Services;
            _uniqueDatabaseName = $"Test-{Guid.NewGuid()}";
            // Apply Migrations
            using var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<BookManagerDbContext>();
            dbContext.Database.Migrate();

            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
            storedUsername = configuration.GetValue<string>("BasicAuthentication:Username");
            storedPassword = configuration.GetValue<string>("BasicAuthentication:Password");

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", storedUsername, storedPassword))));
        }

        protected virtual void ConfigureTestServices(IServiceCollection services)
        {
            // Replace EF Core's DbContext to use
            RemoveDependencyInjectionRegisteredService<DbContextOptions<BookManagerDbContext>>(services);

            services.AddDbContext<BookManagerDbContext>(
                (sp, options) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var testConnectionString = configuration.GetValue<string>("ConnectionStrings:BooksDatabase");
                    var parts = testConnectionString!.Split(";");
                    var uniqueDbTestConnectionStringBuilder = new StringBuilder();
                    foreach (var part in parts)
                    {
                        var isDatabasePart = part.StartsWith("Database=");
                        uniqueDbTestConnectionStringBuilder.Append(isDatabasePart
                            ? $"Database={_uniqueDatabaseName};"
                            : $"{part};");
                    }

                    var uniqueDbTestConnectionString = uniqueDbTestConnectionStringBuilder.ToString().TrimEnd(';');
                    options.UseSqlServer(uniqueDbTestConnectionString);
                });            
        }

        private void RemoveDependencyInjectionRegisteredService<TService>(IServiceCollection services)
        {
            var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }
        }

        public void Dispose()
        {
            using var dbContext = _serviceProvider.GetService<BookManagerDbContext>();
            dbContext?.Database.EnsureDeleted();
        }
    }
}
internal static class WebHostBuilderExtensions
{
    internal static IWebHostBuilder UseCommonConfiguration(this IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;

            config
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                config.AddUserSecrets(appAssembly, optional: true);
            }
        });

        return builder;
    }
}