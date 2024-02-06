using System.Collections.Generic;
using TellDontAskKata.Main.Repository;
using TellDontAskKata.Main.UseCase;

namespace TellDontAskKata.Main.Domain;

public class Order
{
    public decimal Total { get; set; }
    public string Currency { get; set; }
    public IList<OrderItem> Items { get; set; }
    public decimal Tax { get; set; }
    public OrderStatus Status { get; set; }
    public int Id { get; set; }

    public void RequestApproval(OrderApprovalRequest request, IOrderRepository repository)
    {
        if (Status == OrderStatus.Shipped)
        {
            throw new ShippedOrdersCannotBeChangedException();
        }

        if (request.Approved && Status == OrderStatus.Rejected)
        {
            throw new RejectedOrderCannotBeApprovedException();
        }

        if (!request.Approved && Status == OrderStatus.Approved)
        {
            throw new ApprovedOrderCannotBeRejectedException();
        }

        Status = request.Approved ? OrderStatus.Approved : OrderStatus.Rejected;
        
        repository.Save(this);
    }
}