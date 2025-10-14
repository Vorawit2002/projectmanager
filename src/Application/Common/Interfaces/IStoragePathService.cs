using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.Common.Interfaces;
public interface IStoragePathService
{    
    string GeneratePrefixPath(string subdirectory);
}
