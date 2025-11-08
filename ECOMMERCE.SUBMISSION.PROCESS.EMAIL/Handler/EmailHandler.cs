using ECOMMERCE.SUBMISSION.EMAIL.CORE;
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.HELPER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.PROCESS.EMAIL.Handler
{
    public class EmailHandler
    {
        public async Task Handle(EmailRequest req)
        {
            if (req == null)
                return;

            Console.WriteLine($"[email.outbox] Processing send email to {req.To}");
            await Task.CompletedTask;
        }
    }
}
