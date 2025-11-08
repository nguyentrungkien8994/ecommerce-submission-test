// See https://aka.ms/new-console-template for more information
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.EVENT.BUS.KAFKA;
using ECOMMERCE.SUBMISSION.PROCESS.ORDER;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IEventBus, KafkaEventBus>();
builder.Services.AddSingleton<OrderCreatedHandler>();
builder.Services.AddSingleton<OrderPaymentHandler>();
builder.Services.AddSingleton<ProductionHandler>();
builder.Services.AddHostedService<ServiceWorker>();

await builder.Build().RunAsync();
