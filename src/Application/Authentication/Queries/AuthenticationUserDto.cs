using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Authentication.Queries;
public class AuthenticationUserDto
{
    public string? UserName { get; set; }
    public Guid? Id { get; set; }
    public string? DisplayName { get; set; }
    public IList<string> RoleNames { get; set; } = new List<string>();
}

