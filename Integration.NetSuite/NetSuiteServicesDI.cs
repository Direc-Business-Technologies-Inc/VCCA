using Integration.NS.Implementations;
using Integration.NS.Repositories;
using Integration.NS.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Integration.NS;

public static class NetSuiteServicesDI
{
    public static IServiceCollection AddNetSuiteServicesIntegration(this IServiceCollection services)
    {
        services.TryAddSingleton<INetSuiteConnection, NetSuiteConnection>();
        services.TryAddTransient<INetSuiteActions, NetSuiteActions>();

        services.AddNSImplementationsIntegraton();

        return services;
    }
}
