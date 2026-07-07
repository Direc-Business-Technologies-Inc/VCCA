using Web.BlazorServer.Components.Common.CascadingValues;

namespace Web.BlazorServer.Components.Base;

public partial class BaseComponent : ComponentBase
{
    #region Cascading Parameters
    [CascadingParameter] protected LoadingScreenProvider loadingScreenProvider { get; set; }
    #endregion Cascading Parameters

    #region Injects
    [Inject] protected IAppActionFactory AppActionFactory { get; set; } = default!;
    [Inject] protected IBusyService AppBusyService { get; set; } = default!;
    [Inject] protected UnsavedChangesService UnsavedChangesService { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected DialogService DialogService { get; set; } = default!;
    [Inject] protected IToastService ToastService { get; set; } = default!;
    [Inject] protected IAlertService AlertService { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected AuthorizationHelper Can { get; set; } = default!;
    #endregion Injects

    #region Primitives
    public bool GridSettingsLoaded { get; set; } = false;
    #endregion Primitives
}
