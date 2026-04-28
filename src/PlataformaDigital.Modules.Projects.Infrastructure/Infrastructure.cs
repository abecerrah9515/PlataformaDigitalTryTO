using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PlataformaDigital.BuildingBlocks;
using PlataformaDigital.Modules.Projects.Application;
using PlataformaDigital.Modules.Projects.Domain;

namespace PlataformaDigital.Modules.Projects.Infrastructure;

public class ProjectsDbContext(DbContextOptions<ProjectsDbContext> options) : DbContext(options)
{
    public DbSet<ConsolidationExecution> Executions => Set<ConsolidationExecution>();
}

public class ConsolidationExecutionRepository(ProjectsDbContext db) : IConsolidationExecutionRepository
{
    public async Task AddAsync(ConsolidationExecution execution, CancellationToken ct) => await db.Executions.AddAsync(execution, ct);
    public Task<ConsolidationExecution?> GetByIdAsync(Guid id, CancellationToken ct) => db.Executions.FirstOrDefaultAsync(x => x.Id == id, ct)!;
    public async Task<IReadOnlyCollection<ConsolidationExecution>> GetHistoryAsync(CancellationToken ct) => await db.Executions.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
}

public class ProjectsReadService : IProjectsReadService
{
    public Task<PagedResult<ConsolidatedRecord>> GetRecordsAsync(PageRequest page, string? filter, CancellationToken ct)
    {
        var records = Enumerable.Range(1, page.PageSize)
            .Select(i => new ConsolidatedRecord(Guid.NewGuid(), $"REC-{i}", new Money(100 + i, "USD")))
            .ToList();
        return Task.FromResult(new PagedResult<ConsolidatedRecord>(records, page.Page, page.PageSize, 1000));
    }
}

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IConsolidationExecutionRepository, ConsolidationExecutionRepository>();
        services.AddScoped<IProjectsReadService, ProjectsReadService>();
        return services;
    }
}
