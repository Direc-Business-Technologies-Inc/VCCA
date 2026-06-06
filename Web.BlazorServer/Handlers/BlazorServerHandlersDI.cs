using Microsoft.Extensions.DependencyInjection.Extensions;
using Web.BlazorServer.Handlers.Implementations.Administration.Authorization;
using Web.BlazorServer.Handlers.Implementations.Administration.Role;
using Web.BlazorServer.Handlers.Implementations.Administration.User;
using Web.BlazorServer.Handlers.Implementations.System;
using Web.BlazorServer.Handlers.Implementations.Transaction.PurchaseOrder;
using Web.BlazorServer.Handlers.Repositories.Administration.Authorization;
using Web.BlazorServer.Handlers.Repositories.Administration.Role;
using Web.BlazorServer.Handlers.Repositories.Administration.User;
using Web.BlazorServer.Handlers.Repositories.System;
using Web.BlazorServer.Handlers.Repositories.Transaction.PurchaseOrder;

namespace Web.BlazorServer.Handlers;

public static class BlazorServerHandlersDI
{
    public static IServiceCollection AddBlazorServerHandlers(this IServiceCollection services)
    {
        services.TryAddTransient<INavigationRouteHandler, NavigationRouteHandler>();

        services.TryAddTransient<IUserManagementHandler, UserManagementHandler>();
        services.TryAddTransient<IRoleManagementHandler, RoleManagementHandler>();
        services.TryAddTransient<IModuleHandler, ModuleHandler>();
        services.TryAddTransient<IDocumentNumberHandler, DocumentNumberHandler>();
        services.TryAddTransient<IAuthorizationHandler, AuthorizationHandler>();

        services.TryAddTransient<IPurchaseOrderHandler, PurchaseOrderHandler>();
        return services;
    }
}
