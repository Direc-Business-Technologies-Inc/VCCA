using Microsoft.Extensions.DependencyInjection;

namespace Integration.Odoo.Implementations;

public static class OdooImplementationDI
{
    public static IServiceCollection AddOdooImplementations(this IServiceCollection services)
    {
        // Feature IXxxOdooIntegration implementations are registered here
        return services;
    }
}
