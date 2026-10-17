namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

public interface ITemplateRenderer
{
    IReadOnlyList<string> Placeholders(string body);

    TemplateRendering Render(string body, IReadOnlyDictionary<string, string> values);
}
