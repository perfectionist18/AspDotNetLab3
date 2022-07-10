using AspDotNetLab3.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AspDotNetLab3.Middleware
{
    public class FileLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public FileLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILoggerFactory loggerFactory)
        {
            var feature = httpContext.Features.Get<IHttpConnectionFeature>();
            var ip = feature?.LocalIpAddress?.ToString();
            string message = $"time: {DateTime.Now}, ip: {ip}, path: {httpContext.Request.Path}";
            var logger = loggerFactory.CreateLogger("FileLogger");
            logger.LogInformation(message);
            await _next.Invoke(httpContext);
        }
    }
}
