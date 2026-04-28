using FluentValidation;
using MediatR;
using PlataformaDigital.BuildingBlocks;
using PlataformaDigital.Modules.Projects.Domain;

namespace PlataformaDigital.Modules.Projects.Application;

public record CreateConsolidationExecutionCommand(DateOnly From, DateOnly To) : IRequest<Result<Guid>>;
public class CreateConsolidationExecutionValidator : AbstractValidator<CreateConsolidationExecutionCommand>
{
    public CreateConsolidationExecutionValidator() => RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
}

public class CreateConsolidationExecutionHandler(IConsolidationExecutionRepository repo) : IRequestHandler<CreateConsolidationExecutionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateConsolidationExecutionCommand request, CancellationToken cancellationToken)
    {
        var execution = new ConsolidationExecution(Guid.NewGuid(), new Period(request.From, request.To));
        execution.Start();
        await repo.AddAsync(execution, cancellationToken);
        return Result<Guid>.Success(execution.Id);
    }
}

public record GetConsolidationHistoryQuery() : IRequest<Result<IReadOnlyCollection<ConsolidationExecution>>>;
public class GetConsolidationHistoryHandler(IConsolidationExecutionRepository repo) : IRequestHandler<GetConsolidationHistoryQuery, Result<IReadOnlyCollection<ConsolidationExecution>>>
{
    public async Task<Result<IReadOnlyCollection<ConsolidationExecution>>> Handle(GetConsolidationHistoryQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyCollection<ConsolidationExecution>>.Success(await repo.GetHistoryAsync(cancellationToken));
}

public interface IProjectsReadService
{
    Task<PagedResult<ConsolidatedRecord>> GetRecordsAsync(PageRequest page, string? filter, CancellationToken ct);
}
public record GetConsolidatedRecordsQuery(int Page, int PageSize, string? Filter) : IRequest<Result<PagedResult<ConsolidatedRecord>>>;
public class GetConsolidatedRecordsHandler(IProjectsReadService read) : IRequestHandler<GetConsolidatedRecordsQuery, Result<PagedResult<ConsolidatedRecord>>>
{
    public async Task<Result<PagedResult<ConsolidatedRecord>>> Handle(GetConsolidatedRecordsQuery request, CancellationToken cancellationToken)
        => Result<PagedResult<ConsolidatedRecord>>.Success(await read.GetRecordsAsync(new PageRequest(request.Page, request.PageSize), request.Filter, cancellationToken));
}

public record ExportConsolidatedRecordsCommand(string? Filter) : IRequest<Result<byte[]>>;
public class ExportConsolidatedRecordsHandler(IProjectsReadService read, IExcelExportService excel) : IRequestHandler<ExportConsolidatedRecordsCommand, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(ExportConsolidatedRecordsCommand request, CancellationToken cancellationToken)
    {
        var data = await read.GetRecordsAsync(new PageRequest(1, 5000), request.Filter, cancellationToken);
        return Result<byte[]>.Success(await excel.ExportAsync(data.Items, "ConsolidatedRecords", cancellationToken));
    }
}

public record RegisterConsolidationErrorCommand(Guid ExecutionId, string Message, string RecordKey) : IRequest<Result>;
public class RegisterConsolidationErrorHandler(IConsolidationExecutionRepository repo, IDateTimeProvider clock) : IRequestHandler<RegisterConsolidationErrorCommand, Result>
{
    public async Task<Result> Handle(RegisterConsolidationErrorCommand request, CancellationToken cancellationToken)
    {
        var exec = await repo.GetByIdAsync(request.ExecutionId, cancellationToken);
        if (exec is null) return Result.Failure("Execution not found");
        exec.RegisterError(request.Message, request.RecordKey, clock.UtcNow);
        return Result.Success();
    }
}
