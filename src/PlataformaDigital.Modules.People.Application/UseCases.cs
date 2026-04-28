using MediatR;
using PlataformaDigital.BuildingBlocks;
using PlataformaDigital.Modules.People.Domain;

namespace PlataformaDigital.Modules.People.Application;

public record GetActiveEmployeesQuery(int Page, int PageSize) : IRequest<Result<PagedResult<Employee>>>;
public class GetActiveEmployeesHandler(IEmployeeRepository repo) : IRequestHandler<GetActiveEmployeesQuery, Result<PagedResult<Employee>>>
{
    public async Task<Result<PagedResult<Employee>>> Handle(GetActiveEmployeesQuery request, CancellationToken cancellationToken)
    {
        var all = await repo.GetByStatusAsync(EmploymentStatus.Active, cancellationToken);
        var items = all.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return Result<PagedResult<Employee>>.Success(new PagedResult<Employee>(items, request.Page, request.PageSize, all.Count));
    }
}

public record GetFormerEmployeesQuery() : IRequest<Result<IReadOnlyCollection<Employee>>>;
public class GetFormerEmployeesHandler(IEmployeeRepository repo) : IRequestHandler<GetFormerEmployeesQuery, Result<IReadOnlyCollection<Employee>>>
{
    public async Task<Result<IReadOnlyCollection<Employee>>> Handle(GetFormerEmployeesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyCollection<Employee>>.Success(await repo.GetByStatusAsync(EmploymentStatus.Former, cancellationToken));
}

public record GetContractorsQuery() : IRequest<Result<IReadOnlyCollection<Contractor>>>;
public class GetContractorsHandler(IEmployeeRepository repo) : IRequestHandler<GetContractorsQuery, Result<IReadOnlyCollection<Contractor>>>
{
    public async Task<Result<IReadOnlyCollection<Contractor>>> Handle(GetContractorsQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyCollection<Contractor>>.Success(await repo.GetContractorsAsync(cancellationToken));
}

public record RetireEmployeeCommand(Guid EmployeeId) : IRequest<Result>;
public class RetireEmployeeHandler(IEmployeeRepository repo) : IRequestHandler<RetireEmployeeCommand, Result>
{
    public async Task<Result> Handle(RetireEmployeeCommand request, CancellationToken cancellationToken)
    {
        var e = await repo.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (e is null) return Result.Failure("Employee not found");
        e.Retire(); return Result.Success();
    }
}

public record ReinstateEmployeeCommand(Guid EmployeeId) : IRequest<Result>;
public class ReinstateEmployeeHandler(IEmployeeRepository repo) : IRequestHandler<ReinstateEmployeeCommand, Result>
{
    public async Task<Result> Handle(ReinstateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var e = await repo.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (e is null) return Result.Failure("Employee not found");
        e.Reinstate(); return Result.Success();
    }
}

public record UpdateEmployeeCommand(Guid EmployeeId, string Email, string Phone) : IRequest<Result>;
public class UpdateEmployeeHandler(IEmployeeRepository repo) : IRequestHandler<UpdateEmployeeCommand, Result>
{
    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var e = await repo.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (e is null) return Result.Failure("Employee not found");
        e.UpdateInfo(new EmployeeUpdateRequest(request.Email, request.Phone)); return Result.Success();
    }
}

public record AssignAssetCommand(Guid EmployeeId, Guid AssetId, string AssetName) : IRequest<Result>;
public class AssignAssetHandler(IEmployeeRepository repo) : IRequestHandler<AssignAssetCommand, Result>
{
    public async Task<Result> Handle(AssignAssetCommand request, CancellationToken cancellationToken)
    {
        var e = await repo.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (e is null) return Result.Failure("Employee not found");
        e.AssignAsset(request.AssetId, request.AssetName); return Result.Success();
    }
}

public record UnassignAssetsByRetirementCommand(Guid EmployeeId) : IRequest<Result>;
public class UnassignAssetsByRetirementHandler(IEmployeeRepository repo) : IRequestHandler<UnassignAssetsByRetirementCommand, Result>
{
    public async Task<Result> Handle(UnassignAssetsByRetirementCommand request, CancellationToken cancellationToken)
    {
        var e = await repo.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (e is null) return Result.Failure("Employee not found");
        e.UnassignAssetsForRetirement(); return Result.Success();
    }
}

public record ScheduleBulkUpdateEmailCommand(DateTime ScheduleAt) : IRequest<Result>;
public class ScheduleBulkUpdateEmailHandler : IRequestHandler<ScheduleBulkUpdateEmailCommand, Result>
{
    public Task<Result> Handle(ScheduleBulkUpdateEmailCommand request, CancellationToken cancellationToken) => Task.FromResult(Result.Success());
}
