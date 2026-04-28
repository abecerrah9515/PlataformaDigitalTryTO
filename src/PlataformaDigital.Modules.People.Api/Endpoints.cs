using MediatR;
using PlataformaDigital.Modules.People.Application;

namespace PlataformaDigital.Modules.People.Api;

public static class PeopleEndpoints
{
    public static IEndpointRouteBuilder MapPeopleEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/people").WithTags("People");
        g.MapGet("/employees/active", async (int page, int pageSize, ISender sender) => Results.Ok(await sender.Send(new GetActiveEmployeesQuery(page, pageSize))));
        g.MapGet("/employees/former", async (ISender sender) => Results.Ok(await sender.Send(new GetFormerEmployeesQuery())));
        g.MapGet("/contractors", async (ISender sender) => Results.Ok(await sender.Send(new GetContractorsQuery())));
        g.MapPost("/employees/{employeeId:guid}/retire", async (Guid employeeId, ISender sender) => Results.Ok(await sender.Send(new RetireEmployeeCommand(employeeId))));
        g.MapPost("/employees/{employeeId:guid}/reinstate", async (Guid employeeId, ISender sender) => Results.Ok(await sender.Send(new ReinstateEmployeeCommand(employeeId))));
        g.MapPut("/employees/{employeeId:guid}", async (Guid employeeId, UpdateEmployeeCommand cmd, ISender sender) => Results.Ok(await sender.Send(cmd with { EmployeeId = employeeId })));
        g.MapPost("/employees/{employeeId:guid}/assets", async (Guid employeeId, AssignAssetCommand cmd, ISender sender) => Results.Ok(await sender.Send(cmd with { EmployeeId = employeeId })));
        g.MapPost("/employees/{employeeId:guid}/assets/unassign", async (Guid employeeId, ISender sender) => Results.Ok(await sender.Send(new UnassignAssetsByRetirementCommand(employeeId))));
        g.MapPost("/emails/bulk-update/schedule", async (ScheduleBulkUpdateEmailCommand cmd, ISender sender) => Results.Ok(await sender.Send(cmd)));
        return app;
    }
}
