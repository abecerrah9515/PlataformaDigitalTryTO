using MediatR;
using PlataformaDigital.BuildingBlocks;
using PlataformaDigital.Modules.Treasury.Domain;

namespace PlataformaDigital.Modules.Treasury.Application;

public record GetInvoicesQuery(Guid? CustomerId, InvoiceStatus? Status, DateOnly? DueBefore) : IRequest<Result<IReadOnlyCollection<Invoice>>>;
public class GetInvoicesHandler(IInvoiceRepository repo) : IRequestHandler<GetInvoicesQuery, Result<IReadOnlyCollection<Invoice>>>
{
    public async Task<Result<IReadOnlyCollection<Invoice>>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyCollection<Invoice>>.Success(await repo.SearchAsync(request.CustomerId, request.Status, request.DueBefore, cancellationToken));
}

public record AddInvoiceCommentCommand(Guid InvoiceId, string Comment) : IRequest<Result>;
public class AddInvoiceCommentHandler(IInvoiceRepository repo, IDateTimeProvider clock) : IRequestHandler<AddInvoiceCommentCommand, Result>
{
    public async Task<Result> Handle(AddInvoiceCommentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await repo.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null) return Result.Failure("Invoice not found");
        invoice.AddComment(request.Comment, clock.UtcNow);
        return Result.Success();
    }
}

public record RegisterPaymentCommitmentCommand(Guid InvoiceId, DateOnly DueDate, decimal Amount) : IRequest<Result>;
public class RegisterPaymentCommitmentHandler(IInvoiceRepository repo) : IRequestHandler<RegisterPaymentCommitmentCommand, Result>
{
    public async Task<Result> Handle(RegisterPaymentCommitmentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await repo.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null) return Result.Failure("Invoice not found");
        invoice.RegisterCommitment(request.DueDate, request.Amount);
        return Result.Success();
    }
}

public record SendOverdueNotificationCommand(Guid InvoiceId, string RecipientEmail) : IRequest<Result>;
public class SendOverdueNotificationHandler(IInvoiceRepository repo, IEmailService email) : IRequestHandler<SendOverdueNotificationCommand, Result>
{
    public async Task<Result> Handle(SendOverdueNotificationCommand request, CancellationToken cancellationToken)
    {
        if (await repo.GetByIdAsync(request.InvoiceId, cancellationToken) is null) return Result.Failure("Invoice not found");
        await email.SendAsync(request.RecipientEmail, "Factura vencida", "Existe una factura vencida", cancellationToken);
        return Result.Success();
    }
}

public record ExportInvoicesCommand(Guid? CustomerId, InvoiceStatus? Status, DateOnly? DueBefore) : IRequest<Result<byte[]>>;
public class ExportInvoicesHandler(IInvoiceRepository repo, IExcelExportService excel) : IRequestHandler<ExportInvoicesCommand, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(ExportInvoicesCommand request, CancellationToken cancellationToken)
    {
        var invoices = await repo.SearchAsync(request.CustomerId, request.Status, request.DueBefore, cancellationToken);
        return Result<byte[]>.Success(await excel.ExportAsync(invoices, "Invoices", cancellationToken));
    }
}
