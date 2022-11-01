using Coyote.Console.App.EntityFrameworkCore;
using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coyote.Console.App.Repository.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private CoyoteAppDBContext _context = null;
        private DbSet<T> _table = null;
        private readonly ILoggerManager _logger;
        public GenericRepository(CoyoteAppDBContext coyoteConsoleAppDBContext, ILoggerManager logger)
        {
            this._context = coyoteConsoleAppDBContext;
            _table = _context?.Set<T>();
            _logger = logger;
        }

        public void DeleteHard(int Id)
        {
            T existing = _table.Find(Id);
            if (existing != null)
            {
                _table.Remove(existing);
                // _context.SaveChanges();
            }
        }

        public void DeleteHard(IEnumerable<T> entities)
        {
            if (entities != null)
            {
                _table.RemoveRange(entities);
                // _context.SaveChanges();
            }
        }

        public async Task<T> GetById(int Id)
        {

            var obj = await _table.FindAsync(Id);
            if (obj != null)
            {
                _context.Entry(obj).State = EntityState.Detached;
                return obj;
            }
            else
                return null;
        }

        public async Task<T> GetById(long Id)
        {

            var obj = await _table.FindAsync(Id);
            if (obj != null)
            {
                _context.Entry(obj).State = EntityState.Detached;
                return obj;
            }
            else
                return null;
        }
        public async Task<T> InsertAsync(T obj)
        {
            if (obj is IAuditable<int>)
            {
                var obja = (IAuditable<int>)obj;
                obja.CreatedAt = DateTime.UtcNow;
                obja.UpdatedAt = DateTime.UtcNow;
            }
            if (obj is IAuditable<int?>)
            {
                var obja = (IAuditable<int?>)obj;
                obja.CreatedAt = DateTime.UtcNow;
                obja.UpdatedAt = DateTime.UtcNow;
            }
            await _table.AddAsync(obj);
            // _context.SaveChanges();
            return obj;
        }
        public void Update(T obj)
        {
            if (obj is IAuditable<int>)
            {
                var obja = (IAuditable<int>)obj;
                //  obja.CreatedAt = DateTime.UtcNow;
                obja.UpdatedAt = DateTime.UtcNow;
            }
            if (obj is IAuditable<int?>)
            {
                var obja = (IAuditable<int?>)obj;
                //obja.CreatedAt = DateTime.UtcNow;
                obja.UpdatedAt = DateTime.UtcNow;
            }
            _table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
            //  _context.SaveChanges();
        }
        public IQueryable<T> GetAll()
        {
            return _table.AsQueryable();
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool disableTracking = true, bool excludeDeleted = true, params Expression<Func<T, object>>[] includes)
        {
            var query = _table.AsQueryable();

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);
            if (excludeDeleted)
                query = ActiveOnly(query);

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query;
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool disableTracking = true, bool excludeDeleted = true, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var query = _table.AsQueryable();

            if (include != null)
                query = include(query);

            if (filter != null)
                query = query.Where(filter);
            if (excludeDeleted)
                query = ActiveOnly(query);

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query;
        }
        private IQueryable<T> ActiveOnly(IQueryable<T> query)
        {
            return query;
        }
        public void DetachLocal(Func<T, bool> predicate)
        {
            var local = _table.Local.Where(predicate).FirstOrDefault();
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;

            }
        }
        public void DetachLocalRow(long Id)
        {
            var obj = _table.Find(Id);
            if (obj != null)
            {
                _context.Entry(obj).State = EntityState.Detached;
            }
        }
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public async Task<DataSet> ExecuteStoredProcedure(string commandText, SqlParameter[] parameters = null)
        {
            DataSet dset = new DataSet();
            var connection = this._context.Database.GetDbConnection().ConnectionString;
            try
            {
                using (var con = new SqlConnection(this._context.Database.GetDbConnection().ConnectionString))
                {
                    await con.OpenAsync().ConfigureAwait(false);

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = commandText;
                        cmd.CommandTimeout = 380;
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Any())
                            cmd.Parameters.AddRange(parameters);

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dset);
                            return dset;
                        }


                    }

                }
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {

                throw;
            }
        }


        public DbConnection GetDBName()
        {

            var obj = _context.Database.GetDbConnection();
            if (obj != null)
            {
                return obj;
            }
            else
                return null;
        }

    }
}
