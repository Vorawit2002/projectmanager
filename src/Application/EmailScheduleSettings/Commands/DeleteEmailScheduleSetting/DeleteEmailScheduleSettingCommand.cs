using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.EmailLogs.Commands.DeleteEmailLog;
using ProjectManagement.Domain.Entities;
using Hangfire;
using Hangfire.States;

namespace ProjectManagement.Application.EmailScheduleSettings.Commands.DeleteEmailScheduleSetting;
public record DeleteEmailScheduleSettingCommand(Guid Id) : IRequest<bool>;
public class DeleteEmailScheduleSettingCommandHandler : IRequestHandler<DeleteEmailScheduleSettingCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;

    public DeleteEmailScheduleSettingCommandHandler(IApplicationDbContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }
    public async Task<bool> Handle(DeleteEmailScheduleSettingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.EmailScheduleSettings
           .Where(l => l.Id == request.Id)
           .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.EmailScheduleSettings.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        string jobId = entity.HangfireJobId ?? string.Empty;
        if (!string.IsNullOrEmpty(jobId))
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var job = connection.GetJobData(jobId);
                if (job.State != SucceededState.StateName)
                {
                    if (job != null) // The job exists
                    {
                        // Delete the existing job
                        BackgroundJob.Delete(jobId);
                    }
                }
            }
        }

        return true;
    }
}
