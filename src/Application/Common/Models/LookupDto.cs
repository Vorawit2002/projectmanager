using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            // Add mappings here if needed
        }
    }
}
