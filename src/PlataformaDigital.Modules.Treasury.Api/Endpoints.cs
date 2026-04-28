using MediatR;
using PlataformaDigital.Modules.Treasury.Application;
using PlataformaDigital.Modules.Treasury.Domain;

namespace PlataformaDigital.Modules.Treasury.Api;

public static class TreasuryEndpoints
{
    public static IEndpointRouteBuilder MapTreasuryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/treasury").WithTags("Treasury");
        group.MapGet("/invoices", async (Guid? customerId, InvoiceStatus? status, DateOnly? dueBefore, ISender sender)
            => Results.Ok(await sender.Send(new GetInvoicesQuery(customerId, status, dueBefore))));
        group.MapPost("/invoices/{invoiceId:guid}/comments", async (Guid invoiceId, AddInvoiceCommentCommand cmd, ISender sender)
            => Results.Ok(await sender.Send(cmd with { InvoiceId = invoiceId })));
        group.MapPost("/invoices/{invoiceId:guid}/commitments", async (Guid invoiceId, RegisterPaymentCommitmentCommand cmd, ISender sender)
            => Results.Ok(await sender.Send(cmd with { InvoiceId = invoiceId })));
        group.MapPost("/invoices/{invoiceId:guid}/notifications/overdue", async (Guid invoiceId, SendOverdueNotificationCommand cmd, ISender sender)
            => Results.Ok(await sender.Send(cmd with { InvoiceId = invoiceId })));
        group.MapPost("/invoices/export", async (ExportInvoicesCommand cmd, ISender sender)
            => Results.File((await sender.Send(cmd)).Value!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "invoices.xlsx"));
        return app;
    }
}
