# Cutover: tawk-mcp to Corvus.Mcp.Server.Tawk

This repository was `tawk-mcp`. It is now `Corvus.Mcp.Server.Tawk`, part of the Corvus naming scheme (`Corvus.<Area>.<Kind>.<Name>`). Only names, paths and environment variables changed. The control protocol, the memory database schema, OKF content and the sync history are the same, and the WhatsApp linked device is not touched.

This file lists every name that changed and the ordered steps to switch a live install over. The old names are listed here, in `scripts/migrate-from-legacy.sh`, and in the legacy-link code of `install.sh`. Other files use only the new names.

## Contents

- [What changed](#What%20changed)
  - [Repository and code](#Repository%20and%20code)
  - [Commands](#Commands)
  - [Environment variables](#Environment%20variables)
  - [Paths](#Paths)
  - [[#The MCP folder `~/.local/bin/mcp`]]
  - [Services, registrations, images](#Services,%20registrations,%20images)
- [Switching a live install over](#Switching%20a%20live%20install%20over)
- [Rolling back](#Rolling%20back)

<a id="What changed"></a>

## What changed

<a id="Repository and code"></a>

### Repository and code

| Old | New |
| --- | --- |
| folder `~/Documents/Corvus/tawk-mcp` | `~/Documents/Corvus/Corvus.Mcp.Server.Tawk` |
| `github.com/loganventer/tawk-mcp` | `github.com/loganventer/Corvus.Mcp.Server.Tawk` |
| `TawkMcp.slnx` | `Corvus.Mcp.Server.Tawk.slnx` |
| projects, folders, namespaces `Tawk.Mcp.<Layer>` | `Corvus.Mcp.Server.Tawk.<Layer>` (Core, ResourceAccess, Engines, Managers, Clients, Host, Tests) |
| assembly `tawk-mcp` (`tawk-mcp.dll`) | `corvus-tawk-mcp` (`corvus-tawk-mcp.dll`) |
| `<Product>tawk-mcp</Product>` | `<Product>corvus-tawk-mcp</Product>` |
| MCP server info name `tawk-mcp`, title `tawk` | `corvus-tawk-mcp`, title `Corvus Tawk` |
| client name in tawk's `hello` and its Agents list `tawk-mcp` | `corvus-tawk-mcp` (tawk uses the `features` list, not the name) |
| OKF actor for what it writes `tawk-mcp/<version>` | `corvus-tawk-mcp/<version>` (concepts already stored keep their old value) |
| memory sync commit author `tawk-mcp <tawk-mcp@localhost>` | `corvus-tawk-mcp <corvus-tawk-mcp@localhost>` |

<a id="Commands"></a>

### Commands

| Old | New |
| --- | --- |
| `tawk-mcp` | `corvus-tawk-mcp` |
| `claude-tawk` (`scripts/claude-tawk`) | `corvus-claude` (`scripts/corvus-claude`) |
| `tawk` (Corvus Tawk itself; its own repository changes it) | `corvus-tawk` |

`install.sh --legacy-links` makes **legacy symlinks** `~/.local/bin/tawk-mcp -> ~/.local/bin/mcp/corvus-tawk-mcp` and `~/.local/bin/claude-tawk -> corvus-claude`, so old registrations and aliases still start. Without the flag they are not made. Each prints a one-line deprecation notice on stderr when run under the old name. They are temporary: remove them once nothing uses the old names (step 10). `install.sh --uninstall` removes them if they are there.

<a id="Environment variables"></a>

### Environment variables

There is no fallback: the old names are not read any more.

| Old | New |
| --- | --- |
| `TAWKMCP_*` (every option: `TAWKMCP_PORT`, `TAWKMCP_SYNC_REPO`, `TAWKMCP_CHANNEL_OWN`, ...) | `CORVUS_TAWK_MCP_*` (`CORVUS_TAWK_MCP_PORT`, `CORVUS_TAWK_MCP_SYNC_REPO`, `CORVUS_TAWK_MCP_CHANNEL_OWN`, ...) |
| `TAWK_MCP_SRC`, `TAWK_MCP_PULL`, `TAWK_MCP_PULL_TIMEOUT`, `TAWK_MCP_ADMIN`, `TAWK_MCP_SERVER` (claude-tawk) | `CORVUS_TAWK_MCP_SRC`, `CORVUS_TAWK_MCP_PULL`, `CORVUS_TAWK_MCP_PULL_TIMEOUT`, `CORVUS_TAWK_MCP_ADMIN`, `CORVUS_TAWK_MCP_SERVER` (corvus-claude) |
| (none) | `CORVUS_TAWK_MCP_EXTRA_SERVERS`: more channels corvus-claude loads, default `corvus-schedule`; empty for none |
| `TAWK_CONTROL_SOCKET` | `CORVUS_TAWK_CONTROL_SOCKET` |
| `TAWKMCP_SYNC_SSH` (compose only) | `CORVUS_TAWK_MCP_SYNC_SSH` |

<a id="Paths"></a>

### Paths

`$XDG_*` defaults apply as before (`~/.config`, `~/.local/share`, `~/.local/state`, `~/.cache`).

| Kind | Old | New |
| --- | --- | --- |
| config | `~/.config/tawk-mcp/` | `~/.config/corvus/tawk-mcp/` |
| bearer token | `~/.config/tawk-mcp/token` | `~/.config/corvus/tawk-mcp/token` |
| standing instructions | `~/.config/tawk-mcp/instructions.md` | `~/.config/corvus/tawk-mcp/instructions.md` |
| data | `~/.local/share/tawk-mcp/` | `~/.local/share/corvus/tawk-mcp/` |
| memory database | `~/.local/share/tawk-mcp/memory.db` (+ `-wal`, `-shm`) | `~/.local/share/corvus/tawk-mcp/memory.db` (+ `-wal`, `-shm`) |
| sync state | `~/.local/share/tawk-mcp/memory.db.sync.git`, `.sync.json`, `.sync.lock` | the same names under `~/.local/share/corvus/tawk-mcp/` |
| Whisper models | `~/.local/share/tawk-mcp/models/` | `~/.local/share/corvus/tawk-mcp/models/` |
| Whisper native libraries | `~/.local/share/tawk-mcp/native/` | `~/.local/share/corvus/tawk-mcp/native/` |
| launcher build stamp | `~/.local/share/tawk-mcp/built-commit` | `~/.local/share/corvus/tawk-mcp/built-commit` |
| state, cache | `~/.local/state/tawk-mcp/`, `~/.cache/tawk-mcp/` (unused) | `~/.local/state/corvus/tawk-mcp/`, `~/.cache/corvus/tawk-mcp/` |
| tawk's control socket | `$XDG_RUNTIME_DIR/tawk/control.sock`, else `~/.local/state/tawk/control.sock` | `$XDG_RUNTIME_DIR/corvus/tawk/control.sock`, else `~/.local/state/corvus/tawk/control.sock` |
| tawk's admin token | `admin.token` beside the old socket | `admin.token` beside the new socket |
| program | `~/.local/bin/tawk-mcp` | `~/.local/bin/mcp/corvus-tawk-mcp` (old name: legacy link, with `--legacy-links`) |
| launcher | `~/.local/bin/claude-tawk` -> `tawk-mcp/scripts/claude-tawk` | `~/.local/bin/corvus-claude` -> `Corvus.Mcp.Server.Tawk/scripts/corvus-claude` (old name: legacy link, with `--legacy-links`) |
| temp folders | `tawk-mcp-build.*`, `tawk-mcp-publish.*`, `tawk-mcp-sync-*` | `corvus-tawk-mcp-build.*`, `corvus-tawk-mcp-publish.*`, `corvus-tawk-mcp-sync-*` |

<a id="The MCP folder ~/.local/bin/mcp"></a>

### The MCP folder: `~/.local/bin/mcp`

Every MCP server is now installed into one folder, `~/.local/bin/mcp`, apart from the commands people type, which stay in `~/.local/bin`. So `corvus-tawk-mcp` is `~/.local/bin/mcp/corvus-tawk-mcp`, and `corvus-claude` stays `~/.local/bin/corvus-claude`. `~/.local/bin/mcp` need not be on `PATH`: registrations, the systemd unit, the LaunchAgent and the crontab line name the program in full. `install.sh --mcp-dir DIR` picks another folder.

An install from before this change left the program at `~/.local/bin/corvus-tawk-mcp`. The new `install.sh` (and so `corvus-claude`, which runs it to rebuild) installs to `~/.local/bin/mcp/` and warns while the old file is there, but leaves it, because a registration may still start it; it is no longer updated. To move over:

```sh
./install.sh --no-service -y          # or ./install.sh -y with the HTTP service; it rewrites the unit with the new path
claude mcp remove corvus-tawk -s user
claude mcp add corvus-tawk --scope user \
  --env CORVUS_TAWK_MCP_SYNC_REPO=git@github-tawk-memory:loganventer/Corvus.Data.Sync.TawkMemory.git \
  -- "$HOME/.local/bin/mcp/corvus-tawk-mcp" --stdio
rm ~/.local/bin/corvus-tawk-mcp       # once nothing starts it any more
```

Keep the `--env` options the old registration had (`claude mcp get corvus-tawk` shows them). Close and restart the `ct` sessions afterwards; each runs its own `--stdio` server. A unit written by hand from `contrib/systemd` needs its `ExecStart` changed to `%h/.local/bin/mcp/corvus-tawk-mcp`, then `systemctl --user daemon-reload && systemctl --user restart corvus-tawk-mcp`. `install.sh --uninstall` also removes the old file.

<a id="Services, registrations, images"></a>

### Services, registrations, images

| Old | New |
| --- | --- |
| systemd user unit `tawk-mcp.service` (`contrib/systemd/tawk-mcp.service`) | `corvus-tawk-mcp.service` (`contrib/systemd/corvus-tawk-mcp.service`) |
| LaunchAgent `com.loganventer.tawk-mcp`, log `~/Library/Logs/tawk-mcp.log` | `com.loganventer.corvus-tawk-mcp`, log `~/Library/Logs/corvus-tawk-mcp.log` |
| crontab marker `# tawk-mcp: start at boot` | `# corvus-tawk-mcp: start at boot` |
| Claude Code MCP server `tawk` | `corvus-tawk` |
| channel `server:tawk`, events `<channel source="tawk">` | `server:corvus-tawk`, `<channel source="corvus-tawk">` |
| tool names `mcp__tawk__*` (for permission rules) | `mcp__corvus-tawk__*` |
| Docker image and container `tawk-mcp` | `corvus-tawk-mcp`; host folders mounted from `corvus/tawk` and `corvus/tawk-mcp` (inside the container `/run/tawk` and `/data` are unchanged) |
| memory sync repository `loganventer/tawk-memory` | `loganventer/Corvus.Data.Sync.TawkMemory` |

Registering a service with the new `install.sh` retires the old one (it would hold the same port): it stops, disables and removes `tawk-mcp.service`, unloads and removes the old LaunchAgent, or removes the old crontab line. `install.sh --no-service`, which `corvus-claude` uses, registers nothing and stops nothing.

<a id="Switching a live install over"></a>

## Switching a live install over

Do the steps in this order. Commands assume the default folders and Linux.

1. **Cut Corvus Tawk over first, or at the same time.** `corvus-tawk-mcp` looks for tawk's socket at `$XDG_RUNTIME_DIR/corvus/tawk/control.sock`. A tawk that still uses the old socket is not found. To run the new server against an old tawk for a while, set `CORVUS_TAWK_CONTROL_SOCKET=$XDG_RUNTIME_DIR/tawk/control.sock` in its registration (step 5).

2. **Stop what uses the old files.** Close every Claude Code session started with `ct` or `claude-tawk`; each runs its own `tawk-mcp --stdio`. If you run the HTTP service, stop it with `systemctl --user stop tawk-mcp`.

3. **Build and install under the new names.** This installs `~/.local/bin/mcp/corvus-tawk-mcp`, links `corvus-claude`, and with `--legacy-links` makes the legacy links (the old `~/.local/bin/tawk-mcp` binary is replaced by a link). Leave `--legacy-links` out if nothing uses the old names:
   ```sh
   cd ~/Documents/Corvus/Corvus.Mcp.Server.Tawk
   PATH="$HOME/.dotnet:$PATH" ./install.sh --no-service -y --legacy-links
   ```
   If you run the HTTP service, use `./install.sh -y` instead. It registers `corvus-tawk-mcp.service` and retires `tawk-mcp.service`.

4. **Copy the data.** Check the plan first, then copy. The script copies and never moves, and it lists any old variables that are still set:
   ```sh
   scripts/migrate-from-legacy.sh --dry-run
   scripts/migrate-from-legacy.sh
   ```

5. **Register the MCP server as `corvus-tawk`.** This also points memory sync at the renamed repository. The SSH alias and deploy key stay valid, because GitHub keeps deploy keys when it renames a repository:
   ```sh
   claude mcp remove tawk -s user
   claude mcp add corvus-tawk --scope user \
     --env CORVUS_TAWK_MCP_SYNC_REPO=git@github-tawk-memory:loganventer/Corvus.Data.Sync.TawkMemory.git \
     -- "$HOME/.local/bin/mcp/corvus-tawk-mcp" --stdio
   ```
   Until GitHub has renamed `tawk-memory`, the old address still works: `git@github-tawk-memory:loganventer/tawk-memory.git`. After the rename, GitHub redirects it.

6. **Change the `ct` alias** in `~/.bashrc`. `corvus-claude` loads `server:corvus-tawk` and `server:corvus-schedule` by itself:
   ```sh
   ct() { mise upgrade claude || echo "ct: Claude Code update failed; starting anyway" >&2; corvus-claude "$@"; }
   ```
   Remove the comment above it about `claude-tawk` passing `server:tawk`. An old `ct` that still passes `server:corvus-schedule` keeps working, because the server is not added twice.

7. **Rename the variables** that `migrate-from-legacy.sh` listed, wherever you export them: shell profile, service file, MCP client `env`, and compose `.env`. `TAWKMCP_X` becomes `CORVUS_TAWK_MCP_X`, `TAWK_MCP_X` becomes `CORVUS_TAWK_MCP_X`, and `TAWK_CONTROL_SOCKET` becomes `CORVUS_TAWK_CONTROL_SOCKET`.

8. **Update permission rules.** Any `allow`/`deny` rule for `mcp__tawk__...` in `~/.claude/settings.json` or a project's `.claude/settings*.json` becomes `mcp__corvus-tawk__...`.

9. **Check.** Run these, then start `ct` and run `/mcp`. `corvus-tawk` should show as connected, and in Corvus Tawk's Agents list the session should show as `corvus-tawk-mcp`.
   ```sh
   ~/.local/bin/mcp/corvus-tawk-mcp --version
   claude mcp get corvus-tawk
   ~/.local/bin/mcp/corvus-tawk-mcp sync      # with CORVUS_TAWK_MCP_SYNC_REPO set: should say it is in step
   ```

10. **Later, once nothing uses the old names,** remove the legacy links (if you made them) and the old folders. These steps are not automatic:
    ```sh
    rm -f ~/.local/bin/tawk-mcp ~/.local/bin/claude-tawk
    rm -r ~/.config/tawk-mcp ~/.local/share/tawk-mcp
    ```

<a id="Rolling back"></a>

## Rolling back

The old folders and the old checkout are left as they were, so a rollback needs no copying:

1. Close the `ct` sessions.
2. Run `./install.sh --no-service -y` in `~/Documents/Corvus/tawk-mcp`. It puts a real `tawk-mcp` back.
3. Link the old launcher back with `ln -sfn ~/Documents/Corvus/tawk-mcp/scripts/claude-tawk ~/.local/bin/claude-tawk`.
4. Re-register the server: `claude mcp remove corvus-tawk -s user && claude mcp add tawk --scope user --env TAWKMCP_SYNC_REPO=git@github-tawk-memory:loganventer/tawk-memory.git -- "$HOME/.local/bin/tawk-mcp" --stdio`.
5. Restore the old `ct`.

Memory written under the new paths in the meantime reaches the old install through memory sync, when sync is set up.
