using System.Text.Json;
using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Engines.Memory;

public interface IContactFieldCatalog
{
    IReadOnlyList<ContactFieldDefinition> All { get; }

    ContactFieldDefinition? Find(string name);

    /// <summary>Checks the field exists, the value fits it, and the source is allowed to set it.</summary>
    FieldCheck Check(string name, JsonElement value, FactSource source);
}
