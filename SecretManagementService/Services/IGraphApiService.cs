using SecretManagementService.Models;
using SecretManagementService.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Services;

public interface IGraphApiService
{
    public Task<GraphApiGenericResponse<GraphApiApplicationResponse>> GetAppDataAsync();
}
