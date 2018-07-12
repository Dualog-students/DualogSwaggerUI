using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwaggerUI.AuthServer;

namespace SwaggerUI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOAuthAuthentication();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDualogSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Map("/auth-server", authServer =>
            {
                authServer.UseAuthentication();
                authServer.UseIdentityServer();
                authServer.UseMvc();
            });

            app.Map("/resource-server", resourceServer =>
            {
                resourceServer.UseAuthentication();
                resourceServer.UseMvc();

                // IMPORTANT: to be able to serve the custom index html
                resourceServer.UseStaticFiles();

                // Use the dualog swagger 
                resourceServer.UseDualogSwagger();
            });
        }
    }
}
