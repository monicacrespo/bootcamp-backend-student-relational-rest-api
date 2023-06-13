namespace BookManager
{
    using BookManager.Extensions;
    using BookManager.Application;
    using BookManager.Persistence.SqlServer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Dependency Injection Container
        public void ConfigureServices(IServiceCollection services)
        {
            var booksConnectionString =
                _configuration.GetValue<string>("ConnectionStrings:BooksDatabase");           

            services
                .AddTransient<IBookManagerService,BookManagerService>()
                .AddDbContext<BookManagerDbContext>(options =>
                {
                     options.UseSqlServer(booksConnectionString);
                })
                .AddScoped<IBookManagerDbContext, BookManagerDbContext>()
                .AddOpenApi()
                .AddControllers();
        }
        
        // Middleware pipeline
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseOpenApi();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}