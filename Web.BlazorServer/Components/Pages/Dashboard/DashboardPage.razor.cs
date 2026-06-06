using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Shared.Kernel;
using System.Threading.Tasks;
using Web.BlazorServer.Defaults;
using Web.BlazorServer.Handlers.Repositories.Transaction.PurchaseOrder;
using Web.BlazorServer.ViewModels.Transactions.PurchaseOrder;

namespace Web.BlazorServer.Components.Pages.Dashboard;

public partial class DashboardPage
{

    [Inject]
    private IPurchaseOrderHandler PurchaseOrderHandler { get; set; }

    readonly string ActionGetPurchaseOrders = EnumHelper.GetEnumDescription(AppActions.GetAllPurchaseOrders);


    protected override async Task OnInitializedAsync()
    {

        var action = await AppActionFactory.RunAsync(async () =>
        {
            AppBusyService.SetBusy(ActionGetPurchaseOrders, true);

            List<PurchaseOrderLineVM> result = await PurchaseOrderHandler.GetPO();

            AppBusyService.SetBusy(ActionGetPurchaseOrders, false);

        }, AppActionOptionPresets.Loading(ActionGetPurchaseOrders));

        await base.OnInitializedAsync();
    }

    async Task GetData()
    {

    }
}
