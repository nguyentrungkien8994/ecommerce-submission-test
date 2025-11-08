
namespace ECOMMERCE.SUBMISSION.EMAIL.CORE
{
    public class EmailRequest
    {
        public required string From { get; init; }
        public required string FromName { get; init; }
        public required string To { get; init; }
        public string? Cc { get; init; }
        public string? Bcc { get; init; }
        public required string Subject { get; init; }
        public string? HtmlBody { get; init; } // Optional if TemplateId provided
        public string? TextBody { get; init; }
        public string? TemplateId { get; init; }
        public string? TemplateSchema { get; init; }
        public object? TemplateModel { get; init; }
        public string? MessageId { get; init; } // for idempotency/debug
        public Dictionary<string, string>? Headers { get; init; }
        public EmailProvider PreferredProvider { get; init; } = EmailProvider.SendGrid;
        public Dictionary<string, string>? Tags { get; init; }
    }
}
