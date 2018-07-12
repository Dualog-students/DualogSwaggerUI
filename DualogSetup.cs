using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SwaggerUI.ResourceServer.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SwaggerUI
{
    static class DualogSetup
    {

        internal static IApplicationBuilder UseDualogSwagger(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger((options) =>
            {
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    // Add additional metadata to the swagger json file
                    swaggerDoc.Schemes = new List<string>
                    {
                        "http",
                        "https"
                    };

                    swaggerDoc.Produces = new List<string>
                    {
                        "application/json"
                    };

                    swaggerDoc.Consumes = new List<string>
                    {
                        "application/json"
                    };

                    // Use the http context to add the current host
                    swaggerDoc.Host = httpReq.Host.Value;
                });
            });

            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "Dualog.net API Docs";

                // Enables a search bar for the "SwaggerTagAttribute"
                options.EnableFilter();

                // Enables a swagger config validator
                options.EnableValidator();

                options.DisplayRequestDuration();

                // Collapses all controllers/endpoints in the view
                options.DocExpansion(DocExpansion.None);

                // Removes the "models" section from the end of the view
                //options.DefaultModelsExpandDepth(-1);

                // Define the endpoint for serving the swagger documentation
                options.SwaggerEndpoint("/resource-server/swagger/v1/swagger.json", "My API V1");

                // Add the custom index.html page 
                options.IndexStream = () =>
                    Assembly.GetExecutingAssembly().GetManifestResourceStream("SwaggerUI.Resources.index.html");

                // Set our custom css
                options.InjectStylesheet("/resource-server/swagger-ui/dualog-swagger.css");

                // Additional OAuth settings (app specific, but same concept)
                // (See https://github.com/swagger-api/swagger-ui/blob/v3.10.0/docs/usage/oauth2.md)
                options.OAuthClientId("test-id");
                options.OAuthClientSecret("test-secret");
                options.OAuthRealm("test-realm");
                options.OAuthAppName("test-app");
                options.OAuthScopeSeparator(" ");
                options.OAuthAdditionalQueryStringParams(new { foo = "bar" });
                options.OAuthUseBasicAuthenticationWithAccessCodeGrant();


            });

            return app;
        }


        internal static IServiceCollection AddDualogSwagger(this IServiceCollection services)
        {
            // Add dualog swagger specific
            services.AddSwaggerGen(options =>
            {
                // Add some metadata to the Swagger docs
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Dualog Web API",
                    Description = "This is a documentation for a Dualog API",
                    Contact = new Contact
                    {
                        Name = "Dualog AS",
                        Url = "http://dualog.com"
                    },
                    License = new License
                    {
                        Name = "Licensed under the Apache 2.0 license",
                        Url = "http://www.apache.org/licenses/LICENSE-2.0.html"
                    }
                });

                // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    // Token url can be specified also
                    //TokenUrl = "/auth-server/connect/token",
                    AuthorizationUrl = "/auth-server/connect/authorize",
                    Scopes = new Dictionary<string, string>
                    {
                        { "readAccess", "Access read operations" },
                        { "writeAccess", "Access write operations" }
                    }
                });

                // Assign scope requirements to operations based on AuthorizeAttribute
                options.OperationFilter<SecurityRequirementsOperationFilter>();

                // Or add a global security definition for all endpoints
                // Apply OAuth2 scheme to all operations
                //options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                //{
                //    { "oauth2", new[] { "readAccess", "writeAccess" } }
                //});

                options.DescribeAllEnumsAsStrings();

                // Set the comments path for the Swagger JSON and UI.
                options.IncludeXmlComments(Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "xml"));
            });

            return services;
        }
    }
}
