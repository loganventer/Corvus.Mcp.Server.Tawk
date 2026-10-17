# Contributing

Issues and pull requests are welcome. This page holds the rules the code follows and what a change needs before it is merged. [[ARCHITECTURE]] describes every layer and component, [[HOW_IT_WORKS]] shows the flows, and [[AGENT]] has the longer notes on layout, safety rules and adding a tawk operation. Read [[AGENT]] before a first change, whether you write the code yourself or a coding agent does.

## Contents

- [[#The rules]]
- [[#Where code goes]]
- [[#Build and test]]
- [[#Pull requests]]
- [[#Writing]]

## The rules

Every change keeps to these six. A pull request that breaks one is sent back, however small it is.

| Rule | What it means here |
| --- | --- |
| iDesign layers | Host composes; clients call managers; managers call engines and resource access; everything may use core. Managers never call each other: a tool that needs two managers takes both. An upward call goes only through an interface defined in a lower layer (`IEventSink`, `IUserConfirmation`, `ICircuitBreaker`, `IBackoffPolicy`) |
| SOLID | Single responsibility, open/closed, Liskov substitution, interface segregation and dependency inversion all apply. One manager per area, one store per kind of memory, and small interfaces: add a new one before widening an old one |
| One type per file | Every class, record, enum and interface in its own file, named after it. No nested types |
| Dependency inversion | Depend on interfaces. Bind them only in `TawkMcpComposition` in the host. Do not new up a service anywhere else |
| Composition over inheritance | Behaviour is assembled from small services. Inheritance is kept to the SDK's `BackgroundService`, the exception type, and the closed families of event and frame records |
| Separation of concerns | Formatting and fencing are engines (no I/O), protocol details and SQL are resource access, MCP details are clients, hosting is the host |

## Where code goes

| You are adding | It goes in |
| --- | --- |
| A record, event, error code or a contract shared across layers | `src/Corvus.Mcp.Server.Tawk.Core` |
| A rule with no I/O: formatting, planning, scoring, merging | `src/Corvus.Mcp.Server.Tawk.Engines` |
| Anything that touches tawk's control socket, SQLite or git | `src/Corvus.Mcp.Server.Tawk.ResourceAccess`, behind an interface |
| A use case | `src/Corvus.Mcp.Server.Tawk.Managers`, in the manager for its area |
| An MCP tool, resource or prompt | `src/Corvus.Mcp.Server.Tawk.Clients` |
| A binding, a command line option, middleware or an endpoint | `src/Corvus.Mcp.Server.Tawk.Host` |
| A test or a fake | `tests/Corvus.Mcp.Server.Tawk.Tests`, fakes in `Fakes/`, one per file |

## Build and test

```sh
dotnet build Corvus.Mcp.Server.Tawk.slnx -warnaserror
dotnet test Corvus.Mcp.Server.Tawk.slnx
```

The build is strict: nullable enabled, `latest-recommended` analysers, warnings as errors. Do not suppress a warning in code to get past it; fix it, or, if the rule does not fit this codebase, turn it off in `.editorconfig` with a comment saying why.

The tests need a Unix-like system (they use Unix domain sockets and file modes) and run in a few seconds. They use hand-written fakes only, with no mocking library. There is no CI, so run them locally before pushing.

## Pull requests

- One change per pull request, with a title that says what it does.
- The build with no warnings and the tests passing, on your own machine, before you open it.
- A change that alters behaviour comes with a test.
- Update the documentation the change touches: [[MANUAL]] and the tools table in [[README]] for a tool, [[CONFIGURATION]] for a setting, [[ARCHITECTURE]] for a new component or interface.
- tawk's [CONTROL.md](https://github.com/loganventer/Corvus.App.Terminal.Tawk/blob/main/CONTROL.md) defines the protocol. When it and this code disagree, the document wins; a change that needs a new operation starts in tawk.

## Writing

- Plain British English in code comments, documentation, tool descriptions and commit messages.
- No em dashes, and no dashes used as separators or asides. Use commas, colons, full stops or brackets.
- Comments only where the reason is not obvious from the code.
- Commit messages say what the change does, in the project's own voice, with no co-author or "generated with" lines.
