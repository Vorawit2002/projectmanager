using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Infrastructure.FileStorage;
public class StoragePathService : IStoragePathService
{
    public string GeneratePrefixPath(string subdirectory)
    {
        string tailPath = GenerateTimePath();
        string combindedPath = $"{tailPath}/{subdirectory}";
        if (!Directory.Exists(combindedPath))
        {
            Directory.CreateDirectory(combindedPath);
        }
        return combindedPath;
    }
    private string GenerateTimePath()
    {
        string year = DateTime.Now.Year.ToString();
        string month = DateTime.Now.Month.ToString();
        string timePath = $"{year}/{month}";
        return timePath;
    }
}
