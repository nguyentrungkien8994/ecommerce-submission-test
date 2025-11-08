using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.EVENT.BUS.CORE
{
    public interface IEventBus
    {
        Task PublishAsync<T>(string topic, T message);
        Task Subscribe<T>(string topic, Func<T, Task> handler, CancellationToken stoppingToken);
    }
}
