using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace MiddlewareExample
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseMyAuthentication(this IApplicationBuilder app)
        {

            app.Use(async (ctx, next) => {
                try
                {
                    string user = ctx.Request.Query["user"];
                    string role = ctx.Request.Query["role"];

                    if (!string.IsNullOrEmpty(user))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user),
                            new Claim(ClaimTypes.Role, string.IsNullOrEmpty(role)? "Guest" : role),
                        };

                        var claimsIdentity = new ClaimsIdentity(claims,"Custom");
                        ctx.User = new ClaimsPrincipal(claimsIdentity);

                    }
                }
                catch (Exception ex)
                {
                    await ctx.Response.WriteAsync(ex.Message);
                }
                finally
                {
                    await next.Invoke();
                }
            });
        }
        public static void UseMyAuthorization(this IApplicationBuilder app)
        {
            app.Use(async(ctx, next)=> {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Authorization for:" + ctx.Request.Path);
                    if (ctx.User.Identity.IsAuthenticated && ctx.User.IsInRole("Admin"))
                    {
                        
                        await ctx.Response.WriteAsync($"Permision for user {ctx.User.Identity.Name} granted<br/>");
                        await next.Invoke();
                    }
                    else
                    {
                        ctx.Response.StatusCode = 404;
                        await ctx.Response.WriteAsync("Permision denied");
                    }
                }
                catch(Exception ex)
                {
                    await ctx.Response.WriteAsync(ex.Message);
                }
            });
        }
    }
}
