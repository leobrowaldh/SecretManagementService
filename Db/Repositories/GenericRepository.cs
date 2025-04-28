using Db.Dtos;
using Db.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DbRepos;

//Inherit from GenericRepository<T> and implement your specific repositories.
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public virtual async Task<ResponsePageDto<T>> ReadItemsAsync(bool flat, string filter, int pageNumber, int pageSize, bool seeded = false)
    {
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
        IQueryable<T> query = _dbSet.AsNoTracking();
        query = ApplyIncludes(query, flat);
        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == itemId);
    }

    public virtual async Task<T> AddItemAsync(T item)
    {
        _dbSet.Add(item);
        await _dbContext.SaveChangesAsync();
        return item;
    }

    public virtual async Task<T> UpdateItemAsync(T item)
    {
        _dbSet.Update(item);
        await _dbContext.SaveChangesAsync();
        return item;
    }

    public virtual async Task<T> DeleteItemAsync(Guid itemId)
    {
        var item = await ReadItemAsync(itemId, true);
        if (item == null)
            throw new ArgumentException($"Item {itemId} not found");

        _dbSet.Remove(item);
        await _dbContext.SaveChangesAsync();
        return item;
    }

    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query, bool flat)
    {
        if (flat)
            return query;

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

    // Configure your filtering for this particular entity, querying the properties you want to search in.
    protected virtual IQueryable<T> ApplyCustomFilter(IQueryable<T> query, bool seeded, string filter) => query;

}