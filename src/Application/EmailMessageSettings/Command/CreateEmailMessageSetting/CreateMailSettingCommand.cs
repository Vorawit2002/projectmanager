using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EmailMessageSettings.Command.CreateEmailMessageSetting;
public class CreateEmailMessageSettingCommand : IRequest<bool>
{
    public string SettingCode { get; init; } = default!;
    public string SettingName { get; init; } = default!;
    public string MailSubject { get; init; } = default!;
    public string MailBody { get; init; } = default!;
    public bool IsActive { get; set; }
    public string? Remark { get; init; } = default!;
}
public class CreateMailSettingCommandHandler : IRequestHandler<CreateEmailMessageSettingCommand, bool>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public CreateMailSettingCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task<bool> Handle(CreateEmailMessageSettingCommand request, CancellationToken cancellationToken)
    {
        var mailSetting = new EmailMessageSetting
        {
            SettingCode = request.SettingCode,
            SettingName = request.SettingName,
            MailSubject = request.MailSubject,
            MailBody = request.MailBody,
            IsActive = request.IsActive,
            Remark = request.Remark ?? string.Empty
        };

        await _applicationDbContext.EmailMessageSettings.AddAsync(mailSetting);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
