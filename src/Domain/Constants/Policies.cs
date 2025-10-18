namespace ProjectManagement.Domain.Constants;

public abstract class Policies
{
    public const string CanPurge = nameof(CanPurge);
    public const string CanManageUsers = nameof(CanManageUsers);
    public const string CanManageMasterData = nameof(CanManageMasterData);
    public const string CanViewMasterData = nameof(CanViewMasterData);
    public const string CanViewDepartmentData = nameof(CanViewDepartmentData);
    public const string CanViewOwnData = nameof(CanViewOwnData);
    public const string CanModifyData = nameof(CanModifyData);
    public const string ReadOnly = nameof(ReadOnly);
}
