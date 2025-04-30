using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;

namespace WebTechnology.Configurations
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Hien v Tam API", 
                    Version = "v1",
                    Description = "A beautiful API for Hien v Tam application",
                    Contact = new OpenApiContact
                    {
                        Name = "Development Team",
                        Email = "dev@hienvtam.com",
                        Url = new Uri("https://hienvtam.com/support")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    },
                    TermsOfService = new Uri("https://hienvtam.com/terms")
                });

                // XML documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // JWT Bearer Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                });

                // Enable operation sorting
                c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
                
                // Enable enum schema filtering to show enums as strings
                c.SchemaFilter<EnumSchemaFilter>();
                
                // Enable pretty printing
                c.UseInlineDefinitionsForEnums();
                
                // Custom operation IDs
                c.CustomOperationIds(apiDesc => 
                    $"{apiDesc.ActionDescriptor.RouteValues["action"]}_{apiDesc.HttpMethod}");
            });

            // Configure Swagger UI options
            services.Configure<SwaggerUIOptions>(options =>
            {
                options.DocumentTitle = "Hien v Tam API Documentation";
                options.DefaultModelsExpandDepth(-1); // Hide schemas by default
                options.DocExpansion(DocExpansion.List); // Collapse operations by default
                options.EnableDeepLinking();
                options.DisplayOperationId();
                options.DisplayRequestDuration();
                options.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
                {
                    ["activated"] = true,
                    ["theme"] = "agate"
                };
                options.InjectStylesheet("/swagger-ui/custom-swagger.css"); // Add custom CSS
                options.InjectJavascript("/swagger-ui/custom.js"); // Add custom JS
            });
        }
    }

    // Helper class for enum schema filtering
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                Enum.GetNames(context.Type)
                    .ToList()
                    .ForEach(name => schema.Enum.Add(new OpenApiString(name)));
                schema.Type = "string";
                schema.Format = null;
            }
        }
    }
}