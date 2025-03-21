using SecretManagementService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;

public interface IGraphApiService
{
    public Task<List<ExpiringSecret>?> GetExpiringSecrets(int _daysUntilSecretsExpire);
}
