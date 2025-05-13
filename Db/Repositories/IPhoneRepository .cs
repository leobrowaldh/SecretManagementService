using Db.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public interface IPhoneRepository : IGenericRepository<Phone>
{
    Task<List<Phone>> GetPhonesByApplicationIdAsync(Guid applicationId);
}
