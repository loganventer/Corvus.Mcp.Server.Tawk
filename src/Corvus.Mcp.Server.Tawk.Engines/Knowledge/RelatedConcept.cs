using Corvus.Mcp.Server.Tawk.Core.Okf;

namespace Corvus.Mcp.Server.Tawk.Engines.Knowledge;

/// <summary>A link as seen from one end, with the name of the concept at the other end.</summary>
public sealed record RelatedConcept(OkfLink Link, string OtherId, string OtherName, bool Outgoing);
