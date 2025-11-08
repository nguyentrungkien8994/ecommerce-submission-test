namespace ECOMMERCE.SUBMISSION.EMAIL.CORE
{
    public class EmailResult
    {
        public bool Success { get; init; }
        public string? ProviderMessageId { get; init; }
        public string? Error { get; init; }
        public EmailProvider Provider { get; init; }
    }
}
