using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookManager.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        var mainAssemblyName = typeof(Startup).Assembly.GetName().Name;

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = mainAssemblyName,
                Version = "v1",
                Description = "BookManager Sample",
                Contact = new OpenApiContact
                {
                    Name = "Monica Crespo"
                }
            });

            c.DocumentFilter<LowerCaseDocumentFilter>();

            // Add Bearer Auth support to Swagger UI
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header",
                    BearerFormat = "JWT"
                });

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

            // Add Basic Auth support to Swagger UI
            c.AddSecurityDefinition(
                "Basic",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header"
                });

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Basic"
                            }
                        },
                        new string[]{}
                    }
                });

            // TODO: Find a good way with c.OperationFilter<MyIOperationFilter>(); to discriminate between different auth schemes to apply one or another

        });

        return services;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
internal class LowerCaseDocumentFilter
    : IDocumentFilter
{
    private static string LowercaseEverythingButParameters(string key) => string.Join('/', key.Split('/').Select(x => x.Contains("{") ? x : x.ToLower()));

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.ToDictionary(entry => LowercaseEverythingButParameters(entry.Key),
            entry => entry.Value);
        swaggerDoc.Paths = new OpenApiPaths();
        foreach (var (key, value) in paths)
        {
            swaggerDoc.Paths.Add(key, value);
        }
    }
}