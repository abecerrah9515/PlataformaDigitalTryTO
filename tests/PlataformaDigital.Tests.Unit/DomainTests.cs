using PlataformaDigital.Modules.People.Domain;
using PlataformaDigital.Modules.Projects.Domain;
using PlataformaDigital.Modules.Treasury.Domain;

namespace PlataformaDigital.Tests.Unit;

public class DomainTests
{
    [Fact]
    public void Retirar_empleado_debe_cambiar_estado_a_former()
    {
        var employee = new Employee(Guid.NewGuid(), "Jane Doe", "jane@softtek.com");
        employee.Retire();
        Assert.Equal(EmploymentStatus.Former, employee.Status);
    }

    [Fact]
    public void Reintegrar_empleado_debe_cambiar_estado_a_active()
    {
        var employee = new Employee(Guid.NewGuid(), "Jane Doe", "jane@softtek.com");
        employee.Retire();
        employee.Reinstate();
        Assert.Equal(EmploymentStatus.Active, employee.Status);
    }

    [Fact]
    public void Agregar_comentario_factura_debe_registrar_comentario()
    {
        var invoice = new Invoice(Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), 500);
        invoice.AddComment("Llamada de seguimiento", DateTime.UtcNow);
        Assert.Single(invoice.Comments);
    }

    [Fact]
    public void Crear_ejecucion_consolidacion_debe_iniciar_en_running()
    {
        var execution = new ConsolidationExecution(Guid.NewGuid(), new Period(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31)));
        execution.Start();
        Assert.Equal(ConsolidationStatus.Running, execution.Status);
    }
}
