using Integration.SAP.Repositories;
using Integration.SAP.Services;
using Integration.SAP.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Integration.SAP;

public static class SapServicesDI
{
    public static IServiceCollection AddSAPServicesIntegraton(this IServiceCollection services)
    {

        services.TryAddSingleton<IServiceLayer, ServiceLayer>();
        services.TryAddTransient<IServiceLayerActions, ServiceLayerActions>();

        services.AddSAPImplementationsIntegraton();

        return services;
    }
}
