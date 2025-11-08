using Confluent.Kafka;
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.HELPER;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.EVENT.BUS.KAFKA
{
    public class KafkaEventBus : IEventBus
    {
        private readonly ILogger<KafkaEventBus> _logger;
        private readonly IProducer<string, string> _producer;
        private readonly ConsumerConfig _consumerConfig;

        public KafkaEventBus(ILogger<KafkaEventBus> logger, IConfiguration configuration)
        {
            _logger = logger;
            var bootstrapServer = ConfigHelper.GetConfigByKey(EcommerceConstants.KAFKA_BOOTSTRAP, configuration);
            var topic = ConfigHelper.GetConfigByKey(EcommerceConstants.KAFKA_TOPIC, configuration);
            var groupId = ConfigHelper.GetConfigByKey(EcommerceConstants.KAFKA_GROUP_ID, configuration);
            _producer = new ProducerBuilder<string, string>(
                new ProducerConfig { BootstrapServers = bootstrapServer, AllowAutoCreateTopics=true }).Build();

            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServer,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = true
            };
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            try
            {

                var json = JsonSerializer.Serialize(message);
                await _producer.ProduceAsync(topic, new Message<string, string> { Value = json });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
            }
        }

        public async Task Subscribe<T>(string topic, Func<T, Task> handler, CancellationToken stoppingToken)
        {
            var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
            consumer.Subscribe(topic);

            await Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var msg = consumer.Consume();
                        var data = JsonSerializer.Deserialize<T>(msg.Message.Value);
                        if (data != null)
                            handler(data);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                    }
                }
            });
        }
    }
}
