using Corvus.Mcp.Server.Tawk.Core.Memory;

namespace Corvus.Mcp.Server.Tawk.Core.Okf;

/// <summary>What a bundle holds: concepts, the links between them, and the profile fields of contact concepts.</summary>
public sealed record OkfBundle(IReadOnlyList<OkfConcept> Concepts, IReadOnlyList<OkfLink> Links, IReadOnlyList<ContactFact> Facts);
