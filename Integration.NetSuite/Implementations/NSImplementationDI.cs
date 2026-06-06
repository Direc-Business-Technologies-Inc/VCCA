using Application.UseCases.Repositories.Integration;
using Integration.NetSuite.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Integration.NS.Implementations;

public static class NSImplementationDI
{
    public static IServiceCollection AddNSImplementationsIntegraton(this IServiceCollection services)
    {
        // Feature IXxxNSIntegration implementations are registered here
        services.TryAddTransient<IPurchaseOrder, PurchaseOrderIntegration>();

        return services;
    }
}
