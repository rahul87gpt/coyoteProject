using Coyote.Console.App.EntityFrameworkCore;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Coyote.Console.App.Repository.Repository
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoyoteAppDBContext _databaseContext;
        private ILoggerManager _iLoggerManager;
        private Dictionary<Type, object> _repositories;
        private bool disposed = false;

        public UnitOfWork(CoyoteAppDBContext databaseContext, ILoggerManager iLog)
        {
            _databaseContext = databaseContext;
            _iLoggerManager = iLog;
        }

        /// <summary>
        /// Get the schema name
        /// </summary>
        public string SchemaName { get => _databaseContext.SchemaName; }
        /// <summary>
        /// Get the repository of entity
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Entity Repository</returns>
        public IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<TEntity>(_databaseContext, _iLoggerManager);
            }
            return (IGenericRepository<TEntity>)_repositories[type];
        }

        /// <summary>
        /// save the entity DbContext details
        /// </summary>
        /// <returns>Successfully save the records return true else false.</returns>
        public bool SaveChanges()
        {
            using (var dbContextTransaction = _databaseContext.Database.BeginTransaction())
            {
                try
                {
                    _databaseContext.SaveChanges();
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    _iLoggerManager.LogError($"Exception thrown while DbContext save changes -" + Environment.NewLine +
                           $"Message : {ex.Message}" + Environment.NewLine +
                           $"StackTrace : {ex.StackTrace}", ex);
                    return false;
                }
            }
        }
        /// <summary>
        /// save the entity DbContext details by async method
        /// </summary>
        /// <returns>Successfully save the records return true else false.</returns>
        public async Task<bool> SaveChangesAsync()
        {
            using (var dbContextTransaction = _databaseContext.Database.BeginTransaction())
            {
                try
                {
                    await _databaseContext.SaveChangesAsync().ConfigureAwait(false);
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (DbUpdateException ex)
                 {
                    dbContextTransaction.Rollback();
                    _iLoggerManager.LogError($"Exception thrown while DbContext save changes -" + Environment.NewLine +
                         $"Message : {ex.Message}" + Environment.NewLine +
                         $"StackTrace : {ex.StackTrace}", ex);
                    return false;
                }
            }
        }

        #region IDisposable Support        
        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">bool parameters</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing && _databaseContext != null)
            {
                _databaseContext.Dispose();
            }
            disposed = true;
        }

        #endregion
    }
}
