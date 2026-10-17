namespace Corvus.Mcp.Server.Tawk.Core.Okf;

/// <summary>One file of a bundle: its path from the bundle root, with forward slashes, and its text.</summary>
public sealed record OkfBundleFile(string Path, string Content);
