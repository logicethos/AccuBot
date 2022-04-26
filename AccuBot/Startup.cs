using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccuBot.GRPC;
using LettuceEncrypt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace AccuBot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtAuthenticationManager.JWT_TOKEN_KEY)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            services.AddAuthorization();
            services.AddGrpc();
            /*services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    //Use the below line to change the default directory
                    //for your Razor Pages.
                    options.RootDirectory = Path.Combine("/",Program.DataPath, "www");
                
                    //Use the below line to change the default
                    //"landing page" of the application.
                    //options.Conventions.AddPageRoute(Path.Combine(Program.DataPath, "www","index.razor") , "");
              
                  
                });*/


            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));

            services.AddLettuceEncrypt(c =>
                {
                    c.DomainNames = new[] { "red2.logicethos.com" };
                    c.EmailAddress = "stuart@logicethos.com";
                    c.AcceptTermsOfService = true;
                    c.RenewDaysInAdvance = TimeSpan.FromDays(3);
//                    c.RenewalCheckPeriod = TimeSpan.FromSeconds(30);
#if DEBUG
                    c.UseStagingServer = true;
#endif
                })
                .PersistDataToDirectory(new DirectoryInfo(Path.Combine(Program.DataPath,"certs")),null);

        //    services.AddControllers();
         //   services.AddRouting();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseRouting();
            app.UseGrpcWeb(); 
            app.UseCors();


            if (!String.IsNullOrEmpty(Program.DataPathWWW) &&  Directory.Exists(Program.DataPathWWW))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Program.DataPathWWW),
                    RequestPath = ""
                });
            }
            else
            {
                Log.Error($"WWW directory does not exist ({Program.DataPathWWW})");
            }

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
             //   endpoints.MapRazorPages();
             //   endpoints.MapControllers();
                endpoints.MapGrpcService<AuthenticationService>().EnableGrpcWeb().RequireCors("AllowAll");
                endpoints.MapGrpcService<ApiService>().EnableGrpcWeb().RequireCors("AllowAll");
                
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