using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using ECOMMERCE.SUBMISSION.EMAIL.CORE;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.EMAIL.INFRASTRUCTURE
{
    public sealed class HandlebarsTemplateRenderer : ITemplateRenderer
    {
        private readonly ILogger<HandlebarsTemplateRenderer> _logger;
        private static readonly object _sync = new();
        private static readonly Dictionary<string, (Func<object, string> compiled, DateTime mtime)> _cache =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly string _root;

        public HandlebarsTemplateRenderer(ILogger<HandlebarsTemplateRenderer> logger)
        {
            _logger = logger;
            _root = Path.Combine(AppContext.BaseDirectory, "Templates", "Views");
            RegisterDefaultHelpers();
            RegisterPartials();
        }

        public Task<string> RenderAsync(string templateId, string? templateSchema, object? model, CancellationToken ct = default)
        {
            var full = ResolvePath(templateId,templateSchema);
            var tpl = GetOrCompile(full);
            var normalized = NormalizeModel(model);
            return Task.FromResult(tpl(normalized ?? new { }));
        }

        private string ResolvePath(string templateId, string? templateSchema)
        {
            var rel = templateId.Replace('\\', '/');
            if (!rel.EndsWith(".hbs", StringComparison.OrdinalIgnoreCase)) rel += ".hbs";
            string path = string.IsNullOrWhiteSpace(templateSchema) ? Path.Combine(_root, rel) : Path.Combine(_root, templateSchema, rel);

            var full = Path.GetFullPath(path);
            if (!full.StartsWith(_root, StringComparison.Ordinal))
                throw new InvalidOperationException("Template path escapes Templates root.");
            if (!File.Exists(full))
            {
                var available = Directory.Exists(_root)
                    ? string.Join("\n - ", Directory.GetFiles(_root, "*.hbs", SearchOption.AllDirectories)
                        .Select(p => Path.GetRelativePath(_root, p).Replace('\\', '/')))
                    : "(no templates)";
                throw new FileNotFoundException($"Template '{rel}' not found under Templates/.\nAvailable:\n - {available}", full);
            }
            return full;
        }

        private static Func<object, string> GetOrCompile(string full)
        {
            var mtime = File.GetLastWriteTimeUtc(full);
            lock (_sync)
            {
                if (_cache.TryGetValue(full, out var entry) && entry.mtime == mtime)
                    return entry.compiled;

                var source = File.ReadAllText(full);

                // Handlebars.Compile(...) -> HandlebarsTemplate<object, object>
                var compiledRaw = Handlebars.Compile(source);

                // BỌC thành Func<object,string> để code còn lại không phải đổi
                Func<object, string> compiled = model => compiledRaw(model, null);

                _cache[full] = (compiled, mtime);
                return compiled;
            }
        }

        private void RegisterDefaultHelpers()
        {
            Handlebars.RegisterHelper("upper", (w, _, a) =>
            { if (a.Length > 0 && a[0] != null) w.WriteSafeString(a[0]!.ToString()!.ToUpperInvariant()); });
            Handlebars.RegisterHelper("lower", (w, _, a) =>
            { if (a.Length > 0 && a[0] != null) w.WriteSafeString(a[0]!.ToString()!.ToLowerInvariant()); });
            Handlebars.RegisterHelper("formatDate", (w, _, a) =>
            {
                var fmt = (a.Length > 1 ? a[1]?.ToString() : null) ?? "yyyy-MM-dd";
                if (a.Length > 0 && a[0] is DateTime dt) w.WriteSafeString(dt.ToString(fmt));
                else if (a.Length > 0 && DateTime.TryParse(a[0]?.ToString(), out var parsed)) w.WriteSafeString(parsed.ToString(fmt));
            });
        }

        private void RegisterPartials()
        {
            var partialsDir = Path.Combine(_root, "partials");
            if (!Directory.Exists(partialsDir)) return;
            foreach (var file in Directory.GetFiles(partialsDir, "*.hbs", SearchOption.AllDirectories))
            {
                var name = Path.GetRelativePath(partialsDir, file).Replace('\\', '/').Replace(".hbs", "");
                Handlebars.RegisterTemplate(name, File.ReadAllText(file));
            }
        }

        private static object? NormalizeModel(object? model)
        {
            if (model is null) return null;
            if (model is JsonDocument jd) return FromJsonElement(jd.RootElement);
            if (model is JsonElement je) return FromJsonElement(je);
            return model; // POCO/Expando
        }

        private static object? FromJsonElement(JsonElement e) => e.ValueKind switch
        {
            JsonValueKind.Object => ToExpando(e),
            JsonValueKind.Array => e.EnumerateArray().Select(FromJsonElement).ToList(),
            JsonValueKind.String => e.TryGetDateTime(out var dt) ? dt : e.GetString(),
            JsonValueKind.Number => e.TryGetInt64(out var l) ? l : (e.TryGetDouble(out var d) ? d : e.GetDecimal()),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => e.ToString()
        };

        private static ExpandoObject ToExpando(JsonElement e)
        {
            IDictionary<string, object?> map = new ExpandoObject();
            foreach (var p in e.EnumerateObject())
                map[p.Name] = FromJsonElement(p.Value);
            return (ExpandoObject)map;
        }
    }
}
