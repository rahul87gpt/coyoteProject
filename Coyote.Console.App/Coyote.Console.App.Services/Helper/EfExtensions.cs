using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.Helper
{
    public static class EfExtensions
    {
        public static Task<List<TSource>> ToListAsyncSafe<TSource>(
          this IQueryable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!(source is IAsyncEnumerable<TSource>))
            {
                return Task.FromResult(source.ToList());
            }

            return source.ToListAsync();
        }

        public static Task<TSource> FirstOrDefaultAsyncSafe<TSource>(
          this IQueryable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!(source is IAsyncEnumerable<TSource>))
            {
                return Task.FromResult(source.FirstOrDefault());
            }

            return source.FirstOrDefaultAsync();
        }

        public static Task<bool> AnyAsyncSafe<TSource>(
          this IQueryable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!(source is IAsyncEnumerable<TSource>))
            {
                return Task.FromResult(source.Any());
            }

            return source.AnyAsync();
        }

    }
}
