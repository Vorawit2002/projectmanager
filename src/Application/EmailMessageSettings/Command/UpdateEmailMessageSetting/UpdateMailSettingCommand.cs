using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.EmailMessageSettings.Command.UpdateEmailMessageSetting;
public class UpdateEmailMessageSettingCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string SettingCode { get; set; } = default!;
    public string SettingName { get; set; } = default!;
    public string MailSubject { get; set; } = default!;
    public string MailBody { get; set; } = default!;
    public bool IsActive { get; set; }
    public string? Remark { get; set; }
}
public class UpdateEmailMessageSettingCommandHandler : IRequestHandler<UpdateEmailMessageSettingCommand, bool>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public UpdateEmailMessageSettingCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task<bool> Handle(UpdateEmailMessageSettingCommand request, CancellationToken cancellationToken)
    {
        var mailSetting = await _applicationDbContext.EmailMessageSettings.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, mailSetting);
        mailSetting.SettingCode = request.SettingCode;
        mailSetting.SettingName = request.SettingName;
        mailSetting.MailSubject = request.MailSubject;
        mailSetting.MailBody = request.MailBody;
       mailSetting.IsActive = request.IsActive;
        mailSetting.Remark = request.Remark ?? string.Empty;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
