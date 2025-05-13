using Db.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public interface IEmailRepository : IGenericRepository<Email>
{
    Task<List<Email>> GetEmailsByApplicationIdAsync(Guid applicationId);
}

