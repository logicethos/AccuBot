using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccuBot.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AccuBot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    //Use the below line to change the default directory
                    //for your Razor Pages.
               //     options.RootDirectory = "/wwwroot";
                
                    //Use the below line to change the default
                    //"landing page" of the application.
              //      options.Conventions.AddPageRoute("/wwwroot/Index.razor", "");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                //endpoints.MapControllers();
                endpoints.MapGrpcService<ApiService>();
               // endpoints.MapFallbackToPage("/Index.razor");
                

                /*endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });*/
            });
            
        }
    }
}