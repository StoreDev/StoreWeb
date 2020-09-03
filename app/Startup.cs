using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using StoreWeb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using XboxWebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.HttpOverrides;

namespace StoreWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Microsoft";
            })
            .AddCookie()
            .AddOAuth("Microsoft", options =>
            {
                var scope = Configuration.GetSection("Microsoft:Scope").Get<List<string>>();
                options.ClientId = Configuration["Microsoft:ClientId"];
                options.ClientSecret = Configuration["Microsoft:ClientSecret"];
                foreach (var scopeVal in scope)
                {
                    options.Scope.Add(scopeVal);
                }
                options.CallbackPath = new PathString("/signin-microsoft");

                options.AuthorizationEndpoint = "https://login.live.com/oauth20_authorize.srf";
                options.TokenEndpoint = "https://login.live.com/oauth20_token.srf";
                options.UserInformationEndpoint = "https://api.github.com/user";

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "xid");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "gtg");
                options.ClaimActions.MapJsonKey(ClaimTypes.Hash, "uhs");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var expiresIn = context.ExpiresIn.GetValueOrDefault(TimeSpan.FromMinutes(3600));
                        var wlResponse = new XboxWebApi.Authentication.Model.WindowsLiveResponse()
                        {
                            /*
                            * HACK: We prefix token with d= to make it pass UserAuthentication
                            */
                            AccessToken = "d=" + context.AccessToken,
                            TokenType = context.TokenType,
                            ExpiresIn = (int)expiresIn.TotalMinutes,
                            RefreshToken = context.RefreshToken,
                            UserId = "",
                            Scope = context.Options.Scope.ToString()
                        };

                        var accessToken = new XboxWebApi.Authentication.AccessToken(wlResponse);
                        var userToken = await XboxWebApi.Authentication.AuthenticationService.AuthenticateXASUAsync(accessToken);
                        var xtoken = await XboxWebApi.Authentication.AuthenticationService.AuthenticateXSTSAsync(userToken);

                        // First serialize with Newtonsoft, then deserialize with System.Text.Json again...
                        var userinfoJson = JsonConvert.SerializeObject(xtoken.UserInformation);
                        using (JsonDocument document = JsonDocument.Parse(userinfoJson))
                        {
                            context.RunClaimActions(document.RootElement);
                        }
                    }
                };
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });

            services.AddDbContext<StoreWebContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("StoreWebContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
