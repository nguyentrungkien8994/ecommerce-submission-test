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

namespace ECOMMERCE.SUBMISSION.PROCESS.PRODUCTION
{
    public class ServiceWorker : BackgroundService
    {
        private readonly ProductionHandler _handler;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ServiceWorker> _logger;
        private readonly IConfiguration _configuration;
        public ServiceWorker(ProductionHandler handler, IEventBus eventBus, ILogger<ServiceWorker> logger, IConfiguration configuration)
        {
            _handler = handler;
            _eventBus = eventBus;
            _logger = logger;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                string topic = ConfigHelper.GetConfigByKey(EcommerceConstants.KAFKA_TOPIC, _configuration);
                await _eventBus.Subscribe<ProductionReq>(topic, _handler.Handle, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
