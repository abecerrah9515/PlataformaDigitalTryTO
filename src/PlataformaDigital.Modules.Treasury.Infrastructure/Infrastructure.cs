using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PlataformaDigital.Modules.Treasury.Domain;

namespace PlataformaDigital.Modules.Treasury.Infrastructure;

public class TreasuryDbContext(DbContextOptions<TreasuryDbContext> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices => Set<Invoice>();
}

public class InvoiceRepository(TreasuryDbContext db) : IInvoiceRepository
{
    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct) => db.Invoices.FirstOrDefaultAsync(x => x.Id == id, ct)!;

    public async Task<IReadOnlyCollection<Invoice>> SearchAsync(Guid? customerId, InvoiceStatus? status, DateOnly? dueBefore, CancellationToken ct)
    {
        var query = db.Invoices.AsQueryable();
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (dueBefore.HasValue) query = query.Where(x => x.DueDate <= dueBefore.Value);
        return await query.ToListAsync(ct);
    }
}

public static class DependencyInjection
{
    public static IServiceCollection AddTreasuryInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        return services;
    }
}
