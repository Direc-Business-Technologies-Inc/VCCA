using Shared.Services.Repository;
using Web.BlazorServer.Components.Security;

namespace Web.BlazorServer.Services.Implementation;

public class BlazorCurrentUserService(AppAuthenticationService authService) : ICurrentUserService
{
    public Guid UserId => Guid.TryParse(authService.GetUserId(), out var id) ? id : Guid.Empty;
    public string UserName => authService.GetUserName();
    public void SetUser(Guid userId, string userName) { }
    public void SetUser(Guid userId) { }
}
