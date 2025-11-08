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
    public class ProductionHandler
    {
        private readonly IEventBus _eventBus;
        private readonly IServiceBase<Order> _serviceOrder;

        public ProductionHandler(IEventBus bus, IServiceBase<Order> serviceOrder)
        {
            _eventBus = bus;
            _serviceOrder = serviceOrder;
        }

        public async Task Handle(ProductionReq req)
        {
            if (req == null)
                return;

            Console.WriteLine($"[Prod.Process] Processing order {req.order_id}");
            var order = await _serviceOrder.GetAsync(req.order_id);
            if (order == null)
                throw new Exception("Order not found");

            order.status = (int)OrderStatus.In_Production;
            order.updated_by = "Order processor";
            order.updated_at = DateTimeHelper.GetUtcTimestamp();
            await _serviceOrder.Update(order);
        }
    }
}
