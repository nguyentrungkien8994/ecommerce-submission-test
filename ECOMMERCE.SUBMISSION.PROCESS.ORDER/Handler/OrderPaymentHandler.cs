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
    public class OrderPaymentHandler
    {
        private readonly IEventBus _eventBus;
        private readonly IServiceBase<Order> _serviceOrder;

        public OrderPaymentHandler(IEventBus bus, IServiceBase<Order> serviceOrder)
        {
            _eventBus = bus;
            _serviceOrder = serviceOrder;
        }

        public async Task Handle(OrderPaymentReq orderReq)
        {
            if (orderReq == null)
                return;

            Console.WriteLine($"[Order.Payment.Result] Processing order {orderReq.order_id}");
            var order = await _serviceOrder.GetAsync(orderReq.order_id);
            if (order == null || order.status != (int)OrderStatus.Checking_Out)
                throw new Exception("Order not found");

            order.status = orderReq.payment_success ? (int)OrderStatus.Ready_In_Production : (int)OrderStatus.Payment_Failed;
            order.updated_by = "Order processor";
            order.updated_at = DateTimeHelper.GetUtcTimestamp();
            await _serviceOrder.Update(order);
        }
    }
}
