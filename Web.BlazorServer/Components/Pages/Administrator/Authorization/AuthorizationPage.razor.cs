using Shared.Kernel;
using Sprache;
using System.ComponentModel;
using Web.BlazorServer.Handlers.Repositories.Administration.Authorization;
using Web.BlazorServer.Handlers.Repositories.Administration.Role;
using Web.BlazorServer.Handlers.Repositories.System;

namespace Web.BlazorServer.Components.Pages.Administrator.Authorization;

public partial class AuthorizationPage
{

    #region Primitives
    AuthorizationType ActiveTab { get; set; } = AuthorizationType.Role;
    #endregion Primitives

    #region Overrides

    #endregion Overrides

}

public enum AuthorizationType
{
    [Description("Role Authorization")]
    Role,
    [Description("User Authorization")]
    User
}
