using Db.DbModels;
using Db.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public interface IGenericRepository<T>
{
    Task SetContextAsync(Dictionary<string, object> contextVariables);
    Task<ResponsePageDto<T>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false);
    Task<T?> ReadItemAsync(Guid itemId, bool flat);
    Task<T> UpdateItemAsync(T item);
    Task<T> DeleteItemAsync(Guid itemId);
    Task<T> AddItemAsync(T item);
}
