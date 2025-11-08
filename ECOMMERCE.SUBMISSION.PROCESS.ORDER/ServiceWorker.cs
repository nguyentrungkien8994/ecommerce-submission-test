using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.HELPER;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.PROCESS.ORDER
{
    public class ServiceWorker : BackgroundService
    {
        private readonly OrderCreatedHandler _handler;
        private readonly OrderPaymentHandler _paymentHandler;
        private readonly ProductionHandler _productionHandler;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ServiceWorker> _logger;
        private readonly IConfiguration _configuration;
        public ServiceWorker(OrderCreatedHandler handler,
            IEventBus eventBus, ILogger<ServiceWorker> logger,
            IConfiguration configuration,
            ProductionHandler productionHandler,
            OrderPaymentHandler paymentHandler)
        {
            _handler = handler;
            _eventBus = eventBus;
            _logger = logger;
            _configuration = configuration;
            _paymentHandler = paymentHandler;
            _productionHandler = productionHandler;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                string topic = ConfigHelper.GetConfigByKey(EcommerceConstants.KAFKA_TOPIC, _configuration);
                Task orderHandler = _eventBus.Subscribe<Order>(topic, _handler.Handle, stoppingToken);
                Task paymentHandler = _eventBus.Subscribe<OrderPaymentReq>(EcommerceConstants.KAFKA_TOPIC_ORDER_PAYMENT_RESULT, _paymentHandler.Handle, stoppingToken);
                Task productionHandler = _eventBus.Subscribe<ProductionReq>(EcommerceConstants.KAFKA_TOPIC_PROD_PROCESS, _productionHandler.Handle, stoppingToken);
                await Task.WhenAll([orderHandler, paymentHandler, productionHandler]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
