using Db.Dtos;
using Db.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DbRepos;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<T> _dbSet;
    private Dictionary<string, object?> _sessionContext = new();

    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public void SetContext(Dictionary<string, object?> sessionContext)
    {
        _sessionContext = sessionContext;
    }

    public virtual async Task<ResponsePageDto<T>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false)
    {
        await ConfigureContext();

        filter ??= "";
        filter = filter.ToLower();

        IQueryable<T> query = _dbSet.AsNoTracking();
        query = ApplyIncludes(query, flat);
        query = ApplyCustomFilter(query, seeded, filter);

        var totalCount = await query.CountAsync();
        var pageItems = await query.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();

        return new ResponsePageDto<T>
        {
            DbItemsCount = totalCount,
            PageItems = pageItems,
            PageNr = pageNumber,
            PageSize = pageSize
        };
    }

    public virtual async Task<T?> ReadItemAsync(Guid itemId, bool flat)
    {
        await ConfigureContext();

        IQueryable<T> query = _dbSet.AsNoTracking();
        query = ApplyIncludes(query, flat);
        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == itemId);
    }

    public virtual async Task<T> AddItemAsync(T item)
    {
        await ConfigureContext();

        _dbSet.Add(item);
        await _dbContext.SaveChangesAsync();
        return item;
    }

    public virtual async Task<T> UpdateItemAsync(T item)
    {
        await ConfigureContext();

        _dbSet.Update(item);
        await _dbContext.SaveChangesAsync();
        return item;
    }

    public virtual async Task<T> DeleteItemAsync(Guid itemId)
    {
        await ConfigureContext();

        var item = await ReadItemAsync(itemId, true);
        if (item == null)
            throw new ArgumentException($"Item {itemId} not found");

        _dbSet.Remove(item);
        await _dbContext.SaveChangesAsync();
        return item;
    }

    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query, bool flat)
    {
        if (flat) return query;

        var navigations = _dbContext.Model.FindEntityType(typeof(T))?
                            .GetNavigations()
                            .Select(n => n.Name);

        if (navigations != null)
        {
            foreach (var navigation in navigations)
            {
                query = query.Include(navigation);
            }
        }

        return query;
    }

    protected virtual IQueryable<T> ApplyCustomFilter(IQueryable<T> query, bool seeded, string filter) => query;

    protected virtual async Task ConfigureContext()
    {
        if (_sessionContext == null || _sessionContext.Count == 0)
            return;

        var connection = (SqlConnection)_dbContext.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        foreach (var (key, value) in _sessionContext)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "EXEC sp_set_session_context @key, @value";
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@value", value ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }
    }

}
