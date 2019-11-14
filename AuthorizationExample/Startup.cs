using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationExample.Data;
using AuthorizationExample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationExample
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
             {
                 options.Password.RequireDigit = true;
                 options.Password.RequiredLength = 7;
                 options.Password.RequireUppercase = true;
                 options.User.RequireUniqueEmail = true;
             })
            .AddEntityFrameworkStores<ApplicationDbContext>();
            //.AddDefaultTokenProviders();
            services.AddAuthentication(
            //    options =>
            //{
              //  options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;// CookieAuthenticationDefaults.AuthenticationScheme;
              //  options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;//CookieAuthenticationDefaults.AuthenticationScheme;
              //  options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;//CookieAuthenticationDefaults.AuthenticationScheme;
            //}
            ).AddFacebook(o =>
            {
                o.AppId = "625804011287173";
                o.AppSecret = "a0dd37fb0422a3af93f65357b443f1ca";
                o.SignInScheme = IdentityConstants.ApplicationScheme;
                o.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents()
                {
                    OnRemoteFailure = ctx =>
                    {
                        ctx.Response.Redirect("/Account/Login");
                        ctx.HandleResponse();
                        return Task.FromResult(0);

                    }
                };
            })
            .AddCookie();

            services.AddAuthorization(c => {
                c.AddPolicy("XX", p => {
                    p.RequireAssertion(a=>
                    {
                        return !a.User.Identity.IsAuthenticated || a.User.Identity.Name.StartsWith("M");
                        
                    });                 
                });
                c.DefaultPolicy = c.GetPolicy("XX");
               
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                  options.UseSqlServer("Data Source=.;Initial Catalog=TestApp;Integrated Security=true;"));



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
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

            app.UseStatusCodePages(async c =>
            {                
                if (c.HttpContext.Response.StatusCode == 404)
                {
                    c.HttpContext.Response.Redirect("Account/AccessDenied");
                    //await Task.Run(() => { });
                    //await c.HttpContext.Response.WriteAsync("Access denied for user " + c.HttpContext.User.Identity.Name);

                }
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
