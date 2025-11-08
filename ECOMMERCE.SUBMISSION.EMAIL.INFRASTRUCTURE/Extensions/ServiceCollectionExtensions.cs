using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ECOMMERCE.SUBMISSION.EMAIL.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.EMAIL.INFRASTRUCTURE
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Template renderer
            services.AddSingleton<ITemplateRenderer, HandlebarsTemplateRenderer>();

            // Rate limiter (in-memory). For multi-instance strict RL, replace with a Redis-based limiter.

            // Email providers
            services.AddTransient<SendGridEmailSender>();

            services.AddTransient<SesEmailSender>();

            return services;
        }
    }
}
