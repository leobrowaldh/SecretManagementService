using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;

public interface IAzureTokenService
{
    public Task<string> GetGraphApiAccessTokenAsync();
}
