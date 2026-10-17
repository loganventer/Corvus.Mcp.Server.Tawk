using Corvus.Mcp.Server.Tawk.Core.Okf;

namespace Corvus.Mcp.Server.Tawk.Engines.Knowledge;

/// <summary>What a bundle's files held, and what could not be read, one line per problem.</summary>
public sealed record OkfBundleReading(OkfBundle Bundle, IReadOnlyList<string> Problems);
