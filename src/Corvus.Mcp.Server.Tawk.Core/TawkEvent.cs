namespace Corvus.Mcp.Server.Tawk.Core;

public abstract record TawkEvent
{
    /// <summary>The account it happened in, or null from a tawk with one account.</summary>
    public AccountRef? Account { get; init; }
}
