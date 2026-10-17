namespace Corvus.Mcp.Server.Tawk.Host;

public interface ITokenStore
{
    /// <summary>Returns the bearer token, creating and saving one on first use where the store can.</summary>
    string GetOrCreate();

    string Describe();
}
