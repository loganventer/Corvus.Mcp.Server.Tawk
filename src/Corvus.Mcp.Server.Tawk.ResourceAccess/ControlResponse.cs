using System.Text.Json;
using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess;

public sealed record ControlResponse(string Id, bool Ok, JsonElement Result, ControlError? Error) : ControlFrame;
