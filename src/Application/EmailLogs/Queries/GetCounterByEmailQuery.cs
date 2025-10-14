using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.EmailLogs.Queries;
public record GetCounterByEmailQuery(string Email) : IRequest<int>;
public class GetCounterByEmailQueryHandle : IRequestHandler<GetCounterByEmailQuery, int>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetCounterByEmailQueryHandle(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<int> Handle(GetCounterByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.EmailLogs.Where(x => x.SentTo == request.Email).CountAsync();
    }
}
