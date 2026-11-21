using Corvus.Mcp.Library.Brokkr.Contracts.Rows;

namespace Corvus.Mcp.Server.Tawk.ResourceAccess.Sync;

/// <summary>
/// The memory tables that sync, parents before the rows that depend on them, as Brokkr TableSpecs. Every table uses the one rule
/// <see cref="Rule"/>: the more trusted writer wins (user, then contact, then imported, then inferred), then the newer change.
/// </summary>
public static class TawkSyncTables
{
    /// <summary>The name the row rule is registered under.</summary>
    public const string Rule = "tawk-memory";

    /// <summary>The rank of a row with no writer: inferred, the least trusted.</summary>
    public const int Unranked = 0;

    // A concept says who it came from in its producer keys, or failing that through the actor that generated it.
    private const string ConceptSource =
        "COALESCE(json_extract(extra, '$.source'), CASE WHEN generated_by = 'human:self' THEN 'user' "
        + "WHEN generated_by LIKE 'human:%' THEN 'contact' WHEN generated_by LIKE 'process:%' THEN 'imported' ELSE 'inferred' END)";

    public static readonly IReadOnlyList<TableSpec> All =
    [
        new("category", ["path"], ["path", "description", "updated"], Rule),
        new("voice", ["name"], ["name", "description", "guide", "rules", "is_default", "updated"], Rule),
        new("voice_variant", ["voice", "category"], ["voice", "category", "guide", "rules", "examples", "updated"], Rule,
            Parent: new ParentSpec("voice", "name", "voice")),
        new("contact", ["jid"], ["jid", "display_name", "voice", "updated"], Rule),
        new("contact_category", ["jid", "category"], ["jid", "category", "updated"], Rule,
            Parent: new ParentSpec("contact", "jid", "jid")),
        new("contact_fact", ["jid", "field"],
            ["jid", "field", "value", "source", "confidence", "evidence", "sensitive", "updated", "expires", "account"], Rule,
            RankSql: Rank("source"), ExpiresColumn: "expires", Parent: new ParentSpec("contact", "jid", "jid")),
        new("response_template", ["name"], ["name", "description", "category", "voice", "language", "body", "updated"], Rule),
        new("okf_concept", ["id"],
            ["id", "type", "title", "description", "resource", "tags", "generated_by", "generated_at", "verified", "status",
             "stale_after", "sources", "extra", "body", "updated"], Rule,
            RankSql: Rank(ConceptSource)),
        new("okf_link", ["from_id", "to_id", "label"], ["from_id", "to_id", "label", "note", "source", "confidence", "updated", "account"], Rule,
            RankSql: Rank("source"), Parent: new ParentSpec("okf_concept", "id", "from_id")),
    ];

    // The writer's name as a rank, in FactSource's order: inferred 0, imported 1, contact 2, user 3.
    private static string Rank(string source) =>
        $"CASE {source} WHEN 'user' THEN 3 WHEN 'contact' THEN 2 WHEN 'imported' THEN 1 ELSE 0 END";
}
