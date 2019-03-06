using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace Atto.Common.Core.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddDefaultSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(configuration.GetValue("swagger:version", "v1"),
                    new Info
                    {
                        Title = configuration.GetValue("swagger:title", "Title"),
                        Version = configuration.GetValue("swagger:version", "v1"),
                        Description = configuration.GetValue("swagger:description", "App Description"),
                        Contact = new Contact
                        {
                            Name = "AttoSoft",
                            Url = "https://github.com/atto-soft"
                        }
                    });

                string applicationPath = Directory.GetCurrentDirectory();
                string applicatioName = Assembly.GetEntryAssembly().GetName().Name;
                string caminhoXmlDoc = Path.Combine(applicationPath, $"{applicatioName}.xml");
                if (File.Exists(caminhoXmlDoc))
                    c.IncludeXmlComments(caminhoXmlDoc);
            });

            return services;
        }

        public static IApplicationBuilder UseDefaultSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "App"));

            return app;
        }
    }
}