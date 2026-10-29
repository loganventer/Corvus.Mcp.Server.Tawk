using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.AspNetCore;
using Corvus.Mcp.Server.Tawk.Clients.Streaming;
using Corvus.Mcp.Server.Tawk.Host;
using Corvus.Mcp.Server.Tawk.Managers;

if (LegacyCommandName.IsLegacy(LegacyCommandName.ReadArgv0()))
{
    await Console.Error.WriteLineAsync(LegacyCommandName.Notice);
}

var options = TawkMcpOptionsBinder.Bind(args, Environment.GetEnvironmentVariable);
if (options.Error is not null)
{
    await Console.Error.WriteLineAsync(options.Error);
    return 2;
}

switch (options.Command)
{
    case HostCommand.Help:
        Console.WriteLine(HelpText.Text);
        return 0;
    case HostCommand.Version:
        Console.WriteLine("corvus-tawk-mcp " + TawkMcpComposition.Version);
        return 0;
    case HostCommand.PrintToken:
        var store = TawkMcpComposition.CreateTokenStore(options);
        Console.WriteLine(store.GetOrCreate());
        return 0;
    case HostCommand.Healthcheck:
        return await HealthCheckCommand.RunAsync(options, Console.Out);
    case HostCommand.FetchModel:
        return await FetchModelCommand.RunAsync(options, Console.Out);
    case HostCommand.ExportOkf:
    case HostCommand.ImportOkf:
    case HostCommand.Sync:
        return await MemoryCommand.RunAsync(options, Console.Out);
}

// The user's own instructions file is read once, when the server starts.
options = options with
{
    UserInstructions = UserInstructionsFile.Read(options.InstructionsFile ?? UserInstructionsFile.DefaultPath(
        Environment.GetEnvironmentVariable, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))),
};

if (options.Transport == TransportKind.Stdio)
{
    // stdout carries the MCP protocol, so every log line goes to stderr.
    // The content root is the install folder, not the folder the client starts us in: the default config watcher puts an
    // inotify watch on every folder under the content root, and under a large tree on /mnt/c that outlasts the client's connect timeout.
    var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { DisableDefaults = false, Args = [], ContentRootPath = AppContext.BaseDirectory });
    ConfigureLogging(builder.Logging);
    builder.Services.AddTawkMcp(options).WithStdioServerTransport();
    await builder.Build().RunAsync();
    return 0;
}

var tokens = TawkMcpComposition.CreateTokenStore(options);
tokens.GetOrCreate();
var web = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [], ContentRootPath = AppContext.BaseDirectory });
ConfigureLogging(web.Logging);
// Kestrel listens only where CORVUS_TAWK_MCP_BIND and CORVUS_TAWK_MCP_PORT say; this replaces any ASPNETCORE_URLS.
web.WebHost.UseUrls(new UriBuilder(Uri.UriSchemeHttp, IPAddress.Parse(options.Bind).ToString(), options.Port).Uri.ToString().TrimEnd('/'));
web.Services.AddSingleton(tokens);
// Sessions carry elicitation, resource subscriptions and channel events, which stateless HTTP cannot.
web.Services.AddTawkMcp(options).WithHttpTransport(http => http.SessionMode = HttpServerSessionMode.Stateful);

var app = web.Build();
app.UseMiddleware<OriginGuardMiddleware>();
app.UseMiddleware<BearerTokenMiddleware>(tokens);
app.MapGet(HealthEndpoint.Path, (ILiveUpdatesManager live) => HealthEndpoint.Handle(live));
var events = new EventStreamEndpoint(
    app.Services.GetRequiredService<IEventStreamHub>(), app.Services.GetRequiredService<ILiveUpdatesManager>(), options.Heartbeat);
app.MapGet(EventStreamEndpoint.Path, events.HandleAsync);
app.MapMcp("/mcp");
await Console.Error.WriteLineAsync($"corvus-tawk-mcp {TawkMcpComposition.Version} serving MCP at http://{options.Bind}:{options.Port}/mcp");
await app.RunAsync();
return 0;

static void ConfigureLogging(ILoggingBuilder logging)
{
    logging.ClearProviders();
    logging.AddConsole(console => console.LogToStandardErrorThreshold = LogLevel.Trace);
    logging.SetMinimumLevel(LogLevel.Information);
    logging.AddFilter("Microsoft", LogLevel.Warning);
    logging.AddFilter("ModelContextProtocol", LogLevel.Warning);
}
