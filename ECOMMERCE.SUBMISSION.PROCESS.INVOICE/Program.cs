// See https://aka.ms/new-console-template for more information
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.EVENT.BUS.KAFKA;
using ECOMMERCE.SUBMISSION.PROCESS.INVOICE;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IEventBus, KafkaEventBus>();
builder.Services.AddSingleton<OrderPaymentHandler>();
builder.Services.AddHostedService<ServiceWorker>();

await builder.Build().RunAsync();
