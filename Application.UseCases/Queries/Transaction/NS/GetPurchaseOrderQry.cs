using Application.DataTransferObjects.Transactions.NS;
using Application.UseCases.Repositories.Integration;
using MediatR;

namespace Application.UseCases.Queries.Transaction.NS;

public record GetPurchaseOrderQry() : IRequest<IEnumerable<PurchaseOrderLineDTO>>;

public class GetPurchaseOrderQryHandler(IPurchaseOrder purchaseOrder) : IRequestHandler<GetPurchaseOrderQry, IEnumerable<PurchaseOrderLineDTO>>
{
    public async Task<IEnumerable<PurchaseOrderLineDTO>> Handle(GetPurchaseOrderQry request, CancellationToken cancellationToken)
    {
        return await purchaseOrder.GetPO();
    }
}