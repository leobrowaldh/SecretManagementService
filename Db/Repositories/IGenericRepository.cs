using Db.DbModels;
using Db.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories;
public interface IGenericRepository<T>
{
    void SetContext(Dictionary<string, object?> contextVariables);
    void SetExecutingUser(string executingUser);
    /// <summary>
    /// Reads all items from the database with optional filtering, pagination, and tracking.
    /// </summary>
    /// <param name="flat"></param>
    /// <param name="filter"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="seeded"></param>
    /// <param name="track"></param>
    /// <returns></returns>
    Task<ResponsePage<T>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false, bool track = false);
    /// <summary>
    /// Reads all items from the database with optional filtering and tracking, but no pagination
    /// </summary>
    /// <param name="flat"></param>
    /// <param name="filter"></param>
    /// <param name="seeded"></param>
    /// <param name="track"></param>
    /// <returns></returns>
    Task<List<T>> ReadItemsAsync(bool flat, string filter, bool seeded = false, bool track = false);
    Task<T?> ReadItemAsync(Guid itemId, bool flat);
    Task<T> UpdateItemAsync(T item);
    Task<T> DeleteItemAsync(Guid itemId);
    Task<T> AddItemAsync(T item);
}
