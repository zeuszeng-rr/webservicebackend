using Microsoft.Extensions.DependencyInjection;

namespace WebServiceBackend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection service)
        {
            service.AddMediatR(config => config.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly));
            return service;
        }
    }
}
