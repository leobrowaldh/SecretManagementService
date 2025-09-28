using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.DbModels;

namespace Db.Repositories;

public interface ISecretRepository : IGenericRepository<Secret>
{
    Task<List<Secret>> GetExpiringSecrets(int daysUntilExpiration);
}
