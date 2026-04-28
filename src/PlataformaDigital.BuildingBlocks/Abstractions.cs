namespace PlataformaDigital.BuildingBlocks;

public record Result(bool IsSuccess, string? Error = null)
{
    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
}

public record Result<T>(bool IsSuccess, T? Value = default, string? Error = null)
{
    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(string error) => new(false, default, error);
}

public record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount);

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}

public interface IFileStorageService
{
    Task<string> SaveAsync(string fileName, Stream content, CancellationToken ct = default);
}

public interface IExcelExportService
{
    Task<byte[]> ExportAsync<T>(IReadOnlyCollection<T> items, string sheetName, CancellationToken ct = default);
}

public interface ICurrentUserService { string? UserId { get; } }
public interface IDateTimeProvider { DateTime UtcNow { get; } }
public interface IAuditService { Task WriteAsync(string action, string details, CancellationToken ct = default); }

public record PageRequest(int Page = 1, int PageSize = 20);
