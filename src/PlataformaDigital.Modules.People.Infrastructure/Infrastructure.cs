using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PlataformaDigital.Modules.People.Domain;

namespace PlataformaDigital.Modules.People.Infrastructure;

public class PeopleDbContext(DbContextOptions<PeopleDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
}

public class EmployeeRepository(PeopleDbContext db) : IEmployeeRepository
{
    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) => db.Employees.FirstOrDefaultAsync(x => x.Id == id, ct)!;
    public async Task<IReadOnlyCollection<Employee>> GetByStatusAsync(EmploymentStatus status, CancellationToken ct) => await db.Employees.Where(x => x.Status == status).ToListAsync(ct);
    public Task<IReadOnlyCollection<Contractor>> GetContractorsAsync(CancellationToken ct) => Task.FromResult((IReadOnlyCollection<Contractor>)new List<Contractor>());
}

public static class DependencyInjection
{
    public static IServiceCollection AddPeopleInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        return services;
    }
}
