using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebServiceBackend.Api.Extensions
{
    public static class ConfigureSecurityHeaders
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["X-XSS-Protection"] = "1; mode=block";
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
                headers["Referrer-Policy"] = "no-referrer";
                headers["Server"] = "";
                headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
                headers["Pragma"] = "no-cache";
                headers["Expires"] = "0";
                headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), payment=()";
                headers["Content-Security-Policy"] =
                    "default-src 'self'; " +
                    "script-src 'self'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data:; " +
                    "font-src 'self'; " +
                    "object-src 'none'; " +
                    "frame-ancestors 'none'; " +
                    "base-uri 'self'; " +
                    "form-action 'self';";

                await next();
            });
        }
    }
}
