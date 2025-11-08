namespace ECOMMERCE.SUBMISSION.EMAIL.CORE
{
    public interface IEmailQueue
    {
        Task EnqueueAsync(EmailRequest request, CancellationToken ct = default);
        Task<(string MessageId, EmailRequest? Request)> DequeueAsync(CancellationToken ct);
        Task AckAsync(string messageId, CancellationToken ct);
        Task DeadLetterAsync(string messageId, EmailRequest request, string reason, CancellationToken ct);
    }
}
