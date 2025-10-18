using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using ServiceStack.Text;
using ProjectManagement.Domain.Enums;
using CsvHelper.TypeConversion;

namespace ProjectManagement.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Clear all existing users first
        var existingUsers = await _userManager.Users.ToListAsync();
        foreach (var user in existingUsers)
        {
            await _userManager.DeleteAsync(user);
        }
        _logger.LogInformation("Deleted {Count} existing users", existingUsers.Count);

        // Default roles
        var adminRole = new IdentityRole("Admin");
        var managerRole = new IdentityRole("Manager");
        var userRole = new IdentityRole("User");
        var viewerRole = new IdentityRole("Viewer");

        if (_roleManager.Roles.All(r => r.Name != adminRole.Name))
        {
            await _roleManager.CreateAsync(adminRole);
        }
        if (_roleManager.Roles.All(r => r.Name != managerRole.Name))
        {
            await _roleManager.CreateAsync(managerRole);
        }
        if (_roleManager.Roles.All(r => r.Name != userRole.Name))
        {
            await _roleManager.CreateAsync(userRole);
        }
        if (_roleManager.Roles.All(r => r.Name != viewerRole.Name))
        {
            await _roleManager.CreateAsync(viewerRole);
        }

        // Create Admin user
        var administrator = new ApplicationUser 
        { 
            UserName = "admin", 
            Email = "admin@projectmanagement.com",
            FirstName = "Admin",
            LastName = "System",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Admin123!");
            if (!string.IsNullOrWhiteSpace(adminRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new [] { adminRole.Name });
            }
            _logger.LogInformation("Created admin user: {Username}", administrator.UserName);
        }

        // Default data
        // Seed, if necessary
        //if (!_context.TodoLists.Any())
        //{
        //    _context.TodoLists.Add(new TodoList
        //    {
        //        Title = "Todo List",
        //        Items =
        //        {
        //            new TodoItem { Title = "Make a todo list 📃" },
        //            new TodoItem { Title = "Check off the first item ✅" },
        //            new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
        //            new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
        //        }
        //    });

        //    await _context.SaveChangesAsync();
        //}
        //await Department();
        await Organization();
        await EventTypes();
        await MailSetting();
        await SMTPServer();
        await ImportCompanyGroupCsv();
    }

    public async Task Department()
    {
        using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"/items/departments.json", Encoding.UTF8))
        {
            string json = r.ReadToEnd();
            dynamic dynamicObjs = JsonConvert.DeserializeObject(json) ?? throw new Exception();
            Console.WriteLine(dynamicObjs.Count);

            foreach (var jsonObj in dynamicObjs)
            {
                string Name = (string)jsonObj.Name;
                var existTitleName = await _context.Departments.FirstOrDefaultAsync(t => t.Name == Name);
                if (existTitleName == null)
                {
                    var utcDate = DateTime.Now;
                    Department department = new Department();
                    if (jsonObj.Name != null) department.Name = jsonObj.Name;
                    if (jsonObj.IsActive != null) department.IsActive = jsonObj.IsActive;
                    //department.Created = utcDate.ToLocalTime();
                    //department.LastModified = utcDate.ToLocalTime();
                    //department.DeletedOnUtc = utcDate.ToLocalTime();
                    _context.Departments.Add(department);
                }
            }
            await _context.SaveChangesAsync();
            Console.WriteLine("Import Departments successfully.");
        }
    }

        public async Task Organization()
    {
        using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"/items/organizations.json", Encoding.UTF8))
        {
            string json = r.ReadToEnd();
            dynamic dynamicObjs = JsonConvert.DeserializeObject(json) ?? throw new Exception();
            Console.WriteLine(dynamicObjs.Count);

            foreach (var jsonObj in dynamicObjs)
            {
                string Name = (string)jsonObj.Name;
                var existTitleName = await _context.Organizations.FirstOrDefaultAsync(t => t.Name == Name);
                if (existTitleName == null)
                {
                    var utcDate = DateTime.Now;
                    Organization organization = new Organization();
                    if (jsonObj.Name != null) organization.Name = jsonObj.Name;
                    _context.Organizations.Add(organization);
                }
            }
            await _context.SaveChangesAsync();
            Console.WriteLine("Import Organization successfully.");
        }
    }
    public async Task EventTypes()
    {
        using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"/items/eventtypes.json", Encoding.UTF8))
        {
            string json = r.ReadToEnd();
            dynamic dynamicObjs = JsonConvert.DeserializeObject(json) ?? throw new Exception();
            Console.WriteLine(dynamicObjs.Count);

            foreach (var jsonObj in dynamicObjs)
            {
                string Name = (string)jsonObj.Name;
                var existTitleName = await _context.EventTypes.FirstOrDefaultAsync(t => t.Name == Name);
                if (existTitleName == null)
                {
                    var utcDate = DateTime.Now;
                    EventType eventType = new EventType();
                    if (jsonObj.Name != null) eventType.Name = jsonObj.Name;
                    if (jsonObj.EventTypeCode != null) eventType.EventTypeCode = jsonObj.EventTypeCode;
                    _context.EventTypes.Add(eventType);
                }
            }
            await _context.SaveChangesAsync();
            Console.WriteLine("Import EventTypes successfully.");
        }
    }

    public async Task MailSetting()
    {
        using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"/items/MailSetting.json", Encoding.UTF8))
        {
            string json = r.ReadToEnd();
            dynamic dynamicObjs = JsonConvert.DeserializeObject(json) ?? throw new Exception();
            if (dynamicObjs != null)
            {
                Console.WriteLine(dynamicObjs.Count);

                foreach (var jsonObj in dynamicObjs)
                {
                    string settingCode = (string)jsonObj.SettingCode;
                    var existMailSetting = await _context.EmailMessageSettings.FirstOrDefaultAsync(t => t.SettingCode == settingCode);
                    if (existMailSetting == null)
                    {
                        EmailMessageSetting newObj = new EmailMessageSetting();
                        if (jsonObj.SettingCode != null) newObj.SettingCode = jsonObj.SettingCode;
                        if (jsonObj.SettingName != null) newObj.SettingName = jsonObj.SettingName;
                        if (jsonObj.MailSubject != null) newObj.MailSubject = jsonObj.MailSubject;
                        if (jsonObj.MailBody != null) newObj.MailBody = jsonObj.MailBody;
                        if (jsonObj.IsActive != null) newObj.IsActive = jsonObj.IsActive;
                        if (jsonObj.Remark != null) newObj.Remark = jsonObj.Remark;
                        if (jsonObj.Sendtime != null) newObj.Sendtime = jsonObj.Sendtime;
                        _context.EmailMessageSettings.Add(newObj);

                    }
                }
                await _context.SaveChangesAsync();
            }
        }
    }
    public async Task SMTPServer()
    {
        using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"/items/SMTPServer.json", Encoding.UTF8))
        {
            string json = r.ReadToEnd();
            dynamic dynamicObjs = JsonConvert.DeserializeObject(json) ?? throw new Exception();
            if (dynamicObjs != null)
            {
                Console.WriteLine(dynamicObjs.Count);

                foreach (var jsonObj in dynamicObjs)
                {
                    string configName = (string)jsonObj.ConfigName;
                    var existConfigMail = await _context.SMTPSettings.FirstOrDefaultAsync(t => t.ConfigName == configName);
                    if (existConfigMail == null)
                    {
                        SMTPSetting newConfigMail = new SMTPSetting();
                        if (jsonObj.ConfigName != null) newConfigMail.ConfigName = jsonObj.ConfigName;
                        if (jsonObj.SMTPServer != null) newConfigMail.SMTPServer = jsonObj.SMTPServer;
                        if (jsonObj.SMTPPort != null) newConfigMail.SMTPPort = jsonObj.SMTPPort;
                        if (jsonObj.SMTPAuthentication != null) newConfigMail.SMTPAuthentication = jsonObj.SMTPAuthentication;
                        if (jsonObj.SMTPUserName != null) newConfigMail.SMTPUserName = jsonObj.SMTPUserName;
                        if (jsonObj.SMTPPassword != null) newConfigMail.SMTPPassword = jsonObj.SMTPPassword;
                        if (jsonObj.SMTPEnableSSL != null) newConfigMail.SMTPEnableSSL = jsonObj.SMTPEnableSSL;
                        if (jsonObj.IsActive != null) newConfigMail.IsActive = jsonObj.IsActive;
                        if (jsonObj.Remark != null) newConfigMail.Remark = jsonObj.Remark;

                        _context.SMTPSettings.Add(newConfigMail);
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
    }

    public class ProjectsCsvModel
    {
        public string ProjectCode { get; set; } = default!;
        public string ProjectName { get; set; } = default!;
        public string? ContractNumber { get; set; }
        public DateTime? ContractSignedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public decimal? ProjectCost { get; set; }
        public ProjectType? ProjectType { get; set; }
    }
    public class ProjectsCsvMap : ClassMap<ProjectsCsvModel>
    {
        public ProjectsCsvMap()
        {
            Map(m => m.ProjectCode).Name("รหัสโครงการ");
            Map(m => m.ProjectName).Name("ชื่อโครงการ(Draft)");
            Map(m => m.ProjectCost).Name("งบประมาณโครงการ").TypeConverter<DecimalCurrencyConverter>();
            Map(m => m.ContractNumber).Name("เลขที่สัญญา");
            Map(m => m.ContractSignedDate).Name("วันที่เซ็นสัญญา").TypeConverter<CustomNullableDateTimeConverter>();
            Map(m => m.StartDate).Name("วันที่เริ่มสัญญา").TypeConverter<CustomNullableDateTimeConverter>();
            Map(m => m.EndDate).Name("วันที่สิ้นสุดสัญญา").TypeConverter<CustomNullableDateTimeConverter>();
            Map(m => m.WarrantyEndDate).Name("วันที่สิ้นสุดประกัน").TypeConverter<CustomNullableDateTimeConverter>();
            //Map(m => m.ProjectType).Name("สถานะโครงการ");
        }
    }
    public async Task ImportCompanyGroupCsv()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // ✅ เพิ่มตรงนี้
        var path = Path.Combine(Directory.GetCurrentDirectory(), "items", "ProjectsList67-68.csv");
        using var reader = new StreamReader(path, Encoding.GetEncoding("windows-874"));
        using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        });
        csv.Context.RegisterClassMap<ProjectsCsvMap>();
        var records = csv.GetRecords<ProjectsCsvModel>().ToList();

        foreach (var record in records)
        {
            var exists = await _context.Projects.FirstOrDefaultAsync(cg => cg.ProjectCode == record.ProjectCode);
            if (exists == null)
            {
                var project = new Project
                {
                    ProjectCode = record.ProjectCode,
                    ProjectName = record.ProjectName,
                    ProjectCost = record.ProjectCost,
                    ContractNumber = record.ContractNumber,
                    ContractSignedDate = record.ContractSignedDate,
                    StartDate = record.StartDate,
                    EndDate = record.EndDate,
                    WarrantyEndDate = record.WarrantyEndDate,

                };
                _context.Projects.Add(project);
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Import ProjectsList67-68 CSV completed.");
    }
    // แปลงค่างบประมาณโปรเจค
    public class DecimalCurrencyConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            // ลบ $ , และเว้นวรรคออก แล้วแปลงเป็น decimal
            var cleaned = text.Replace("$", "").Replace(",", "").Trim();
            return decimal.Parse(cleaned);
        }
    }
    // แปลงค่าวันที่
    public class CustomNullableDateTimeConverter : DefaultTypeConverter
    {
        private readonly string[] _dateFormats = { "dd/MM/yyyy H:mm", "dd/MM/yyyy", "d/M/yyyy H:mm", "d/M/yyyy" };

        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (DateTime.TryParseExact(text, _dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            // ถ้าแปลงไม่สำเร็จลองแปลงแบบทั่วไป
            if (DateTime.TryParse(text, out var fallbackDate))
            {
                return fallbackDate;
            }

            throw new TypeConverterException(this, memberMapData, text, row.Context, $"Cannot convert '{text}' to DateTime.");
        }
    }
}
