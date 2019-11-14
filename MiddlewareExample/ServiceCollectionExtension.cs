using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareExample
{
    public static class ServiceCollectionExtension
    {
        public static void AddScopedWithFactory<IT, T>(this IServiceCollection services) 
            where IT:class
            where T:class,IT
        {        
            services.AddScoped<Func<IT>, Func<T>>((ctx) => () => ctx.GetService<IT>() as T);
            services.AddScoped<IT, T>();
        }
        public static void AddTransientWithFactory<IT, T>(this IServiceCollection services)
        where IT : class
        where T : class, IT
        {            
            services.AddTransient<Func<IT>, Func<T>>((ctx) => () => ctx.GetService<IT>() as T);
            services.AddTransient<IT, T>();
        }
    }
}
