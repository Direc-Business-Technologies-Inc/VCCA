using Integration.Odoo.Implementations;
using Integration.Odoo.Repositories;
using Integration.Odoo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Integration.Odoo;

public static class OdooServicesDI
{
    public static IServiceCollection AddOdooServicesIntegration(this IServiceCollection services)
    {
        services.TryAddSingleton<IOdooConnection, OdooConnection>();
        services.TryAddTransient<IOdooActions, OdooActions>();

        services.AddOdooImplementations();

        return services;
    }
}
