namespace Corvus.Mcp.Server.Tawk.Core;

/// <summary>Raised by corvus-tawk-mcp itself whenever the state of its connection to tawk changes.</summary>
public sealed record ConnectionStateEvent(TawkConnectionState State, HelloInfo? Hello = null) : TawkEvent;
