using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PaginatedListAdmin<T> : List<T>
{
    public int PageIndexAdmin { get; private set; }
    public int TotalPagesAdmin { get; private set; }

    public PaginatedListAdmin(List<T> items, int count, int pageIndexAdmin, int pageSize)
    {
        PageIndexAdmin = pageIndexAdmin;
        TotalPagesAdmin = (int)Math.Ceiling(count / (double)pageSize);
        this.AddRange(items);
    }

    public bool HasPreviousPage => PageIndexAdmin > 1;
    public bool HasNextPage => PageIndexAdmin < TotalPagesAdmin;

    public static async Task<PaginatedListAdmin<T>> CreateAsync(IQueryable<T> source, int pageIndexAdmin, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndexAdmin - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedListAdmin<T>(items, count, pageIndexAdmin, pageSize);
    }
}
