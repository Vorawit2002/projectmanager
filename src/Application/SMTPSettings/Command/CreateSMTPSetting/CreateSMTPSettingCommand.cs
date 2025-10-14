using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.SMTPSettings.Command.CreateSMTPSetting;
public class CreateSMTPSettingCommand : IRequest<bool>
{
    public string ConfigName { get; set; } = default!;
    public string SMTPServer { get; set; } = default!;
    public string SMTPPort { get; set; } = default!;
    public MailAuthenType SMTPAuthentication { get; set; }
    public string SMTPUserName { get; set; } = default!;
    public string SMTPPassword { get; set; } = default!;
    public bool SMTPEnableSSL { get; set; }
    public bool IsActive { get; set; }
    public string? Remark { get; set; }
}
public class CreateConfigMailCommandHandler : IRequestHandler<CreateSMTPSettingCommand, bool>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public CreateConfigMailCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task<bool> Handle(CreateSMTPSettingCommand request, CancellationToken cancellationToken)
    {
        var configMail = new SMTPSetting
        {
            ConfigName = request.ConfigName,
            SMTPServer = request.SMTPServer,
            SMTPPort = request.SMTPPort,
            SMTPAuthentication = request.SMTPAuthentication,
            SMTPUserName = request.SMTPUserName,
            SMTPPassword = request.SMTPPassword,
            SMTPEnableSSL = request.SMTPEnableSSL,
            IsActive = request.IsActive,
            Remark = request.Remark ?? string.Empty
        };

        await _applicationDbContext.SMTPSettings.AddAsync(configMail);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
