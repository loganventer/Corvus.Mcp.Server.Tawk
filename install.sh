#!/usr/bin/env bash
# corvus-tawk-mcp installer: builds and installs corvus-tawk-mcp from source.
# It installs PREFIX/bin/mcp/corvus-tawk-mcp (the folder that holds every MCP server) and, from a checkout,
# links PREFIX/bin/corvus-claude to scripts/corvus-claude. With --legacy-links it also links the old names
# tawk-mcp and claude-tawk in PREFIX/bin to them; both print a deprecation notice.
#
# From a checkout:      ./install.sh [options]
# Straight from GitHub: curl -fsSL https://raw.githubusercontent.com/loganventer/Corvus.Mcp.Server.Tawk/main/install.sh | bash
#                       curl -fsSL .../install.sh | bash -s -- --no-service
#
# Options:
#   --prefix DIR       install location (default ~/.local): corvus-tawk-mcp goes in DIR/bin/mcp,
#                      the corvus-claude launcher in DIR/bin
#   --mcp-dir DIR      put corvus-tawk-mcp in DIR instead of PREFIX/bin/mcp
#   --ref REF          branch or tag to build when cloning (default main)
#   --repo URL         repository to clone (default https://github.com/loganventer/Corvus.Mcp.Server.Tawk.git)
#   --no-service       do not register corvus-tawk-mcp to start at boot (it is registered by default:
#                      a systemd user service with linger, a LaunchAgent on macOS, or a crontab line)
#   --systemd          accepted for compatibility; registering is the default
#   --docker           build the Docker image instead of a program, and print how to run it
#   --legacy-links     also link the old names tawk-mcp and claude-tawk in PREFIX/bin (deprecated)
#   --uninstall        remove an installed corvus-tawk-mcp and its user service
#   -y, --yes          do not ask before installing the .NET SDK
#   -h, --help         show this help
set -euo pipefail

REPO_URL="https://github.com/loganventer/Corvus.Mcp.Server.Tawk.git"
REF="main"
PREFIX="$HOME/.local"
MCP_DIR=""
LEGACY_LINKS=0
SERVICE=1
DOCKER=0
UNINSTALL=0
ASSUME_YES=0
DOTNET_MAJOR=10
CORVUS_TAWK_INSTALLER="https://raw.githubusercontent.com/loganventer/Corvus.App.Terminal.Tawk/main/install.sh"

say()  { printf '\033[1;32m==>\033[0m %s\n' "$*"; }
warn() { printf '\033[1;33m!!\033[0m %s\n' "$*" >&2; }
die()  { printf '\033[1;31mxx\033[0m %s\n' "$*" >&2; exit 1; }
have() { command -v "$1" >/dev/null 2>&1; }

# Printed from here so --help also works when the script arrives through a pipe.
usage() {
    cat <<'HELP'
corvus-tawk-mcp installer: builds and installs corvus-tawk-mcp from source.

From a checkout:      ./install.sh [options]
Straight from GitHub: curl -fsSL https://raw.githubusercontent.com/loganventer/Corvus.Mcp.Server.Tawk/main/install.sh | bash
                      curl -fsSL .../install.sh | bash -s -- --no-service

Options:
  --prefix DIR       install location (default ~/.local): corvus-tawk-mcp goes in DIR/bin/mcp,
                     the corvus-claude launcher in DIR/bin
  --mcp-dir DIR      put corvus-tawk-mcp in DIR instead of PREFIX/bin/mcp
  --ref REF          branch or tag to build when cloning (default main)
  --repo URL         repository to clone (default https://github.com/loganventer/Corvus.Mcp.Server.Tawk.git)
  --no-service       do not register corvus-tawk-mcp to start at boot (it is registered by default:
                     a systemd user service with linger, a LaunchAgent on macOS, or a crontab line)
  --systemd          accepted for compatibility; registering is the default
  --docker           build the Docker image instead of a program, and print how to run it
  --legacy-links     also link the old names tawk-mcp and claude-tawk in PREFIX/bin (deprecated)
  --uninstall        remove an installed corvus-tawk-mcp and its user service
  -y, --yes          do not ask before installing the .NET SDK
  -h, --help         show this help
HELP
}

while [ $# -gt 0 ]; do
    case "$1" in
        --prefix)    PREFIX="${2:?--prefix needs a directory}"; shift 2 ;;
        --ref)       REF="${2:?--ref needs a branch or tag}"; shift 2 ;;
        --repo)      REPO_URL="${2:?--repo needs a URL}"; shift 2 ;;
        --mcp-dir)   MCP_DIR="${2:?--mcp-dir needs a directory}"; shift 2 ;;
        --legacy-links) LEGACY_LINKS=1; shift ;;
        --systemd)   SERVICE=1; shift ;;
        --no-service) SERVICE=0; shift ;;
        --docker)    DOCKER=1; shift ;;
        --uninstall) UNINSTALL=1; shift ;;
        -y|--yes)    ASSUME_YES=1; shift ;;
        -h|--help)   usage; exit 0 ;;
        *)           die "unknown option: $1 (see --help)" ;;
    esac
done


BIN_DIR="$PREFIX/bin"
# MCP servers all live in one folder, apart from the commands people type. It need not be on PATH:
# registrations name the program by its full path.
MCP_DIR="${MCP_DIR:-$BIN_DIR/mcp}"
BIN="$MCP_DIR/corvus-tawk-mcp"
# Where corvus-tawk-mcp was installed before it moved to MCP_DIR (see CUTOVER.md).
OLD_BIN="$BIN_DIR/corvus-tawk-mcp"
UNIT_DIR="${XDG_CONFIG_HOME:-$HOME/.config}/systemd/user"
UNIT="$UNIT_DIR/corvus-tawk-mcp.service"
PLIST="$HOME/Library/LaunchAgents/com.loganventer.corvus-tawk-mcp.plist"
PLIST_LABEL="com.loganventer.corvus-tawk-mcp"
CRON_MARK="# corvus-tawk-mcp: start at boot (added by install.sh)"
LAUNCHER="$BIN_DIR/corvus-claude"
# Legacy names: symlinks to the new ones, made only with --legacy-links (see CUTOVER.md). Remove them, and this block, later.
LEGACY_BIN="$BIN_DIR/tawk-mcp"
LEGACY_LAUNCHER="$BIN_DIR/claude-tawk"
LEGACY_UNIT="$UNIT_DIR/tawk-mcp.service"
LEGACY_PLIST="$HOME/Library/LaunchAgents/com.loganventer.tawk-mcp.plist"
LEGACY_PLIST_LABEL="com.loganventer.tawk-mcp"
LEGACY_CRON_MARK="# tawk-mcp: start at boot (added by install.sh)"
ME="${USER:-$(id -un)}"
TOKEN_FILE="${CORVUS_TAWK_MCP_TOKEN_FILE:-${XDG_CONFIG_HOME:-$HOME/.config}/corvus/tawk-mcp/token}"

if [ "$UNINSTALL" -eq 1 ]; then
    if [ -f "$UNIT" ]; then
        if have systemctl; then
            systemctl --user disable --now corvus-tawk-mcp >/dev/null 2>&1 || true
        fi
        rm -f "$UNIT"
        if have systemctl; then systemctl --user daemon-reload >/dev/null 2>&1 || true; fi
        say "removed the systemd user service"
        if have loginctl && [ "$(loginctl show-user "$ME" -p Linger --value 2>/dev/null)" = yes ]; then
            echo "    Linger stays on for $ME (other user services may rely on it). Turn it off with: loginctl disable-linger $ME"
        fi
    fi
    if [ -f "$LEGACY_UNIT" ]; then
        if have systemctl; then systemctl --user disable --now tawk-mcp >/dev/null 2>&1 || true; fi
        rm -f "$LEGACY_UNIT"
        if have systemctl; then systemctl --user daemon-reload >/dev/null 2>&1 || true; fi
        say "removed the legacy tawk-mcp systemd user service"
    fi
    if [ -f "$PLIST" ]; then
        launchctl bootout "gui/$(id -u)/$PLIST_LABEL" >/dev/null 2>&1 || launchctl unload -w "$PLIST" >/dev/null 2>&1 || true
        rm -f "$PLIST"
        say "removed the LaunchAgent"
    fi
    if have crontab && crontab -l 2>/dev/null | grep -qF "$CRON_MARK"; then
        { crontab -l 2>/dev/null | grep -vF "$CRON_MARK" || true; } | crontab -
        say "removed the @reboot line from your crontab"
    fi
    if [ -e "$BIN" ]; then
        rm -f "$BIN"
        say "removed corvus-tawk-mcp from $MCP_DIR"
        rmdir "$MCP_DIR" 2>/dev/null || true
    fi
    # An install from before corvus-tawk-mcp moved to MCP_DIR left it in BIN_DIR.
    if [ -f "$OLD_BIN" ] && [ "$OLD_BIN" != "$BIN" ]; then
        rm -f "$OLD_BIN"
        say "removed the older corvus-tawk-mcp from $BIN_DIR"
    fi
    if [ -L "$LAUNCHER" ]; then rm -f "$LAUNCHER"; say "removed the corvus-claude link from $BIN_DIR"; fi
    # Legacy links, if present: removed only when they are links to the new names.
    for link in "$LEGACY_BIN" "$LEGACY_LAUNCHER"; do
        case "$(readlink "$link" 2>/dev/null)" in
            corvus-tawk-mcp|corvus-claude|"$BIN"|"$OLD_BIN"|"$LAUNCHER") rm -f "$link"; say "removed the legacy link $link" ;;
        esac
    done
    echo "    The bearer token was kept: $TOKEN_FILE. Delete it to remove everything."
    if have docker && [ -n "$(docker images -q corvus-tawk-mcp 2>/dev/null)" ]; then
        echo "    Docker images named corvus-tawk-mcp were kept; remove them with: docker image rm \$(docker images -q corvus-tawk-mcp)"
    fi
    exit 0
fi

# ---- platform and toolchain -------------------------------------------------

OS="$(uname -s)"
ARCH="$(uname -m)"
case "$OS" in
    Linux)  PLATFORM=linux ;;
    Darwin) PLATFORM=macos ;;
    MINGW*|MSYS*|CYGWIN*) die "corvus-tawk-mcp runs beside tawk, which runs under WSL on Windows. Run this installer inside WSL." ;;
    *)      die "unsupported platform: $OS" ;;
esac
case "$PLATFORM-$ARCH" in
    linux-x86_64)               RID=linux-x64 ;;
    linux-aarch64|linux-arm64)  RID=linux-arm64 ;;
    macos-x86_64)               RID=osx-x64 ;;
    macos-arm64)                RID=osx-arm64 ;;
    *)                          die "corvus-tawk-mcp has no build for $OS on $ARCH" ;;
esac
DISTRO=""
# shellcheck source=/dev/null
[ -r /etc/os-release ] && DISTRO="$(. /etc/os-release && echo "${PRETTY_NAME:-$NAME}")"
say "platform: $PLATFORM $ARCH ($RID)${DISTRO:+, $DISTRO}$(grep -qiE 'microsoft|wsl' /proc/sys/kernel/osrelease 2>/dev/null && echo ', running under WSL')"

confirm() {
    [ "$ASSUME_YES" -eq 1 ] && return 0
    # With no terminal to ask on (piped, or in CI), only --yes may install things.
    { : </dev/tty; } 2>/dev/null || return 1
    printf '%s [Y/n] ' "$1"
    local reply=""
    read -r reply </dev/tty || return 1
    case "$reply" in n|N|no|NO) return 1 ;; *) return 0 ;; esac
}

# The SDK may be on PATH, or where dotnet-install.sh puts it.
find_dotnet() {
    if have dotnet; then
        DOTNET="$(command -v dotnet)"
    elif [ -x "${DOTNET_ROOT:-$HOME/.dotnet}/dotnet" ]; then
        DOTNET="${DOTNET_ROOT:-$HOME/.dotnet}/dotnet"
    else
        DOTNET=""
    fi
}

dotnet_sdk_version() {
    [ -n "$DOTNET" ] || return 1
    "$DOTNET" --list-sdks 2>/dev/null | awk -v m="$DOTNET_MAJOR." 'index($1, m) == 1 {v = $1} END {if (v) print v; else exit 1}'
}

sdk_package_hint() {
    echo "    Install it by running this installer again with --yes (dotnet-install.sh into ~/.dotnet, no sudo),"
    echo "    or with your package manager, for example:"
    if   have apt-get; then echo "      sudo apt-get install dotnet-sdk-$DOTNET_MAJOR.0"
    elif have dnf;     then echo "      sudo dnf install dotnet-sdk-$DOTNET_MAJOR.0"
    elif have pacman;  then echo "      sudo pacman -S dotnet-sdk"
    elif have zypper;  then echo "      sudo zypper install dotnet-sdk-$DOTNET_MAJOR.0 (after adding Microsoft's package repository)"
    elif have brew;    then echo "      brew install --cask dotnet-sdk"
    fi
    echo "    Microsoft's guide: https://learn.microsoft.com/dotnet/core/install/"
}

install_dotnet() {
    local script
    script="$(mktemp "${TMPDIR:-/tmp}/dotnet-install.XXXXXX")"
    if have curl; then
        curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$script"
    elif have wget; then
        wget -qO "$script" https://dot.net/v1/dotnet-install.sh
    else
        rm -f "$script"
        die "curl or wget is needed to download the .NET SDK"
    fi
    say "installing the .NET $DOTNET_MAJOR SDK into $HOME/.dotnet (no sudo)"
    bash "$script" --channel "$DOTNET_MAJOR.0" --install-dir "$HOME/.dotnet" --no-path
    rm -f "$script"
    DOTNET="$HOME/.dotnet/dotnet"
    warn "add the SDK to your PATH for later use: export PATH=\"\$HOME/.dotnet:\$PATH\""
}

find_dotnet
if [ "$DOCKER" -eq 0 ] && ! dotnet_sdk_version >/dev/null; then
    warn "the .NET $DOTNET_MAJOR SDK was not found; it is needed to build corvus-tawk-mcp"
    if confirm "Install it with Microsoft's dotnet-install.sh into ~/.dotnet?"; then
        install_dotnet
        dotnet_sdk_version >/dev/null || die "the .NET SDK install did not work; see the messages above"
    else
        sdk_package_hint
        die "install the .NET $DOTNET_MAJOR SDK, then run this installer again (or pass --yes to let it install one)"
    fi
fi

version_of() {
    case "$1" in
        dotnet) dotnet_sdk_version 2>/dev/null ;;
        git)    have git && git --version 2>/dev/null | cut -d' ' -f3 ;;
        docker) have docker && docker --version 2>/dev/null | sed 's/^Docker version //; s/,.*//' ;;
        corvus-tawk) have corvus-tawk && command -v corvus-tawk ;;
    esac
}
say "toolchain"
for tool in dotnet git docker corvus-tawk; do
    v="$(version_of "$tool" || true)"
    [ "$tool" = docker ] && [ -z "$v" ] && [ "$DOCKER" -eq 0 ] && continue
    if [ -n "$v" ]; then printf '    %-11s %s\n' "$tool" "$v"; else printf '    %-11s %s\n' "$tool" "not found"; fi
done
[ "$DOCKER" -eq 1 ] && ! have docker && die "--docker needs docker"

# ---- source: this checkout, or a fresh clone --------------------------------

SCRIPT_DIR=""
if [ -n "${BASH_SOURCE[0]:-}" ] && [ -f "${BASH_SOURCE[0]}" ]; then
    SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
fi
CLEANUP=""
FROM_CHECKOUT=0
cleanup() { local d; for d in $CLEANUP; do rm -rf "$d"; done; }
trap cleanup EXIT
if [ -n "$SCRIPT_DIR" ] && [ -f "$SCRIPT_DIR/Corvus.Mcp.Server.Tawk.slnx" ] && [ -f "$SCRIPT_DIR/src/Corvus.Mcp.Server.Tawk.Host/Corvus.Mcp.Server.Tawk.Host.csproj" ]; then
    SRC="$SCRIPT_DIR"
    FROM_CHECKOUT=1
    say "building from $SRC"
else
    have git || die "git is required to download corvus-tawk-mcp"
    SRC="$(mktemp -d "${TMPDIR:-/tmp}/corvus-tawk-mcp-build.XXXXXX")"
    CLEANUP="$SRC"
    say "downloading corvus-tawk-mcp ($REF) from $REPO_URL"
    git clone --quiet --depth 1 --branch "$REF" "$REPO_URL" "$SRC"
    say "building commit $(git -C "$SRC" log -1 --format='%h %s (%cd)' --date=short)"
fi

VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$SRC/Directory.Build.props" | head -n 1)"
VERSION="${VERSION:-latest}"

# ---- Docker -----------------------------------------------------------------

if [ "$DOCKER" -eq 1 ]; then
    say "building the Docker image corvus-tawk-mcp:$VERSION"
    docker build -t "corvus-tawk-mcp:$VERSION" "$SRC"
    RUNTIME="${XDG_RUNTIME_DIR:-$HOME/.local/state}"
    say "done. Create the two folders once (Docker would make them owned by root), then start it:"
    cat <<RUN
    mkdir -p -m 0700 "$RUNTIME/corvus/tawk" "${XDG_CONFIG_HOME:-$HOME/.config}/corvus/tawk-mcp"
    docker run -d --name corvus-tawk-mcp --restart unless-stopped \\
      --user "\$(id -u):\$(id -g)" -p 127.0.0.1:8765:8765 \\
      -v "$RUNTIME/corvus/tawk:/run/tawk" -v "${XDG_CONFIG_HOME:-$HOME/.config}/corvus/tawk-mcp:/data" \\
      corvus-tawk-mcp:$VERSION
    The bearer token appears in ${XDG_CONFIG_HOME:-$HOME/.config}/corvus/tawk-mcp/token once it has started.
    With --restart unless-stopped, Docker starts it again at boot whenever the Docker service starts at boot.
RUN
    have corvus-tawk || warn "Corvus Tawk (corvus-tawk) is not installed. Install it with: curl -fsSL $CORVUS_TAWK_INSTALLER | bash"
    exit 0
fi

# ---- build and install ------------------------------------------------------

OUT="$(mktemp -d "${TMPDIR:-/tmp}/corvus-tawk-mcp-publish.XXXXXX")"
CLEANUP="${CLEANUP:+$CLEANUP }$OUT"
say "publishing a self-contained single file for $RID (this takes a minute the first time)"
DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 "$DOTNET" publish "$SRC/src/Corvus.Mcp.Server.Tawk.Host/Corvus.Mcp.Server.Tawk.Host.csproj" \
    -c Release -r "$RID" --self-contained \
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none \
    -o "$OUT" >/dev/null

say "installing to $BIN"
mkdir -p "$MCP_DIR"
install -m 0755 "$OUT/corvus-tawk-mcp" "$BIN"
if [ -f "$OLD_BIN" ] && [ ! -L "$OLD_BIN" ] && [ "$OLD_BIN" != "$BIN" ]; then
    warn "an older corvus-tawk-mcp is still in $BIN_DIR and is no longer updated. Point registrations and services"
    warn "at $BIN, then remove it: rm $OLD_BIN   (see CUTOVER.md)"
fi

# The launcher that starts Claude Code with the channel on is linked, not copied, so it finds this checkout
# to rebuild from. A launcher already there that is not a link is left alone.
if [ "$FROM_CHECKOUT" -eq 1 ] && [ -f "$SRC/scripts/corvus-claude" ]; then
    if [ -e "$LAUNCHER" ] && [ ! -L "$LAUNCHER" ]; then
        warn "$LAUNCHER exists and is not a link; leaving it as it is"
    else
        mkdir -p "$BIN_DIR"
        ln -sfn "$SRC/scripts/corvus-claude" "$LAUNCHER"
        say "linked $LAUNCHER to $SRC/scripts/corvus-claude"
    fi
fi

# Legacy, only with --legacy-links: the old command names point at the new ones, so registrations and aliases
# that still use them keep working; each prints a deprecation notice when run under the old name. tawk-mcp may
# be the old program itself, which is replaced. claude-tawk is replaced only when it is a link (to an older
# checkout, say).
if [ "$LEGACY_LINKS" -eq 1 ]; then
    mkdir -p "$BIN_DIR"
    ln -sfn "$BIN" "$LEGACY_BIN"
    say "legacy: linked $LEGACY_BIN to $BIN (deprecated; see CUTOVER.md)"
    if [ -L "$LAUNCHER" ]; then
        if [ -e "$LEGACY_LAUNCHER" ] && [ ! -L "$LEGACY_LAUNCHER" ]; then
            warn "$LEGACY_LAUNCHER exists and is not a link; leaving it as it is"
        else
            ln -sfn corvus-claude "$LEGACY_LAUNCHER"
            say "legacy: linked $LEGACY_LAUNCHER to corvus-claude (deprecated; see CUTOVER.md)"
        fi
    fi
fi

# Only the commands people type need PATH; $MCP_DIR does not, as registrations use the full path.
if [ -L "$LAUNCHER" ] || [ "$LEGACY_LINKS" -eq 1 ]; then
    case ":$PATH:" in
        *":$BIN_DIR:"*) ;;
        *) warn "$BIN_DIR is not on your PATH, so corvus-claude is not found by name; add it, for example: export PATH=\"$BIN_DIR:\$PATH\"" ;;
    esac
fi

# ---- start at boot -----------------------------------------------------------

SERVICE_KIND=""

systemd_user_ok() {
    [ -d /run/systemd/system ] && have systemctl && systemctl --user show-environment >/dev/null 2>&1
}

enable_linger() {
    have loginctl || { warn "loginctl not found; corvus-tawk-mcp starts when you log in, not at boot"; return 0; }
    if [ "$(loginctl show-user "$ME" -p Linger --value 2>/dev/null)" = yes ]; then
        say "linger is already on for $ME, so user services start at boot"
        return 0
    fi
    if loginctl enable-linger "$ME" 2>/dev/null; then
        say "turned on linger for $ME, so corvus-tawk-mcp starts at boot without anyone logging in"
    elif [ "$ASSUME_YES" -eq 1 ] && have sudo && sudo -n loginctl enable-linger "$ME" 2>/dev/null; then
        say "turned on linger for $ME (with sudo), so corvus-tawk-mcp starts at boot"
    else
        warn "could not turn on linger, so corvus-tawk-mcp starts when you log in rather than at boot. To fix that run:"
        echo "    sudo loginctl enable-linger $ME"
    fi
}

# The service was called tawk-mcp before. Registering the new one retires the old one, which would hold the same port.
retire_legacy_systemd() {
    [ -f "$LEGACY_UNIT" ] || return 0
    systemctl --user disable --now tawk-mcp >/dev/null 2>&1 || true
    rm -f "$LEGACY_UNIT"
    say "legacy: stopped, disabled and removed the old tawk-mcp user service"
}

register_systemd() {
    retire_legacy_systemd
    mkdir -p "$UNIT_DIR"
    sed "s#%h/.local/bin/mcp/corvus-tawk-mcp#$BIN#" "$SRC/contrib/systemd/corvus-tawk-mcp.service" > "$UNIT"
    systemctl --user daemon-reload
    systemctl --user enable --now corvus-tawk-mcp >/dev/null 2>&1 || warn "systemctl --user enable --now corvus-tawk-mcp failed; see: systemctl --user status corvus-tawk-mcp"
    enable_linger
    say "systemd user service corvus-tawk-mcp: $(systemctl --user is-enabled corvus-tawk-mcp 2>/dev/null || echo unknown), $(systemctl --user is-active corvus-tawk-mcp 2>/dev/null || echo unknown)"
    echo "    Its log: journalctl --user -u corvus-tawk-mcp"
    SERVICE_KIND="systemd"
}

register_launchagent() {
    if [ -f "$LEGACY_PLIST" ]; then
        launchctl bootout "gui/$(id -u)/$LEGACY_PLIST_LABEL" >/dev/null 2>&1 || launchctl unload -w "$LEGACY_PLIST" >/dev/null 2>&1 || true
        rm -f "$LEGACY_PLIST"
        say "legacy: unloaded and removed the old LaunchAgent $LEGACY_PLIST_LABEL"
    fi
    mkdir -p "$(dirname "$PLIST")" "$HOME/Library/Logs"
    cat > "$PLIST" <<PLISTXML
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>$PLIST_LABEL</string>
    <key>ProgramArguments</key>
    <array>
        <string>$BIN</string>
    </array>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
    <key>StandardOutPath</key>
    <string>$HOME/Library/Logs/corvus-tawk-mcp.log</string>
    <key>StandardErrorPath</key>
    <string>$HOME/Library/Logs/corvus-tawk-mcp.log</string>
</dict>
</plist>
PLISTXML
    launchctl bootout "gui/$(id -u)/$PLIST_LABEL" >/dev/null 2>&1 || true
    if ! launchctl bootstrap "gui/$(id -u)" "$PLIST" 2>/dev/null; then
        launchctl load -w "$PLIST" 2>/dev/null || warn "launchctl could not load $PLIST"
    fi
    if launchctl print "gui/$(id -u)/$PLIST_LABEL" >/dev/null 2>&1; then
        say "LaunchAgent $PLIST_LABEL: loaded, $(launchctl print "gui/$(id -u)/$PLIST_LABEL" 2>/dev/null | awk -F'= ' '/^[[:space:]]*state =/ {print $2; exit}')"
    else
        say "LaunchAgent $PLIST_LABEL: $(launchctl list | grep -F "$PLIST_LABEL" || echo 'not loaded')"
    fi
    echo "    It starts when you log in and restarts if it stops. Its log: ~/Library/Logs/corvus-tawk-mcp.log"
    SERVICE_KIND="launchagent"
}

register_crontab() {
    if grep -qiE 'microsoft|wsl' /proc/sys/kernel/osrelease 2>/dev/null; then
        warn "this WSL has no systemd. To turn it on, add these lines to /etc/wsl.conf, then run 'wsl --shutdown' in Windows:"
        echo "    [boot]"
        echo "    systemd=true"
        echo "    and run this installer again."
    else
        warn "there is no systemd user session here, so no user service can be registered"
    fi
    if ! have crontab; then
        warn "crontab is not available either, so corvus-tawk-mcp was NOT registered to start at boot. Start it yourself: $BIN"
        return 0
    fi
    if crontab -l 2>/dev/null | grep -qF "$LEGACY_CRON_MARK"; then
        { crontab -l 2>/dev/null | grep -vF "$LEGACY_CRON_MARK" || true; } | crontab -
        say "legacy: removed the old tawk-mcp @reboot line from your crontab"
    fi
    if crontab -l 2>/dev/null | grep -qF "$CRON_MARK"; then
        say "your crontab already starts corvus-tawk-mcp at boot"
        SERVICE_KIND="crontab"
        return 0
    fi
    if confirm "Add an @reboot line to your crontab to start corvus-tawk-mcp at boot instead?"; then
        { crontab -l 2>/dev/null || true; printf '@reboot %s >/dev/null 2>&1 %s\n' "$BIN" "$CRON_MARK"; } | crontab -
        say "added to your crontab: @reboot $BIN (it starts at the next boot; start it now with: $BIN &)"
        SERVICE_KIND="crontab"
    else
        warn "corvus-tawk-mcp was NOT registered to start at boot. Start it yourself: $BIN"
    fi
}

if [ "$SERVICE" -eq 1 ]; then
    say "registering corvus-tawk-mcp to start at boot (--no-service skips this)"
    if [ "$PLATFORM" = macos ]; then
        register_launchagent
    elif systemd_user_ok; then
        register_systemd
    else
        register_crontab
    fi
fi

# ---- check and next steps ---------------------------------------------------

say "checking: $("$BIN" --version)"
"$BIN" print-token >/dev/null
say "the bearer token for HTTP clients is in $TOKEN_FILE ($BIN print-token shows it)"

# Voice notes are transcribed inside corvus-tawk-mcp. The default model is fetched now so the first one does
# not wait for a download (about 1.6 GB); another model chosen in tawk is fetched the first time it is needed.
MODEL_DIR="${CORVUS_TAWK_MCP_TRANSCRIBE_MODEL_DIR:-${XDG_DATA_HOME:-$HOME/.local/share}/corvus/tawk-mcp/models}"
DEFAULT_MODEL=large-v3-turbo
if [ -f "$MODEL_DIR/ggml-$DEFAULT_MODEL.bin" ]; then
    say "transcription model: $DEFAULT_MODEL is already in $MODEL_DIR"
else
    say "transcription model: fetching $DEFAULT_MODEL (about 1.6 GB), this takes a while ..."
    if "$BIN" fetch-model "$DEFAULT_MODEL" >/dev/null 2>&1; then
        say "transcription model: fetched $DEFAULT_MODEL into $MODEL_DIR"
    else
        warn "the $DEFAULT_MODEL transcription model could not be fetched now. corvus-tawk-mcp fetches it the first time"
        warn "a voice note is transcribed, or run: $BIN fetch-model $DEFAULT_MODEL"
    fi
fi

if ! have corvus-tawk; then
    warn "Corvus Tawk (corvus-tawk) is not installed. corvus-tawk-mcp needs it running; install it with:"
    echo "    curl -fsSL $CORVUS_TAWK_INSTALLER | bash"
fi

say "done. Next steps:"
echo "    1. In tawk, turn on Settings > Automation > Control socket, and choose what agents may do"
echo "       under Settings > Automation > Agent access (read, send or manage)."
echo "       What agents do shows in tawk's Agentic tab (F3, or click 🤖 Agentic in the header)."
case "$SERVICE_KIND" in
    systemd|launchagent) echo "    2. corvus-tawk-mcp is already serving http://127.0.0.1:8765/mcp, and starts at boot." ;;
    crontab)             echo "    2. corvus-tawk-mcp starts at boot; start it now with: $BIN &   (it serves http://127.0.0.1:8765/mcp)" ;;
    *)                   echo "    2. Start it: $BIN   (it serves http://127.0.0.1:8765/mcp)" ;;
esac
echo "    3. Add it to Claude Code:"
echo "       claude mcp add --transport http corvus-tawk http://127.0.0.1:8765/mcp --header \"Authorization: Bearer \$($BIN print-token)\""
echo "       or, for the channel that corvus-claude starts with (stdio):"
echo "       claude mcp add corvus-tawk -- $BIN --stdio"
