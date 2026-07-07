using System.Security.Claims;
using System.Text.Json;

namespace Web.BlazorServer.Components.Security;

public class AppAuthenticationService
{
    readonly IHttpContextAccessor HttpContextAccessor;

    public event Action<ClaimsPrincipal>? UserChanged;
    ClaimsPrincipal? currentUser;

    public ClaimsPrincipal CurrentUser
    {
        get { return currentUser ?? new(); }
        set
        {
            currentUser = value;

            if (UserChanged is not null)
            {
                UserChanged(currentUser);
            }
        }
    }

    public AppAuthenticationService(IHttpContextAccessor httpContextAccessor)
    {
        this.HttpContextAccessor = httpContextAccessor;
        CurrentUser = this.HttpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
    }

    public string GetClaimValue(string claimType)
    {
        if (CurrentUser.Identity?.IsAuthenticated == true)
            return CurrentUser.FindFirst(claimType)?.Value ?? string.Empty;

        return string.Empty;
    }

    public List<string> GetPermissions()
    {
        string permissionString = GetClaimValue("Permissions");
        if (string.IsNullOrEmpty(permissionString)) return [];
        return JsonSerializer.Deserialize<List<string>>(permissionString) ?? [];
    }

    public string GetUserId()
    {
        return GetClaimValue("Id");
    }

    public string GetUserName()
    {
        return GetClaimValue("Name");
    }

    public string GetUserRoleName()
    {
        return GetClaimValue("Role");
    }

    public string GetUserRoleId()
    {
        return GetClaimValue("RoleId");
    }

    public string GetUserEmail()
    {
        return GetClaimValue("Email");
    }

    public bool HasPermission(string permission)
    {
        var permissions = GetPermissions();
        return permissions.Any(p => p.Equals(permission, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsAuthenticated()
    {
        return CurrentUser.Identity?.IsAuthenticated == true;
    }
}
