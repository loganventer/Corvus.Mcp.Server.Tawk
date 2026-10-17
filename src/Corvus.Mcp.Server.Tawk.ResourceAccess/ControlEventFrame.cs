using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess;

public sealed record ControlEventFrame(TawkEvent Event) : ControlFrame;
