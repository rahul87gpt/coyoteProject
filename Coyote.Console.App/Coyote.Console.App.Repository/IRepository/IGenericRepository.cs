using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coyote.Console.App.Repository
{
    public interface IGenericRepository<T> where T : class
    {

        Task<T> GetById(int Id);
        Task<T> GetById(long Id);
        Task<T> InsertAsync(T ob);
        void Update(T ob);
        void DeleteHard(int Id);
        void DeleteHard(IEnumerable<T> entities);
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool disableTracking = true, bool excludeDeleted = true, params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool disableTracking = true, bool excludeDeleted = true, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        void DetachLocal(Func<T, bool> predicate);
        void DetachLocalRow(long Id);
        Task<DataSet> ExecuteStoredProcedure(string commandText, SqlParameter[] parameters = null);
        DbConnection GetDBName();
        //Task ExecuteStoredProcedure(string getAllZoneOutlet, System.Data.SqlClient.SqlParameter[] sqlParameters);
    }
}