namespace ECOMMERCE.SUBMISSION.EMAIL.CORE
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(string templateId,string? templateSchema, object? model, CancellationToken ct = default);
    }
}
