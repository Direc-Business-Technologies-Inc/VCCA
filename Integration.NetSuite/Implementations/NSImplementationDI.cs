using Microsoft.Extensions.DependencyInjection;

namespace Integration.NS.Implementations;

public static class NSImplementationDI
{
    public static IServiceCollection AddNSImplementationsIntegraton(this IServiceCollection services)
    {
        // Feature IXxxNSIntegration implementations are registered here
        return services;
    }
}
