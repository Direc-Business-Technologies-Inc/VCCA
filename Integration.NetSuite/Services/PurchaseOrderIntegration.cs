using Application.DataTransferObjects.Transactions.NS;
using Application.UseCases.Repositories.Integration;
using Integration.NS.Repositories;

namespace Integration.NetSuite.Services;

public class PurchaseOrderIntegration(INetSuiteActions netsuiteActions) : IPurchaseOrder
{
    public async Task<List<PurchaseOrderLineDTO>> GetPO()
    {
        List<PurchaseOrderLineDTO> purchaseOrders = await netsuiteActions.QueryAsync<PurchaseOrderLineDTO, object>("NS_PurchaseOrder_Get_Items", new { tranid = "PO15"});

        return purchaseOrders;
    }
}
