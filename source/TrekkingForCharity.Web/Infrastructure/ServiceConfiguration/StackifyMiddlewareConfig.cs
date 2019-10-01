using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TrekkingForCharity.Web.Infrastructure.ServiceConfiguration
{
    public static class StackifyMiddlewareConfig
    {
        public static IApplicationBuilder UseStackify(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
            }

            return app;
        }
    }
}