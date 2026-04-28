using PlataformaDigital.SharedKernel;

namespace PlataformaDigital.Modules.Projects.Domain;

public enum ConsolidationStatus { Pending, Running, Completed, CompletedWithErrors, Failed }
public record Money(decimal Amount, string Currency);
public record Period(DateOnly From, DateOnly To);
public record DataSource(string Name, string System);

public sealed class ConsolidationError(string message, string recordKey, DateTime occurredAt)
{
    public string Message { get; } = message;
    public string RecordKey { get; } = recordKey;
    public DateTime OccurredAt { get; } = occurredAt;
}

public sealed class ConsolidatedRecord(Guid id, string sourceKey, Money amount)
{
    public Guid Id { get; } = id;
    public string SourceKey { get; } = sourceKey;
    public Money Amount { get; } = amount;
}

public sealed class ConsolidationExecution : AuditableEntity<Guid>
{
    private readonly List<ConsolidationError> _errors = [];
    public Period Period { get; private set; }
    public ConsolidationStatus Status { get; private set; }
    public IReadOnlyCollection<ConsolidationError> Errors => _errors;

    public ConsolidationExecution(Guid id, Period period)
    {
        Id = id;
        Period = period;
        Status = ConsolidationStatus.Pending;
    }

    public void Start() => Status = ConsolidationStatus.Running;
    public void Complete() => Status = _errors.Count == 0 ? ConsolidationStatus.Completed : ConsolidationStatus.CompletedWithErrors;
    public void RegisterError(string message, string recordKey, DateTime at) => _errors.Add(new ConsolidationError(message, recordKey, at));
}

public interface IConsolidationExecutionRepository
{
    Task AddAsync(ConsolidationExecution execution, CancellationToken ct);
    Task<ConsolidationExecution?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<ConsolidationExecution>> GetHistoryAsync(CancellationToken ct);
}
