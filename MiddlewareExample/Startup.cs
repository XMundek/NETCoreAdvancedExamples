using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MiddlewareExample.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Reflection;

namespace MiddlewareExample
{
    public class Startup
    {
        IContainer ApplicationContainer;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider 
            ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
            //services.AddScoped<Func<ICountService>, Func<CountService>>(
            //    //(ctx)=>()=> new CountService()
            //    (ctx) => () => ctx.GetService<ICountService>() as CountService
            //    );
            //services.AddScopedWithFactory<ICountService, CountService>();
            //services.AddScopedWithFactory<IReadCountService, ReadCountService>();
            //services.AddSingleton<ICountService, CountService>();
            //services.AddSingleton<IReadCountService, ReadCountService>();
            //services.AddScoped<ICountService, CountService>();
            //services.AddScoped<IReadCountService, ReadCountService>();
            //services.AddTransient<ICountService, CountService>();
            //services.AddTransient<IReadCountService, ReadCountService>();
            //return services.BuildServiceProvider();
            var builder = new ContainerBuilder();
            builder.Populate(services);
            // builder.RegisterType<ReadCountService>().As<IReadCountService>();
            builder.RegisterAssemblyTypes(
                Assembly.GetExecutingAssembly()).Where(t => t.Namespace.EndsWith("Services"))
            .AsImplementedInterfaces().SingleInstance();
            ApplicationContainer = builder.Build();
            // creating the IServiceProvider out of the Autofac container
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ICountService srv)
        {
            //app.Map("/x", (map) =>
            //{
            //    map.Run(async (context) =>
            //    {
            //        await context.Response.WriteAsync("Run middleware for x branch");
            //    });
            //});
            //app.Map("/y", (map) =>
            //{
            //    map.Use(async (context, next) =>
            //    {
            //        await context.Response.WriteAsync("Run middleware for y branch");
            //        await next.Invoke();
            //    });
            //});

            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("xxxx<br />");
            //    await next.Invoke();
            //});

            //app.Use(async (context, next) =>
            //{
            //    try
            //    {
            //        context.Response.ContentType = "text/html";
            //    }
            //    catch (Exception ex)
            //    {
            //        await context.Response.WriteAsync(ex.Message);
            //    }
            //    finally
            //    {
            //        await next.Invoke();
            //    }                
            //});

            //app.UseMvcWithDefaultRoute();

            //app.UseMyAuthentication();
            //app.UseMyAuthorization();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("FirstUse<br />");
                await context.Response.WriteAsync($"Counter:{srv.IncCount()}<br/>");
                await next.Invoke();
            });
            app.Use(async (context, next) =>
            {
                if (context.Request.Query.ContainsKey("id"))
                    await context.Response.WriteAsync($"SecondUse=>{context.Request.Query["id"]}<br />");
                else
                    await next.Invoke();
            });
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("ThirdUse<br />");
                await next.Invoke();
            });

            app.Map("/z", (map) =>
            {
                map.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Run middleware for z branch");
                });
            });

            app.MapWhen(ctx=>ctx.Request.Path.Value.Contains("a"), 
                (map) =>{
                    map.Run(async (context) =>
                    {
                        await context.Response.WriteAsync("Run middleware for conditional branch");
                    });
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("FirstRun<br />");
            });
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("SecondRun<br />");
            });
        }
    }
}
