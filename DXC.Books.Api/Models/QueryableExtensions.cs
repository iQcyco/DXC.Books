using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Models;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> entities, string orderByQueryString)
    {
        if (!entities.Any())
            return entities;
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return entities;
        }

        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var orderQueryBuilder = new StringBuilder();

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;
            var propertyFromQueryName = param.Split(" ")[0];
            var objectProperty = propertyInfos.FirstOrDefault(pi =>
                pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
            if (objectProperty == null)
                continue;
            var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
            orderQueryBuilder.Append($"{objectProperty.Name} {sortingOrder}, ");
        }

        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
        return entities.OrderBy(orderQuery);
    }

    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> entities, int pageNumber,
        int pageSize)
    {
        var count = await entities.CountAsync();
        var items = await entities.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>
        {
            Data = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = count
        };
    }
}