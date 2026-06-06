using Application.DataTransferObjects.Transactions.NS;

namespace Application.UseCases.Repositories.Integration;

public interface IPurchaseOrder
{
    Task <List<PurchaseOrderLineDTO>> GetPO();
}
