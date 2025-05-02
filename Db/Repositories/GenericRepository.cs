using Db.Dtos;
using Db.Helpers;
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
    private string _executingUser = string.Empty;

    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public void SetContext(Dictionary<string, object?> sessionContext)
    {
        _sessionContext = sessionContext;
    }
    public void SetExecutingUser(string executingUser)
    {
        _executingUser = executingUser;
    }

    public virtual async Task<ResponsePageDto<T>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false)
    {
        await SqlQueryInjector.ApplySessionContextAsync(_dbContext.Database.GetDbConnection(), _sessionContext);
        await SqlQueryInjector.ExecuteAsUserAsync(_dbContext.Database.GetDbConnection(), _executingUser);

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
        await SqlQueryInjector.ApplySessionContextAsync(_dbContext.Database.GetDbConnection(), _sessionContext);
        await SqlQueryInjector.ExecuteAsUserAsync(_dbContext.Database.GetDbConnection(), _executingUser);

        IQueryable<T> query = _dbSet.AsNoTracking();
        query = ApplyIncludes(query, flat);
        var predicate = EFPrimaryKeyHelper.ByPrimaryKey<T>(_dbContext, itemId);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T> AddItemAsync(T item)
    {
        await SqlQueryInjector.ApplySessionContextAsync(_dbContext.Database.GetDbConnection(), _sessionContext);
        await SqlQueryInjector.ExecuteAsUserAsync(_dbContext.Database.GetDbConnection(), _executingUser);

        _dbSet.Add(item);
        await _dbContext.SaveChangesAsync();
        return item;
    }

    public virtual async Task<T> UpdateItemAsync(T item)
    {
        await SqlQueryInjector.ApplySessionContextAsync(_dbContext.Database.GetDbConnection(), _sessionContext);
        await SqlQueryInjector.ExecuteAsUserAsync(_dbContext.Database.GetDbConnection(), _executingUser);

        _dbSet.Update(item);
        await _dbContext.SaveChangesAsync();
        return item;
    }

    public virtual async Task<T> DeleteItemAsync(Guid itemId)
    {
        await SqlQueryInjector.ApplySessionContextAsync(_dbContext.Database.GetDbConnection(), _sessionContext);
        await SqlQueryInjector.ExecuteAsUserAsync(_dbContext.Database.GetDbConnection(), _executingUser);

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

}
