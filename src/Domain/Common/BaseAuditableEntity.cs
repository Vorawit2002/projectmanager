namespace ProjectManagement.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public bool GCRecord { get; set; }
    public DateTime DeletedOnUtc { get; set; }
    public string? DeletedBy { get; set; }
}
