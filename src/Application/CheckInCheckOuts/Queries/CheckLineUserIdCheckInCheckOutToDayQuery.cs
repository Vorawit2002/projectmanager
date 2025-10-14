using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using static ProjectManagement.Application.CheckInCheckOuts.Commands.CreateCheckInCheckOut.CreateCheckInCheckOutForLineCommandHandler;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;
public class CheckLineUserIdCheckInCheckOutToDayQuery : IRequest<CheckLineUserIdCheckInCheckOutToDayDto>
{
    public string LinebotUserId { get; set; } = default!;
}
public class CheckLineUserIdCheckInCheckOutToDayQueryHandler : IRequestHandler<CheckLineUserIdCheckInCheckOutToDayQuery, CheckLineUserIdCheckInCheckOutToDayDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMinIOService _minIOService;
    private readonly HttpClient _httpClient;
    public CheckLineUserIdCheckInCheckOutToDayQueryHandler(IApplicationDbContext context, IMapper mapper, IMinIOService minIOService, HttpClient httpClient)
    {
        _context = context;
        _mapper = mapper;
        _minIOService = minIOService;
        _httpClient = httpClient;
    }
    public async Task<CheckLineUserIdCheckInCheckOutToDayDto> Handle(CheckLineUserIdCheckInCheckOutToDayQuery request, CancellationToken cancellationToken)
    {
        var dtos = new CheckLineUserIdCheckInCheckOutToDayDto();
        string url = $"https://ntiportal.nti.co.th/GetUserid?LineUserId={request.LinebotUserId}";
        ApiResponse? result = null;
        try
        {
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", "ApiKey ntk_be7e07a95cf6406396fb4ac48757aea3");
            }
            using HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResult = await response.Content.ReadAsStringAsync();
            Console.WriteLine("ผลลัพธ์จาก API: " + jsonResult);

            // แปลง JSON เป็น object
            result = JsonConvert.DeserializeObject<ApiResponse>(jsonResult);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("เกิดข้อผิดพลาดในการเรียก API: " + ex.Message);
            throw new InvalidOperationException(ex.Message);
        }
        var emp = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == result.id!);
        Guard.Against.NotFound(result.id, emp);
        var checkInCheckOutDtos = await _context.CheckInCheckOuts
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(l => l.EmployeeId == emp.Id && l.CheckIn.Date == DateTime.Now.Date.ToLocalTime() );
     
        if (checkInCheckOutDtos == null)
        {
            dtos.IsFirstCheckinOfDay = true;
            dtos.CheckinTime = DateTime.Now;
            dtos.LinebotUserId = request.LinebotUserId;
            dtos.Message = "ยังไม่มีการ Checkin ในวันนี้";
        }
        else
        {
            dtos.IsFirstCheckinOfDay = false;
            dtos.CheckinTime = checkInCheckOutDtos.CheckIn;
            dtos.LinebotUserId = request.LinebotUserId;
            dtos.Message = "ท่านได้ Checkin แล้วในวันนี้";
        }
        return dtos;
    }
}
