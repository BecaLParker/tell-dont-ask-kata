using System;
using TellDontAskKata.Main.Domain;
using TellDontAskKata.Main.UseCase;
using TellDontAskKata.Tests.Doubles;
using Xunit;

namespace TellDontAskKata.Tests.UseCase;

public class OrderApprovalUseCaseTest
{
    private readonly TestOrderRepository _orderRepository;

    public OrderApprovalUseCaseTest()
    {
        _orderRepository = new TestOrderRepository();
    }


    [Fact]
    public void ApprovedExistingOrder()
    {
        var initialOrder = new Order
        {
            Status = OrderStatus.Created,
            Id = 1
        };
        _orderRepository.AddOrder(initialOrder);

        var request = new OrderApprovalRequest
        {
            OrderId = 1,
            Approved = true
        };
        
        initialOrder.RequestApproval(request, _orderRepository);

        var savedOrder = _orderRepository.GetSavedOrder();
        Assert.Equal(OrderStatus.Approved, savedOrder.Status);
    }

    [Fact]
    public void RejectedExistingOrder()
    {
        var initialOrder = new Order
        {
            Status = OrderStatus.Created,
            Id = 1
        };
        _orderRepository.AddOrder(initialOrder);

        var request = new OrderApprovalRequest
        {
            OrderId = 1,
            Approved = false
        };

        initialOrder.RequestApproval(request, _orderRepository);

        var savedOrder = _orderRepository.GetSavedOrder();
        Assert.Equal(OrderStatus.Rejected, savedOrder.Status);
    }


    [Fact]
    public void CannotApproveRejectedOrder()
    {
        var initialOrder = new Order
        {
            Status = OrderStatus.Rejected,
            Id = 1
        };
        _orderRepository.AddOrder(initialOrder);

        var request = new OrderApprovalRequest
        {
            OrderId = 1,
            Approved = true
        };


        initialOrder.RequestApproval(request, _orderRepository);
      
        Assert.Equal(OrderStatus.Rejected, initialOrder.Status);
        Assert.Null(_orderRepository.GetSavedOrder());
    }

    [Fact]
    public void CannotRejectApprovedOrder()
    {
        var initialOrder = new Order
        {
            Status = OrderStatus.Approved,
            Id = 1
        };
        _orderRepository.AddOrder(initialOrder);

        var request = new OrderApprovalRequest
        {
            OrderId = 1,
            Approved = false
        };


        initialOrder.RequestApproval(request, _orderRepository);
            
        Assert.Equal(OrderStatus.Approved, initialOrder.Status);
        Assert.Null(_orderRepository.GetSavedOrder());
    }

    [Fact]
    public void ShippedOrdersCannotBeRejected()
    {
        var initialOrder = new Order
        {
            Status = OrderStatus.Shipped,
            Id = 1
        };
        _orderRepository.AddOrder(initialOrder);

        var request = new OrderApprovalRequest
        {
            OrderId = 1,
            Approved = false
        };


        initialOrder.RequestApproval(request, _orderRepository);

        Assert.Equal(OrderStatus.Shipped, initialOrder.Status);
        Assert.Null(_orderRepository.GetSavedOrder());
    }

}