namespace Corvus.Mcp.Server.Tawk.Host;

public static class HelpText
{
    public const string Text = """
        corvus-tawk-mcp: an MCP server for tawk, the WhatsApp client for the terminal.

        Usage:
          corvus-tawk-mcp [--port N]           serve MCP over streamable HTTP at http://127.0.0.1:N/mcp (default 8765)
          corvus-tawk-mcp --stdio              serve MCP over stdio, for a client that starts corvus-tawk-mcp itself
          corvus-tawk-mcp print-token          print the bearer token for HTTP mode, creating it if needed
          corvus-tawk-mcp healthcheck          ask a running HTTP server for /healthz (exit 0 when it answers)
          corvus-tawk-mcp sync                 sync the memory database with its repository once, now
          corvus-tawk-mcp fetch-model NAME     download a Whisper model ahead of time: tiny, base, small,
                                               medium, large-v3-turbo or large-v3 (otherwise the first
                                               transcription that needs a model downloads it)
          corvus-tawk-mcp export-okf DIR       write knowledge as an Open Knowledge Format 0.2 bundle (--include-sensitive for all of it)
          corvus-tawk-mcp import-okf DIR       read an Open Knowledge Format bundle into memory
          corvus-tawk-mcp --version | --help

        Options (each also has an environment variable):
          --socket PATH                 tawk's control socket          CORVUS_TAWK_CONTROL_SOCKET
          --bind ADDRESS                HTTP bind address, 127.0.0.1   CORVUS_TAWK_MCP_BIND
          every option and command below may be written with two dashes, one or none:
          --port, -port and port are the same
          --port N                      HTTP port, 8765                CORVUS_TAWK_MCP_PORT
          --stdio | --http              transport, http by default     CORVUS_TAWK_MCP_TRANSPORT
          --token-file PATH             bearer token file              CORVUS_TAWK_MCP_TOKEN_FILE (or CORVUS_TAWK_MCP_TOKEN)
          --channel auto|on|off         Claude Code channel events     CORVUS_TAWK_MCP_CHANNEL
          --channel-own on|off          also the messages you send     CORVUS_TAWK_MCP_CHANNEL_OWN (off)
          --channel-read on|off         also read receipts for them    CORVUS_TAWK_MCP_CHANNEL_READ (off)
          --channel-played on|off       also voice notes they played   CORVUS_TAWK_MCP_CHANNEL_PLAYED (off)
          --channel-reactions on|off    also reactions to them         CORVUS_TAWK_MCP_CHANNEL_REACTIONS (off)
          --channel-edits on|off        also others' edits and deletes CORVUS_TAWK_MCP_CHANNEL_EDITS (off)
          --channel-scheduled on|off    also scheduled messages going  CORVUS_TAWK_MCP_CHANNEL_SCHEDULED (off)
          --channel-presence on|off     also who came online or left   CORVUS_TAWK_MCP_CHANNEL_PRESENCE (off)
          --memory write|read|off       voices, contacts, templates    CORVUS_TAWK_MCP_MEMORY (write)
          --data-file PATH              memory database                CORVUS_TAWK_MCP_DATA_FILE
          --sync-repo ADDRESS           private repository for memory  CORVUS_TAWK_MCP_SYNC_REPO
                                        owner/name on GitHub, or an SSH address (git@host:owner/name.git)
          --sync-key PATH               SSH private key to use         CORVUS_TAWK_MCP_SYNC_KEY (SSH chooses by default)
          --sync-branch NAME            its branch, main               CORVUS_TAWK_MCP_SYNC_BRANCH
          --sync-file PATH              the file in it, memory.db      CORVUS_TAWK_MCP_SYNC_FILE
          --sync-interval-minutes N     minutes between syncs, 120     CORVUS_TAWK_MCP_SYNC_INTERVAL_MINUTES
                                        sync runs only with a repository set; it uses git over SSH, no tokens
          --workflow-every N            rounds between workflow checks CORVUS_TAWK_MCP_WORKFLOW_EVERY (20, 0 = off)
          --instructions-file PATH      your standing instructions     CORVUS_TAWK_MCP_INSTRUCTIONS_FILE
          --admin-token-file PATH       tawk's admin token file        CORVUS_TAWK_MCP_ADMIN_TOKEN_FILE
                                        no default: only with it may this instance approve its own sends
          --transcribe MODE             voice notes to text            CORVUS_TAWK_MCP_TRANSCRIBE (auto)
                                        auto: your transcriber at --transcribe-url while it answers,
                                        and otherwise one model loaded inside corvus-tawk-mcp
                                        embedded: always the model inside corvus-tawk-mcp
                                        http, command: only your own transcriber    off: none
          --transcribe-model-dir DIR    models for the one inside      CORVUS_TAWK_MCP_TRANSCRIBE_MODEL_DIR
                                        ~/.local/share/corvus/tawk-mcp/models; a missing model is downloaded
          --transcribe-idle-unload-m N  let that model go when idle    CORVUS_TAWK_MCP_TRANSCRIBE_IDLE_UNLOAD_M (15, 0 = never)
          --transcribe-url ADDRESS      the http transcriber's address CORVUS_TAWK_MCP_TRANSCRIBE_URL
                                        speaks the OpenAI audio API; on this machine unless remote is on
          --transcribe-remote on|off    allow one on another machine   CORVUS_TAWK_MCP_TRANSCRIBE_REMOTE (off)
          --transcribe-command TEXT     program for the command engine CORVUS_TAWK_MCP_TRANSCRIBE_COMMAND
                                        {file} {language} {model} {task} {prompt} are filled in; no shell
                                        tawk's own Settings, Automation, Voice note transcription
                                        override auto, model and language; these are for an older tawk
          --transcribe-auto on|off      transcribe every voice note    CORVUS_TAWK_MCP_TRANSCRIBE_AUTO
                                        that others send, unasked, in the default languages
          --transcribe-model NAME       default model                  CORVUS_TAWK_MCP_TRANSCRIBE_MODEL
                                        large-v3-turbo unless chosen in tawk or here; tiny is the lightest
          --transcribe-models A,B       models an agent may ask for    CORVUS_TAWK_MCP_TRANSCRIBE_MODELS
          --transcribe-language A,B     default languages, or auto     CORVUS_TAWK_MCP_TRANSCRIBE_LANGUAGE
          --transcribe-max-languages N  most languages in one call, 3  CORVUS_TAWK_MCP_TRANSCRIBE_MAX_LANGUAGES
          --transcribe-max-seconds N    longest recording, 3600        CORVUS_TAWK_MCP_TRANSCRIBE_MAX_SECONDS
          --transcribe-timeout-s N      longest pass, 300              CORVUS_TAWK_MCP_TRANSCRIBE_TIMEOUT_S
          --transcribe-concurrency N    jobs at once, 1                CORVUS_TAWK_MCP_TRANSCRIBE_CONCURRENCY
          --schedule-jitter-s N         random +/- shift on schedules  CORVUS_TAWK_MCP_SCHEDULE_JITTER_S (60, 0 = off)
          --backoff-initial-ms N        first retry wait, 500          CORVUS_TAWK_MCP_BACKOFF_INITIAL_MS
          --backoff-max-ms N            longest retry wait, 30000      CORVUS_TAWK_MCP_BACKOFF_MAX_MS
          --breaker-threshold N         failures before pausing, 5     CORVUS_TAWK_MCP_BREAKER_THRESHOLD
          --breaker-cooldown-s N        pause length, 60               CORVUS_TAWK_MCP_BREAKER_COOLDOWN_S
          --request-timeout-s N         read answer timeout, 10        CORVUS_TAWK_MCP_REQUEST_TIMEOUT_S

        HTTP requests need "Authorization: Bearer <token>"; see corvus-tawk-mcp print-token.
        tawk must have Settings > Automation > Control socket turned on.
        """;
}
