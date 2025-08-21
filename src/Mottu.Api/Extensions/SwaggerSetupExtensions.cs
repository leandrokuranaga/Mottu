using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Mottu.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class SwaggerSetupExtensions
    {
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Mottu",
                    Version = "v1",
                    Description = "Mottu - API to manage motorcycle rents"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
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

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var groupName = apiDesc.GroupName;
                    return groupName == docName;
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

                c.ExampleFilters();
                c.OperationFilter<SetApplicationJsonAsDefaultFilter>();
                c.EnableAnnotations();
                c.SchemaFilter<SuccessResponseSchemaFilter>();
            });
            services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
        }
        public static void UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mottu API v1");
                c.RoutePrefix = "swagger";
            });
        }
    }
}
