using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Db.Helpers;
public static class EFPrimaryKeyHelper
{
    public static Expression<Func<T, bool>> ByPrimaryKey<T>(DbContext context, object id)
    {
        var key = context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault()
            ?? throw new InvalidOperationException($"No PK for {typeof(T).Name}");

        var param = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(param, key.Name);
        var convertedId = Expression.Convert(Expression.Constant(id), property.Type);
        var equal = Expression.Equal(property, convertedId);
        return Expression.Lambda<Func<T, bool>>(equal, param);
    }
}