using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.HELPER;
using ECOMMERCE.SUBMISSION.SERVICE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.PROCESS.ORDER
{
    /// <summary>
    /// Process checkout order
    /// </summary>
    public class OrderCreatedHandler
    {
        private readonly IEventBus _eventBus;
        private readonly IServiceBase<Order> _serviceOrder;

        public OrderCreatedHandler(IEventBus bus, IServiceBase<Order> serviceOrder)
        {
            _eventBus = bus;
            _serviceOrder = serviceOrder;
        }

        public async Task Handle(Order orderReq)
        {
            Console.WriteLine($"[Order.Checkout] Processing order {orderReq.id}");
            var order = await _serviceOrder.GetAsync(orderReq.id);
            if (order == null || order.status != (int)OrderStatus.Ready_Checkout)
                throw new Exception("Order not found");

            order.status = (int)OrderStatus.Checking_Out;
            order.updated_by = "Order processor";
            order.updated_at = DateTimeHelper.GetUtcTimestamp();
            await _serviceOrder.Update(order);
        }
    }
}
