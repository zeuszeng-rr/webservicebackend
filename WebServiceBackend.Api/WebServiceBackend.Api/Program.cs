using WebServiceBackend.Api.Extensions;
using WebServiceBackend.Core.Options;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

#region Middlewares
app.UseSecurityHeaders();

app.UseCors("AllowViewClient");
app.UseAuthentication();
app.UseAuthorization();

#endregion

#region Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

#region App Pipeline

app.MapControllers();

#endregion

#region Hangfire Dashboard

var hangfireOptions = builder.Configuration
    .GetSection(HangfireOptions.SectionName)
    .Get<HangfireOptions>() ?? new HangfireOptions();

if (hangfireOptions.EnableHangfire && hangfireOptions.EnableDashboard)
{
    app.UseHangfireDashboard("/hangfire");
}

#endregion

#region Hangfire Recurring Jobs

if (hangfireOptions.EnableHangfire)
{
    RecurringJob.AddOrUpdate(
        "sample-job",
        () => Console.WriteLine($"Hello Hangfire @ {DateTime.Now}"),
        Cron.Minutely);
}
#endregion

app.Run();
