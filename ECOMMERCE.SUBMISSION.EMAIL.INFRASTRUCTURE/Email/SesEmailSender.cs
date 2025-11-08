using Microsoft.Extensions.Options;
using ECOMMERCE.SUBMISSION.EMAIL.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.EMAIL.INFRASTRUCTURE
{
    public class SesEmailSender : IEmailSender
    {
        private readonly ITemplateRenderer _renderer;

        public SesEmailSender(ITemplateRenderer renderer)
        {
            _renderer = renderer;
        }

        public async Task<EmailResult> SendAsync(EmailRequest request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
