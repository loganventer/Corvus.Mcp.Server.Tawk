using Corvus.Mcp.Server.Tawk.Core;

namespace Corvus.Mcp.Server.Tawk.Managers.Memory;

public interface IContactProfileManager
{
    string ListFields();

    Task<string> GetContactAsync(string chat, bool includeSensitive, CancellationToken cancellationToken);

    Task<string> ListContactsAsync(string? category, string? query, CancellationToken cancellationToken);

    Task<string> SetFieldsAsync(string chat, string fieldsJson, string source, double? confidence, string? evidence, CancellationToken cancellationToken);

    Task<string> AddNoteAsync(string chat, string text, string source, CancellationToken cancellationToken);

    Task<string> ForgetFieldAsync(string chat, string field, CancellationToken cancellationToken);

    Task<string> SetCategoriesAsync(string chat, IReadOnlyList<string> categoryPaths, string? voice, CancellationToken cancellationToken);

    Task<string> DeleteContactAsync(string chat, IUserConfirmation? confirmation, CancellationToken cancellationToken);

    Task<string> DueFollowUpsAsync(int withinDays, CancellationToken cancellationToken);
}
