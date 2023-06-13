namespace BookManager.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseOpenApi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Book Manager Sample"));
    }
}