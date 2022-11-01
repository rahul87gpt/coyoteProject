using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    /// <summary>
    /// Paths Service
    /// </summary>
    public class PathsService : IPathsService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;

        public PathsService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMapping;
        }

        /// <summary>
        /// Get all Paths
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<PagedOutputModel<List<PathsResponseModel>>> GetAllPaths(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Paths>();
                var paths = repository.GetAll().Include(c => c.Store).Where(x => x.IsActive == Status.Active);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                    {
                        paths = paths.Where(x => x.Description.ToLower().Contains(inputModel.GlobalFilter.ToLower()));
                    }

                    paths = paths.OrderByDescending(x => x.PathType);
                    count = paths.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        paths = paths.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                }
                List<PathsResponseModel> pathResponseModels;
                pathResponseModels = (await paths.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreatePatheMap).ToList();

                pathResponseModels =  pathResponseModels.OrderBy(x => x.PathTypeName).ThenBy(x => x.Description).ToList();

                return new PagedOutputModel<List<PathsResponseModel>>(pathResponseModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get Path by Id
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<PathsResponseModel> GetPathById(int pathId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Paths>();
                var paths = await repository.GetAll().Include(c => c.Store).Where(x => x.ID == pathId && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (paths == null)
                    throw new NotFoundException(ErrorMessages.PathNotFound.ToString(CultureInfo.CurrentCulture));

                return MappingHelpers.CreatePatheMap(paths);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Insert Path
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="pathiD"></param>
        /// <param name="userId"></param>
        /// <returns>List</returns>
        public async Task<PathsResponseModel> InsertPath(PathsRequestModel viewModel, string path, int userId)
        {

            try
            {
                if (viewModel == null)
                    throw new NullReferenceException();

                if (path == null)
                    throw new BadRequestException(ErrorMessages.PathIdReq.ToString(CultureInfo.CurrentCulture));

                var repository = _unitOfWork?.GetRepository<Paths>();

                var comm = _iAutoMapper.Mapping<PathsRequestModel, Paths>(viewModel);
                comm.PathType = viewModel.PathType;
                comm.Path = path;
                comm.Description = viewModel.Description;
                comm.OutletID = viewModel.OutletID;
                comm.IsActive = Status.Active;
                comm.CreatedBy = userId;
                comm.CreatedDate = DateTime.UtcNow;
                comm.ModifiedBy = userId;
                comm.ModifiedDate = DateTime.UtcNow;
                var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetPathById(result.ID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }

        /// <summary>
        ///  Update Path
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="PathId"></param>
        /// <param name="userId"></param>
        /// <returns>List<PathRequestModel></returns>
        /// 
        public async Task<PathsResponseModel> UpdatePath(PathsRequestModel viewModel, int id, string path, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Paths>();

                if (viewModel == null)
                    throw new NullReferenceException();

                var exists = await repository.GetAll().Where(x => x.ID == id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (exists == null)
                    throw new NotFoundException(ErrorMessages.PathNotFound.ToString(CultureInfo.CurrentCulture));


                if (path != exists.Path)
                {
                    string fileFolderPath = Directory.GetCurrentDirectory() + exists.Path;
                    if (File.Exists(exists.Path))
                    {
                        File.Delete(exists.Path);
                    }
                }
                var comm = MappingHelpers.Mapping<PathsRequestModel, Paths>(viewModel);
                comm.ID = id;
                comm.PathType = comm?.PathType ?? exists.PathType;
                comm.Path = path ?? exists.Path;
                comm.Description = comm.Description ?? exists.Description;
                comm.OutletID = comm.OutletID ?? exists.OutletID;
                comm.ModifiedBy = userId;
                comm.ModifiedDate = DateTime.UtcNow;
                comm.ModifiedBy = userId;
                comm.ModifiedDate = DateTime.UtcNow;
                comm.CreatedBy = exists.CreatedBy;
                comm.CreatedDate = exists.CreatedDate;
                comm.IsActive = exists.IsActive;
                repository.DetachLocal(_ => _.ID == comm.ID);
                repository.DetachLocal(_ => _.ID == comm.ID);
                repository.Update(comm);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetPathById(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }

        /// <summary>
        /// Delete
        /// </summary>     
        public async Task<bool> DeletePath(int Id, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Paths>();
                var exists = await repository.GetAll().Where(x => x.ID == Id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (exists == null)
                    throw new NotFoundException(ErrorMessages.PathNotFound.ToString(CultureInfo.CurrentCulture));

                exists.ModifiedBy = userId;
                exists.IsActive = Status.Deleted;
                exists.ModifiedDate = DateTime.UtcNow;
                repository?.Update(exists);

                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}

