using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace L2ACP.Extensions
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class InfoMiddleware
    {
        private readonly RequestDelegate _next;

        public InfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var info = httpContext.RetrieveAccountInfo();
            httpContext.InjectInfoToContext(info);
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class InfoMiddlewareExtensions
    {
        public static IApplicationBuilder UseInfoMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InfoMiddleware>();
        }
    }
}
