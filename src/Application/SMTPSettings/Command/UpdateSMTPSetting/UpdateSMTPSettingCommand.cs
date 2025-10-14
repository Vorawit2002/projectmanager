using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.SMTPSettings.Command.UpdateSMTPSetting;
public class UpdateSMTPSettingCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string ConfigName { get; set; } = default!;
    public string SMTPServer { get; set; } = default!;
    public string SMTPPort { get; set; } = default!;
    public MailAuthenType SMTPAuthentication { get; set; } = default!;
    public string SMTPUserName { get; set; } = default!;
    public string SMTPPassword { get; set; } = default!;
    public bool SMTPEnableSSL { get; set; } = default!;
    public bool IsActive { get; set; } = default!;
    public string Remark { get; set; } = default!;
}
public class UpdateConfigMailCommandHandler : IRequestHandler<UpdateSMTPSettingCommand, bool>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public UpdateConfigMailCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task<bool> Handle(UpdateSMTPSettingCommand request, CancellationToken cancellationToken)
    {
        var configMail = await _applicationDbContext.SMTPSettings.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (configMail == null) { return false; }
        configMail.ConfigName = request.ConfigName;
        configMail.SMTPServer = request.SMTPServer;
        configMail.SMTPPort = request.SMTPPort;
        configMail.SMTPAuthentication = request.SMTPAuthentication;
        configMail.SMTPUserName = request.SMTPUserName;
        configMail.SMTPPassword = request.SMTPPassword;
        configMail.SMTPEnableSSL = request.SMTPEnableSSL;
        configMail.IsActive = request.IsActive;
        configMail.Remark = request.Remark;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
