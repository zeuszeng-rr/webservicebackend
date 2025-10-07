using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Text;
using WebServiceBackend.Core.Entities;
using WebServiceBackend.Core.Options;
using WebServiceBackend.Application.Commands;

namespace WebServiceBackend.Api.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuretion;
        private readonly AppSettingOptions _appSettingOptions;

        public LoggingMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IConfiguration configuration, IOptions<AppSettingOptions> options)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _configuretion = configuration;
            _appSettingOptions = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";
            if (_appSettingOptions.IncludePaths.Contains(path, StringComparer.OrdinalIgnoreCase))
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var stopwatch = Stopwatch.StartNew();

                string? requestBody = null;
                context.Request.EnableBuffering();
                if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }

                var trnEntity = new TrnLoggingInfoEntity
                {
                    Method = context.Request.Method,
                    Path = path,
                    QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                    RequestBody = requestBody,
                    RequestTimestamp = DateTime.UtcNow,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
                    LogLevel = "Information",
                    CreatedBy = context.User.Identity?.Name 
                };

                trnEntity = await mediator.Send(new AddTrnLoggingInfoCommand(trnEntity));
                
                var originalBodyStream = context.Response.Body;
                await using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                await _next(context);

                stopwatch.Stop();

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string? responseBody = null;
                using (var reader = new StreamReader(context.Response.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    responseBody = await reader.ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                }

                await responseBodyStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;

                trnEntity.StatusCode = context.Response.StatusCode;
                trnEntity.ResponseTimestamp = DateTime.UtcNow;
                trnEntity.ResponseBody = responseBody;
                trnEntity.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                await mediator.Send(new UpdateTrnLoggingInfoCommand(trnEntity));
                return;
            }

            await _next(context);
        }
    }
}
