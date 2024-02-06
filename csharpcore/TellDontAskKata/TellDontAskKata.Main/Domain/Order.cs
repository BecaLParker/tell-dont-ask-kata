using System.Collections.Generic;
using TellDontAskKata.Main.Repository;
using TellDontAskKata.Main.UseCase;

namespace TellDontAskKata.Main.Domain;

public class Order
{
    public decimal Total { get; set; }
    public string Currency { get; init; }
    public IList<OrderItem> Items { get; init; }
    public decimal Tax { get; set; }
    public OrderStatus Status { get; set; }
    public int Id { get; init; }

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

    public static void Create(SellItemsRequest request, IProductCatalog catalog, IOrderRepository repository)
    {
        var order = new Order
        {
            Status = OrderStatus.Created,
            Items = new List<OrderItem>(),
            Currency = "EUR",
            Total = 0m,
            Tax = 0m
        };

        foreach(var itemRequest in request.Requests){
            var product = catalog.GetByName(itemRequest.ProductName);

            if (product == null)
            {
                return;
            }

            var unitaryTax = Round((product.Price / 100m) * product.Category.TaxPercentage);
            var unitaryTaxedAmount = Round(product.Price + unitaryTax);
            var taxedAmount = Round(unitaryTaxedAmount * itemRequest.Quantity);
            var taxAmount = Round(unitaryTax * itemRequest.Quantity);

            var orderItem = new OrderItem
            {
                Product = product,
                Quantity = itemRequest.Quantity,
                Tax = taxAmount,
                TaxedAmount = taxedAmount
            };
            order.Items.Add(orderItem);
            order.Total += taxedAmount;
            order.Tax += taxAmount;
        }

        repository.Save(order);
    }
    
    private static decimal Round(decimal amount)
    {
        return decimal.Round(amount, 2, System.MidpointRounding.ToPositiveInfinity);
    }
}