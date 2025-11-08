using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.HELPER;
using ECOMMERCE.SUBMISSION.SERVICE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.PROCESS.INVOICE
{
    /// <summary>
    /// Process order payment result
    /// </summary>
    public class OrderPaymentHandler
    {
        private readonly IEventBus _eventBus;
        private readonly IServiceBase<Invoice> _serviceInvoice;

        public OrderPaymentHandler(IEventBus bus, IServiceBase<Invoice> serviceInvoice)
        {
            _eventBus = bus;
            _serviceInvoice = serviceInvoice;
        }

        public async Task Handle(OrderPaymentReq req)
        {

            Console.WriteLine($"[Order.Payment] Processing order {req.order_id}");

            //proceduce order.payment.result
            await _eventBus.PublishAsync<OrderPaymentReq>(EcommerceConstants.KAFKA_TOPIC_ORDER_PAYMENT_RESULT, req);

            var invoice = await _serviceInvoice.GetAsync(req.invoice_id);
            if (invoice == null || invoice.status != (int)InvoiceStatus.Temporary)
                throw new Exception("Invoice not found");

            invoice.status = req.payment_success ? (int)InvoiceStatus.Paid : (int)InvoiceStatus.Cancel;
            invoice.updated_by = "Invoice processor";
            invoice.updated_at = DateTimeHelper.GetUtcTimestamp();
            await _serviceInvoice.Update(invoice);


        }
    }
}
