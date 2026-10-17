using Corvus.Mcp.Server.Tawk.Core.Okf;

namespace Corvus.Mcp.Server.Tawk.Engines.Knowledge;

/// <summary>An observation with the names of what it is about.</summary>
public sealed record ObservationView(OkfConcept Concept, IReadOnlyList<string> About);
