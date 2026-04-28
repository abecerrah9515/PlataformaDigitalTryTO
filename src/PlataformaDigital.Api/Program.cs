using Microsoft.AspNetCore.Authentication.JwtBearer;
using PlataformaDigital.BuildingBlocks;
using PlataformaDigital.Modules.People.Api;
using PlataformaDigital.Modules.People.Infrastructure;
using PlataformaDigital.Modules.Projects.Api;
using PlataformaDigital.Modules.Projects.Infrastructure;
using PlataformaDigital.Modules.Treasury.Api;
using PlataformaDigital.Modules.Treasury.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlataformaDigital.Modules.Projects.Application.CreateConsolidationExecutionHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlataformaDigital.Modules.Treasury.Application.GetInvoicesHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlataformaDigital.Modules.People.Application.GetActiveEmployeesHandler).Assembly));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
    });
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddSingleton<IEmailService, MockEmailService>();
builder.Services.AddSingleton<IFileStorageService, MockFileStorageService>();
builder.Services.AddSingleton<IExcelExportService, MockExcelExportService>();
builder.Services.AddSingleton<IAuditService, MockAuditService>();
builder.Services.AddSingleton<ICurrentUserService, MockCurrentUserService>();

builder.Services.AddProjectsInfrastructure();
builder.Services.AddTreasuryInfrastructure();
builder.Services.AddPeopleInfrastructure();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapProjectsEndpoints();
app.MapTreasuryEndpoints();
app.MapPeopleEndpoints();

app.Run();

public class SystemDateTimeProvider : IDateTimeProvider { public DateTime UtcNow => DateTime.UtcNow; }
public class MockCurrentUserService : ICurrentUserService { public string? UserId => "system"; }
public class MockEmailService : IEmailService { public Task SendAsync(string to, string subject, string body, CancellationToken ct = default) => Task.CompletedTask; }
public class MockFileStorageService : IFileStorageService { public Task<string> SaveAsync(string fileName, Stream content, CancellationToken ct = default) => Task.FromResult($"mock://{fileName}"); }
public class MockExcelExportService : IExcelExportService { public Task<byte[]> ExportAsync<T>(IReadOnlyCollection<T> items, string sheetName, CancellationToken ct = default) => Task.FromResult(System.Text.Encoding.UTF8.GetBytes("mock")); }
public class MockAuditService : IAuditService { public Task WriteAsync(string action, string details, CancellationToken ct = default) => Task.CompletedTask; }

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        context.TraceIdentifier = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
        context.Response.Headers["X-Correlation-Id"] = context.TraceIdentifier;
        await next(context);
    }
}

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(Result.Failure(ex.Message));
        }
    }
}
