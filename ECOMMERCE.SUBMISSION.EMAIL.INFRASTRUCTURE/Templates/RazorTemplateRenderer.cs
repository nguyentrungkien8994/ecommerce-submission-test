using ECOMMERCE.SUBMISSION.EMAIL.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.EMAIL.INFRASTRUCTURE
{
    public class RazorTemplateRenderer : ITemplateRenderer
    {
        public Task<string> RenderAsync(string templateId, string? templateSchema, object? model, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
