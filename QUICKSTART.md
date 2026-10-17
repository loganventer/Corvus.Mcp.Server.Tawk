# Quickstart

Five minutes from nothing to asking your MCP client "what did I miss?". This uses Claude Code; the other clients are in [README.md](README.md#Quick%20setup).

Having an AI agent do the setup for you? Point it at [AGENT_SETUP](AGENT_SETUP.md), which covers Windows, Linux and macOS step by step.

## Contents

- [1. Have tawk running and linked](#1.%20Have%20tawk%20running%20and%20linked)
- [2. Turn on the control socket](#2.%20Turn%20on%20the%20control%20socket)
- [3. Install corvus-tawk-mcp](#3.%20Install%20corvus-tawk-mcp)
- [4. Start it and add it to Claude Code](#4.%20Start%20it%20and%20add%20it%20to%20Claude%20Code)
- [5. Ask](#5.%20Ask)
- [If it does not work](#If%20it%20does%20not%20work)

<a id="1. Have tawk running and linked"></a>

## 1. Have tawk running and linked

Install [tawk](https://github.com/loganventer/Corvus.App.Terminal.Tawk) and link it to your phone as its quickstart describes:

```sh
curl -fsSL https://raw.githubusercontent.com/loganventer/Corvus.App.Terminal.Tawk/main/install.sh | bash
```

Leave it running in a terminal.

<a id="2. Turn on the control socket"></a>

## 2. Turn on the control socket

In tawk open **Settings > Automation** and turn on **Control socket**. Leave **Agent access** on `read` for now: the model can then read and nothing else. Whatever agents do later shows in tawk's Agentic tab (click **🤖 Agentic** in the header, or press F3).

The same in `~/.config/corvus/tawk/config.ini`:

```ini
[automation]
control_socket = on
access = read
```

<a id="3. Install corvus-tawk-mcp"></a>

## 3. Install corvus-tawk-mcp

```sh
curl -fsSL https://raw.githubusercontent.com/loganventer/Corvus.Mcp.Server.Tawk/main/install.sh | bash
```

The installer builds corvus-tawk-mcp into `~/.local/bin/mcp/corvus-tawk-mcp` (the folder for MCP servers; it need not be on `PATH`). If the .NET 10 SDK is missing it offers to install it into `~/.dotnet` first (no sudo). It registers corvus-tawk-mcp to start at boot and starts it (a systemd user service on Linux, a LaunchAgent on macOS), then creates the bearer token and prints the `claude mcp add` line to use. From a checkout, `./install.sh` does the same.

<a id="4. Start it and add it to Claude Code"></a>

## 4. Start it and add it to Claude Code

corvus-tawk-mcp is already running and serving MCP over HTTP on your own machine (check with `curl http://127.0.0.1:8765/healthz`). If the installer said it could not register it, start it yourself in a spare terminal with `~/.local/bin/mcp/corvus-tawk-mcp`.

Then add it to Claude Code:

```sh
claude mcp add --transport http corvus-tawk http://127.0.0.1:8765/mcp \
  --header "Authorization: Bearer $(~/.local/bin/mcp/corvus-tawk-mcp print-token)"
```

Start `claude` and run `/mcp`. `corvus-tawk` should show as connected with its tools.

<a id="5. Ask"></a>

## 5. Ask

```text
What did I miss on WhatsApp today?
```

The model calls `unread_summary`, then `read_messages` for the chats that need it, and summarises. You can also run the prompt directly with `/mcp__corvus-tawk__catch_up`.

Nothing is marked as read. To let the model propose replies, set **Access** to `send` in tawk: it can then put a draft into a chat's input box with `draft_message` for you to edit and send, or send a message, which tawk shows you to approve first.

<a id="If it does not work"></a>

## If it does not work

| You see | Do this |
| --- | --- |
| "tawk is not running, or its control socket is off" | Start tawk and turn on the control socket. corvus-tawk-mcp connects by itself as soon as the socket appears. |
| "tawk does not allow this" | The action needs `access = send` or `manage` in tawk, or the chat is outside tawk's `chats` list. |
| The tools do not show in `/mcp` | Check corvus-tawk-mcp is running (`curl http://127.0.0.1:8765/healthz`) and that `claude mcp list` shows `corvus-tawk`. |
| 401 from corvus-tawk-mcp | The token in the header is wrong or missing. Add the server again with `$(~/.local/bin/mcp/corvus-tawk-mcp print-token)`. |

More in [MANUAL.md](MANUAL.md#Troubleshooting).
