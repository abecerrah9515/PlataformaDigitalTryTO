using MediatR;
using PlataformaDigital.Modules.Projects.Application;

namespace PlataformaDigital.Modules.Projects.Api;

public static class ProjectsEndpoints
{
    public static IEndpointRouteBuilder MapProjectsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");
        group.MapPost("/consolidations", async (CreateConsolidationExecutionCommand cmd, ISender sender) => Results.Ok(await sender.Send(cmd)));
        group.MapGet("/consolidations", async (ISender sender) => Results.Ok(await sender.Send(new GetConsolidationHistoryQuery())));
        group.MapGet("/records", async (int page, int pageSize, string? filter, ISender sender) => Results.Ok(await sender.Send(new GetConsolidatedRecordsQuery(page, pageSize, filter))));
        group.MapPost("/records/export", async (ExportConsolidatedRecordsCommand cmd, ISender sender) => Results.File((await sender.Send(cmd)).Value!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "records.xlsx"));
        group.MapPost("/consolidations/{id:guid}/errors", async (Guid id, RegisterConsolidationErrorCommand body, ISender sender) => Results.Ok(await sender.Send(body with { ExecutionId = id })));
        return app;
    }
}
