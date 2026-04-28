using PlataformaDigital.SharedKernel;

namespace PlataformaDigital.Modules.Treasury.Domain;

public enum InvoiceStatus { Open, DueSoon, Overdue, Paid }
public enum PaymentStatus { Pending, Committed, Completed }

public sealed class Customer(Guid id, string name)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
}

public sealed class InvoiceComment(DateTime createdAt, string text)
{
    public DateTime CreatedAt { get; } = createdAt;
    public string Text { get; } = text;
}

public sealed class PaymentCommitment(DateOnly dueDate, decimal amount)
{
    public DateOnly DueDate { get; } = dueDate;
    public decimal Amount { get; } = amount;
}

public sealed class NotificationLog(DateTime sentAt, string recipient)
{
    public DateTime SentAt { get; } = sentAt;
    public string Recipient { get; } = recipient;
}

public sealed class Invoice : AuditableEntity<Guid>
{
    private readonly List<InvoiceComment> _comments = [];
    private readonly List<PaymentCommitment> _commitments = [];
    public Guid CustomerId { get; private set; }
    public DateOnly DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public IReadOnlyCollection<InvoiceComment> Comments => _comments;
    public IReadOnlyCollection<PaymentCommitment> Commitments => _commitments;

    public Invoice(Guid id, Guid customerId, DateOnly dueDate, decimal amount)
    { Id = id; CustomerId = customerId; DueDate = dueDate; Amount = amount; Status = InvoiceStatus.Open; }

    public void AddComment(string text, DateTime now) => _comments.Add(new InvoiceComment(now, text));
    public void RegisterCommitment(DateOnly dueDate, decimal amount) => _commitments.Add(new PaymentCommitment(dueDate, amount));
}

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<Invoice>> SearchAsync(Guid? customerId, InvoiceStatus? status, DateOnly? dueBefore, CancellationToken ct);
}
