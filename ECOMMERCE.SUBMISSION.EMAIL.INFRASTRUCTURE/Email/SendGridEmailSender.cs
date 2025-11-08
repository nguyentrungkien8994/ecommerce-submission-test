using ECOMMERCE.SUBMISSION.EMAIL.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Reflection.Metadata;
using Microsoft.Extensions.Configuration;
using ECOMMERCE.SUBMISSION.HELPER;

namespace ECOMMERCE.SUBMISSION.EMAIL.INFRASTRUCTURE
{
    public class SendGridEmailSender : IEmailSender
    {
        //private readonly EmailOptions _opts;
        private readonly ITemplateRenderer _renderer;
        private readonly IConfiguration _configuration;
        public SendGridEmailSender(ITemplateRenderer renderer, IConfiguration configuration)
        {
            _renderer = renderer;
            _configuration = configuration;
        }

        public async Task<EmailResult> SendAsync(EmailRequest request, CancellationToken ct = default)
        {
            var apiKey = ConfigHelper.GetConfigByKey(EcommerceConstants.EMAIL_SENDGRID_KEY, _configuration);
            if(string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Missing SendGrid ApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(request.From, request.FromName);
            var to = new EmailAddress(request.To);
            var msg = MailHelper.CreateSingleEmail(from, to, request.Subject, request.TextBody, request.HtmlBody);

            if (!string.IsNullOrWhiteSpace(request.TemplateId))
            {
                var html = await _renderer.RenderAsync(request.TemplateId, request.TemplateSchema, request.TemplateModel, ct);
                msg.HtmlContent = html;
            }

            if (request.Headers is not null)
            {
                foreach (var kv in request.Headers)
                    msg.AddHeader(kv.Key, kv.Value);
            }

            var resp = await client.SendEmailAsync(msg, ct);
            var body = await resp.Body.ReadAsStringAsync(ct);
            return new EmailResult
            {
                Success = resp.IsSuccessStatusCode,
                ProviderMessageId = resp.Headers.TryGetValues("X-Message-Id", out var ids) ? ids.FirstOrDefault() : null,
                Error = resp.IsSuccessStatusCode ? null : $"{resp.StatusCode}: {body}",
                Provider = EmailProvider.SendGrid
            };
        }
    }
}
