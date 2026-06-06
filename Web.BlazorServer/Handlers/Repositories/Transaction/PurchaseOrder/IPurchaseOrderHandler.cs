using Web.BlazorServer.ViewModels.Transactions.PurchaseOrder;

namespace Web.BlazorServer.Handlers.Repositories.Transaction.PurchaseOrder;

public interface IPurchaseOrderHandler
{
    Task <List<PurchaseOrderLineVM>> GetPO();
}
