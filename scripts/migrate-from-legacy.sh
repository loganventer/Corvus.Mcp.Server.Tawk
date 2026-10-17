#!/bin/sh
# migrate-from-legacy.sh: copies tawk-mcp's files from the old folders to the Corvus ones.
#
#   config  $XDG_CONFIG_HOME/tawk-mcp  ->  $XDG_CONFIG_HOME/corvus/tawk-mcp   (token, instructions.md, ssh/)
#   data    $XDG_DATA_HOME/tawk-mcp    ->  $XDG_DATA_HOME/corvus/tawk-mcp     (memory.db and its -wal/-shm,
#                                                                              sync state, models, native, built-commit)
#   state   $XDG_STATE_HOME/tawk-mcp   ->  $XDG_STATE_HOME/corvus/tawk-mcp
#   cache   $XDG_CACHE_HOME/tawk-mcp   ->  $XDG_CACHE_HOME/corvus/tawk-mcp
#
# It copies and never moves or deletes: the old folders stay as they are until you remove them yourself.
# A new folder that already exists is left alone unless --force is given; running it twice is safe.
# File modes (0600 files, 0700 folders) are kept. Stop every corvus-tawk-mcp / tawk-mcp (Claude Code sessions
# started with ct, and the user service if you run one) first, so the memory database is not being written.
#
# Usage: scripts/migrate-from-legacy.sh [--dry-run] [--force]
#   --dry-run   print what it would do and change nothing
#   --force     copy over a new folder that already exists (files in it with the same name are replaced)
set -eu

DRY=0
FORCE=0
for arg in "$@"; do
    case "$arg" in
        --dry-run|-n) DRY=1 ;;
        --force|-f)   FORCE=1 ;;
        -h|--help)    sed -n '2,19p' "$0" | sed 's/^# \{0,1\}//'; exit 0 ;;
        *)            echo "migrate-from-legacy: unknown option: $arg (see --help)" >&2; exit 2 ;;
    esac
done

say() { printf '%s\n' "$*"; }
run() {
    if [ "$DRY" -eq 1 ]; then say "    would run: $*"; else "$@"; fi
}

CONFIG="${XDG_CONFIG_HOME:-$HOME/.config}"
DATA="${XDG_DATA_HOME:-$HOME/.local/share}"
STATE="${XDG_STATE_HOME:-$HOME/.local/state}"
CACHE="${XDG_CACHE_HOME:-$HOME/.cache}"

[ "$DRY" -eq 1 ] && say "Dry run: nothing is changed."

# Folds the write-ahead log into the database before it is copied, when sqlite3 is here. The -wal and -shm
# files are copied as well either way, so the copy is complete without it.
checkpoint() {
    db="$1"
    [ -f "$db" ] || return 0
    if command -v sqlite3 >/dev/null 2>&1; then
        if [ "$DRY" -eq 1 ]; then
            say "    would checkpoint $db (sqlite3 PRAGMA wal_checkpoint)"
        elif sqlite3 "$db" 'PRAGMA wal_checkpoint(TRUNCATE);' >/dev/null 2>&1; then
            say "    checkpointed $db"
        else
            say "    could not checkpoint $db (it may be in use); its -wal and -shm files are copied with it"
        fi
    else
        say "    sqlite3 is not installed: $db is copied with its -wal and -shm files"
    fi
}

copied=0
migrate() {
    kind="$1" old="$2" new="$3"
    if [ ! -e "$old" ]; then
        say "$kind: nothing at $old"
        return 0
    fi
    if [ -e "$new" ] && [ "$FORCE" -ne 1 ]; then
        say "$kind: $new already exists, left alone (--force copies over it)"
        return 0
    fi
    verb=copying; [ "$DRY" -eq 1 ] && verb="would copy"
    say "$kind: $verb $old -> $new ($(du -sh "$old" 2>/dev/null | cut -f1))"
    for db in "$old"/*.db; do
        [ -f "$db" ] && checkpoint "$db"
    done
    parent="$(dirname "$new")"
    if [ ! -d "$parent" ]; then
        run mkdir -p "$parent"
        run chmod 0700 "$parent"
    fi
    # cp -Rp keeps modes and times; "old/." copies the contents, so it works whether or not new exists yet.
    run mkdir -p "$new"
    run chmod "$(stat -c '%a' "$old" 2>/dev/null || stat -f '%Lp' "$old")" "$new"
    run cp -Rp "$old/." "$new/"
    copied=$((copied + 1))
}

migrate config "$CONFIG/tawk-mcp" "$CONFIG/corvus/tawk-mcp"
migrate data   "$DATA/tawk-mcp"   "$DATA/corvus/tawk-mcp"
migrate state  "$STATE/tawk-mcp"  "$STATE/corvus/tawk-mcp"
migrate cache  "$CACHE/tawk-mcp"  "$CACHE/corvus/tawk-mcp"
say "runtime: nothing to copy (tawk's control socket and admin token move to \$XDG_RUNTIME_DIR/corvus/tawk when Corvus Tawk starts)"

if [ "$DRY" -eq 1 ]; then
    say "Would copy $copied folder(s). The old folders are never changed."
else
    say "Copied $copied folder(s). The old folders were not changed; remove them yourself once all is well."
fi

# Old variables are not read any more: each needs its new name (CORVUS_TAWK_MCP_..., CORVUS_TAWK_...).
old_vars="$(env | sed -n 's/^\(TAWKMCP_[A-Za-z0-9_]*\)=.*/\1/p; s/^\(TAWK_MCP_[A-Za-z0-9_]*\)=.*/\1/p; s/^\(TAWK_[A-Za-z0-9_]*\)=.*/\1/p' | sort -u)"
if [ -n "$old_vars" ]; then
    say ""
    say "These old environment variables are still set and are no longer read. Rename them:"
    for v in $old_vars; do
        case "$v" in
            TAWKMCP_*)  say "    $v -> CORVUS_TAWK_MCP_${v#TAWKMCP_}" ;;
            TAWK_MCP_*) say "    $v -> CORVUS_TAWK_MCP_${v#TAWK_MCP_}" ;;
            TAWK_*)     say "    $v -> CORVUS_TAWK_${v#TAWK_}" ;;
        esac
    done
else
    say "No old TAWKMCP_, TAWK_MCP_ or TAWK_ environment variables are set."
fi

say ""
say "Still to do by hand (see CUTOVER.md): register the MCP server as corvus-tawk, point CORVUS_TAWK_MCP_SYNC_REPO"
say "at loganventer/Corvus.Data.Sync.TawkMemory, and change the ct alias to run corvus-claude."
