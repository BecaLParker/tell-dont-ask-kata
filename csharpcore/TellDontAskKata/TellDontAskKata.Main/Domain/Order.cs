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
            return;
        }

        switch (request.Approved)
        {
            case true when Status == OrderStatus.Rejected:
            case false when Status == OrderStatus.Approved:
                return;
            default:
                Status = request.Approved ? OrderStatus.Approved : OrderStatus.Rejected;
        
                repository.Save(this);
                break;
        }
    }
}