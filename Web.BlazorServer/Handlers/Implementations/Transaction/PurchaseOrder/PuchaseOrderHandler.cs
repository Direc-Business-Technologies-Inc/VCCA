using Application.UseCases.Queries.Transaction.NS;
using MediatR;
using Web.BlazorServer.Handlers.Repositories.Transaction.PurchaseOrder;
using Web.BlazorServer.ViewModels.Transactions.PurchaseOrder;

namespace Web.BlazorServer.Handlers.Implementations.Transaction.PurchaseOrder;

public class PurchaseOrderHandler(ISender sender) : IPurchaseOrderHandler
{
    public async Task<List<PurchaseOrderLineVM>> GetPO()
    {
        var purchaseOrders = await sender.Send(new GetPurchaseOrderQry());
        // Map the DTOs to ViewModels
        var purchaseOrderVMs = purchaseOrders.Select(x => new PurchaseOrderLineVM
        {
            NetsuiteOrderInternalId = x.NetsuiteOrderInternalId,
            OrderNumber = x.OrderNumber,
            OrderType = x.OrderType,
            OrderStatus = x.OrderStatus,

            NetsuiteSubsidiaryInternalId = x.NetsuiteSubsidiaryInternalId,
            NetsuiteSubsidiaryDefaultBOInternalId = x.NetsuiteSubsidiaryDefaultBOInternalId,

            NetsuiteLocationInternalId = x.NetsuiteLocationInternalId,
            LocationName = x.LocationName,
            LocationUsedBin = x.LocationUsedBin,

            LineSequenceNumber = x.LineSequenceNumber,
            TransactionLineType = x.TransactionLineType,

            NetsuiteVendorInternalId = x.NetsuiteVendorInternalId,
            VendorName = x.VendorName,
            VendorBinAssignmentId = x.VendorBinAssignmentId,

            NetsuiteMaterialInternalId = x.NetsuiteMaterialInternalId,
            MaterialCode = x.MaterialCode,
            MaterialName = x.MaterialName,
            MaterialWeight = x.MaterialWeight,
            LineQuantity = x.LineQuantity,
            LineQuantityReceived = x.LineQuantityReceived,
            NetsuiteUoMInternalId = x.NetsuiteUoMInternalId,
            UoMName = x.UoMName,
            UoMRate = x.UoMRate,
        }).ToList();

        return purchaseOrderVMs;
    }
}
