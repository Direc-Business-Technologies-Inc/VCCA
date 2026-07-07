
namespace Web.BlazorServer.ViewModels.Administration.Role;

public class RolePermissionVM
{
    public int Id { get; set; }
    public ModulePermissionVM Permission { get; set; }
    public Guid RoleId { get; set; }
}
