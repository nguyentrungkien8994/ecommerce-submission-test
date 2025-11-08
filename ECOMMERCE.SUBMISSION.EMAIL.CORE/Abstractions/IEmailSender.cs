namespace ECOMMERCE.SUBMISSION.EMAIL.CORE
{
    public interface IEmailSender
    {
        Task<EmailResult> SendAsync(EmailRequest request, CancellationToken ct = default);
    }
}
