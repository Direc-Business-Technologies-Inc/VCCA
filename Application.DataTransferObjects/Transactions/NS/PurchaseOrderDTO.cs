using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DataTransferObjects.Transactions.NS;

public class PurchaseOrderDTO
{
    public int NetsuiteOrderInternalId { get; set; }
    public string OrderNumber { get; set; }
    public string OrderType { get; set; }
    public string OrderStatus { get; set; }
}
