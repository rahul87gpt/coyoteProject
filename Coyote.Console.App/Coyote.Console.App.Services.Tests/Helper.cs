using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Coyote.Console.App.Services.Tests
{  
    internal static class Helper
    {
        internal static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IDbAsyncEnumerable<T>>()
               .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<T>(elementsAsQueryable.GetEnumerator()));

            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(elementsAsQueryable.Provider));

            //dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            return dbSetMock;
        }
    }
}
