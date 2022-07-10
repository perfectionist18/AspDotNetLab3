using AspDotNetLab3.Extentions;
using AspDotNetLab3.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace AspDotNetLab3
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var appConfiguration = new ConfigurationBuilder().AddJsonFile("conf.json").Build();
  
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), appConfiguration["LogFile"]));
            app.UseMiddleware<FileLoggerMiddleware>();

            app.UseSession();

            var routeBuilder = new RouteBuilder(app);

            routeBuilder.MapRoute("Session/Add/{key}/{value}", async context =>
            {
                var route = context.GetRouteData().Values;
                var key = route["key"].ToString();
                var value = route["value"].ToString();
                context.Session.SetSession(key, value);
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync("Session Add");
            });

            routeBuilder.MapRoute("Session/View/{key}", async context =>
            {
                var route = context.GetRouteData().Values;
                var key = route["key"].ToString();
                var result = context.Session.GetSession(key);
                if (result == null)
                {
                    result = "Заданий параметр відсутній!";
                }
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync($"Результат: {result}");
            });

            routeBuilder.MapRoute("Cookie/Add/{key}/{value}", async context =>
            {
                var route = context.GetRouteData().Values;
                var key = route["key"].ToString();
                var value = route["value"].ToString();
                context.Response.Cookies.Append(key, value);
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync("Cookie Add");
            });

            routeBuilder.MapRoute("Cookie/View/{key}", async context =>
            {
                var route = context.GetRouteData().Values;
                var key = route["key"].ToString();
                string result = context.Request.Cookies[key];
                if (result == null)
                {
                    result = "Заданий параметр відсутній!";
                }
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync($"Результат: {result}");
            });

            routeBuilder.MapRoute("{controller}/{action}/{id?}", async context => {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync("First template");
                });

            routeBuilder.MapRoute("{lang}/{controller=Controller}/{action}/{id?}", async context => {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync("Second template");
                });

            app.UseRouter(routeBuilder.Build());

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
