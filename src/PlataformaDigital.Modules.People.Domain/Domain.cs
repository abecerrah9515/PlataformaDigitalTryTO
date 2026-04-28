using PlataformaDigital.SharedKernel;

namespace PlataformaDigital.Modules.People.Domain;

public enum EmploymentStatus { Active, Former }
public enum ContractorStatus { Active, Inactive }
public enum AssetAssignmentStatus { Assigned, Unassigned }

public sealed class EmployeeAsset(Guid assetId, string name)
{
    public Guid AssetId { get; } = assetId;
    public string Name { get; } = name;
    public AssetAssignmentStatus Status { get; private set; } = AssetAssignmentStatus.Assigned;
    public void Unassign() => Status = AssetAssignmentStatus.Unassigned;
}

public sealed class AccessCard(string cardNumber)
{
    public string CardNumber { get; } = cardNumber;
    public bool IsActive { get; private set; } = true;
    public void Deactivate() => IsActive = false;
}

public sealed class EmployeeUpdateRequest(string email, string phone)
{
    public string Email { get; } = email;
    public string Phone { get; } = phone;
}

public sealed class Employee : AuditableEntity<Guid>
{
    private readonly List<EmployeeAsset> _assets = [];
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public EmploymentStatus Status { get; private set; } = EmploymentStatus.Active;
    public AccessCard? AccessCard { get; private set; }
    public IReadOnlyCollection<EmployeeAsset> Assets => _assets;

    public Employee(Guid id, string fullName, string email) { Id = id; FullName = fullName; Email = email; }
    public void Retire() { Status = EmploymentStatus.Former; foreach (var a in _assets) a.Unassign(); AccessCard?.Deactivate(); }
    public void Reinstate() { Status = EmploymentStatus.Active; }
    public void UpdateInfo(EmployeeUpdateRequest request) => Email = request.Email;
    public void AssignAsset(Guid assetId, string assetName) => _assets.Add(new EmployeeAsset(assetId, assetName));
    public void UnassignAssetsForRetirement() { foreach (var a in _assets) a.Unassign(); }
}

public sealed class FormerEmployee(Guid id, string name);
public sealed class Contractor(Guid id, string name, ContractorStatus status);

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<Employee>> GetByStatusAsync(EmploymentStatus status, CancellationToken ct);
    Task<IReadOnlyCollection<Contractor>> GetContractorsAsync(CancellationToken ct);
}
