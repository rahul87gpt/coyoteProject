using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services
{
  public class APNServices : IAPNServices
  {

    private readonly IAutoMappingServices _iAutoMapper = null;
    private readonly ILoggerManager _iLoggerManager = null;
    private readonly IUnitOfWork _unitOfWork = null;

    public APNServices(IAutoMappingServices iAutoMapperService, ILoggerManager iLoggerManager, IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
      _iAutoMapper = iAutoMapperService;
      _iLoggerManager = iLoggerManager;
    }

    /// <summary>
    /// Get all active APNs.
    /// </summary>
    /// <returns></returns>
    public async Task<PagedOutputModel<List<APNResponseViewModel>>> GetAllActiveAPN(APNFilter filter = null)
    {
      try
      {
        var repository = _unitOfWork?.GetRepository<APN>();
        var apn = repository.GetAll(x => !x.IsDeleted, includes: new Expression<Func<APN, object>>[] { c => c.Product });
        int count = 0;
        if (filter != null)
        {
          if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
            apn = apn.Where(x => x.Number.ToString().ToLower().Contains(filter.GlobalFilter.ToLower()) || x.ProductId.ToString().ToLower().Contains(filter.GlobalFilter.ToLower()));

          if (!string.IsNullOrEmpty((filter?.Number)))
          {
            var prodId = await apn.Where(x => x.Number.ToString().ToLower().Equals(filter.Number.ToLower())).Select(x => x.ProductId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (prodId > 0)

            {
              apn = apn.Where(x => x.ProductId == prodId);
            }
            else
            {
              apn = apn.Where(x => x.Number.ToString().ToLower().Equals(filter.Number.ToLower()));
            }
          }
          if (!string.IsNullOrEmpty((filter?.ProductId)))
            apn = apn.Where(x => x.ProductId.ToString().ToLower().Equals(filter.ProductId.ToLower()));

          if (filter.SoldDateFrom.HasValue)
            apn = apn.Where(x => x.SoldDate >= filter.SoldDateFrom.Value);
          if (filter.SoldDateTo.HasValue)
            apn = apn.Where(x => x.SoldDate <= filter.SoldDateTo.Value);
          count = apn.Count();
          if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
            apn = apn.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);
          switch (filter.Sorting?.ToLower(CultureInfo.CurrentCulture))
          {
            case "number":
              if (string.IsNullOrEmpty(filter.Direction))
                apn = apn.OrderBy(x => x.Number);
              else
                apn = apn.OrderByDescending(x => x.Number);
              break;
            case "solddate":
              if (string.IsNullOrEmpty(filter.Direction))
                apn = apn.OrderBy(x => x.SoldDate);
              else
                apn = apn.OrderByDescending(x => x.SoldDate);
              break;
            case "productid":
              if (string.IsNullOrEmpty(filter.Direction))
                apn = apn.OrderBy(x => x.ProductId);
              else
                apn = apn.OrderByDescending(x => x.ProductId);
              break;
            default:
              if (string.IsNullOrEmpty(filter.Direction))
                apn = apn.OrderBy(x => x.Id);
              else
                apn = apn.OrderByDescending(x => x.Id);
              break;
          }
        }
        List<APNResponseViewModel> aPNViewModels = (await apn.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).OrderByDescending(x => x.UpdatedAt).ToList();
        return new PagedOutputModel<List<APNResponseViewModel>>(aPNViewModels, count);
      }
      catch (NotFoundException)
      {
        throw new NotFoundException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
      }
      catch (Exception ex)
      {
        if (ex is NotFoundException)
        {
          throw new NotFoundException(ex.Message);
        }
        throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
      }
    }
    /// <summary>
    /// Get all active APNs.
    /// </summary>
    /// <returns></returns>
    public async Task<PagedOutputModel<List<APNResponseViewModel>>> GetActiveAPN(APNFilter filter = null)
    {
      try
      {
        var repository = _unitOfWork?.GetRepository<APN>();
        int count = 0;

        List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", filter?.GlobalFilter),
                        new SqlParameter("@Number", filter?.Number),
                        new SqlParameter("@ProductId", filter?.ProductId),
                        new SqlParameter("@SoldDateFrom", filter?.SoldDateFrom),
                        new SqlParameter("@SoldDateTo", filter?.SoldDateTo),
                        new SqlParameter("@SkipCount", filter?.SkipCount),
                        new SqlParameter("@MaxResultCount", filter?.MaxResultCount),
                        new SqlParameter("@SortColumn", filter?.Sorting),
                        new SqlParameter("@SortDirection", filter?.Direction),

                    };
        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveAPN, dbParams.ToArray()).ConfigureAwait(false);
        List<APNResponseViewModel> aPNViewModels = MappingHelpers.ConvertDataTable<APNResponseViewModel>(dset.Tables[0]);
        count = Convert.ToInt32(dset.Tables[1].Rows[0]["RecordCount"]);
        return new PagedOutputModel<List<APNResponseViewModel>>(aPNViewModels, count);
      }
      catch (NotFoundException)
      {
        throw new NotFoundException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
      }
      catch (Exception ex)
      {
        if (ex is NotFoundException)
        {
          throw new NotFoundException(ex.Message);
        }
        throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
      }
    }
    /// <summary>
    /// Get a specific Active APN using APN id.
    /// </summary>
    /// <param name="apnId">APN Id</param>
    /// <returns></returns>
    public async Task<APNResponseViewModel> GetAPNById(long apnId)
    {
      try
      {
        if (apnId > 0)
        {
          var repository = _unitOfWork?.GetRepository<APN>();
          var apn = await repository.GetAll(x => x.Id == apnId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
          APNResponseViewModel aPNViewModels = _iAutoMapper.Mapping<APN, APNResponseViewModel>(apn);
          if (aPNViewModels == null)
          {
            throw new NotFoundException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
          }
          return aPNViewModels;
        }
        throw new NullReferenceException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
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

    public async Task<bool> DeleteAPN(long apnId, int userId)
    {
      try
      {
        if (apnId > 0)
        {
          var repository = _unitOfWork?.GetRepository<APN>();
          var exists = await repository.GetById(apnId).ConfigureAwait(false);
          if (exists != null && !exists.IsDeleted)
          {
            //return await repository.Delete(Id).ConfigureAwait(false);
            exists.IsDeleted = true;
            exists.UpdatedById = userId;

            repository?.Update(exists);
            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
            return true;
          }
          throw new NullReferenceException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
        }
        throw new NullReferenceException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
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

    public async Task<APNResponseViewModel> Update(APNRequestModel viewModel, long id, int userId)
    {
      try
      {
        APNResponseViewModel responseModel = new APNResponseViewModel();
        if (viewModel != null)
        {
          if (id == 0)
          {
            throw new NullReferenceException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
          }
          var repository = _unitOfWork?.GetRepository<APN>();
          var exists = await repository.GetById(id).ConfigureAwait(false);
          if (exists != null)
          {
            if (exists.IsDeleted == true)
            {
              throw new NotFoundException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
            }
            if ((await repository.GetAll(x => x.Number == viewModel.Number && !x.IsDeleted && x.Id != id).AnyAsync().ConfigureAwait(false)))
            {
              throw new AlreadyExistsException(ErrorMessages.APNNumbDuplicate.ToString(CultureInfo.CurrentCulture));
            }
            var prodRpository = _unitOfWork?.GetRepository<Product>();
            if (!(await prodRpository.GetAll(x => x.Id == viewModel.ProductId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
            {
              throw new AlreadyExistsException(ErrorMessages.APNProdNotExist.ToString(CultureInfo.CurrentCulture));
            }

            var apn = _iAutoMapper.Mapping<APNRequestModel, APN>(viewModel);
            apn.Id = id;
            apn.CreatedAt = exists.CreatedAt;
            apn.CreatedById = exists.CreatedById;
            apn.UpdatedAt = DateTime.UtcNow;
            apn.UpdatedById = userId;
            apn.IsDeleted = false;
            repository.DetachLocal(_ => _.Id == apn.Id);
            repository.Update(apn);
            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
            responseModel = await GetAPNById(apn.Id).ConfigureAwait(false);
            return responseModel;
          }
          throw new NotFoundException(ErrorMessages.APNNotFound.ToString(CultureInfo.CurrentCulture));
        }
        throw new NullReferenceException();
      }
      catch (Exception ex)
      {
        if (ex is AlreadyExistsException)
        {
          throw new AlreadyExistsException(ex.Message);
        }
        if (ex is NullReferenceException)
        {
          throw new NullReferenceException(ex.Message);
        }
        if (ex is NotFoundException)
        {
          throw new NotFoundException(ex.Message);
        }
        throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
      }
    }

    public async Task<APNResponseViewModel> Insert(APNRequestModel viewModel, int userId)
    {
      long resultId = 0;
      try
      {
        APNResponseViewModel responseModel = new APNResponseViewModel();
        if (viewModel != null)
        {
          var repository = _unitOfWork?.GetRepository<APN>();
          if ((await repository.GetAll(x => x.Number == viewModel.Number && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
          {
            throw new AlreadyExistsException(ErrorMessages.APNNumbDuplicate.ToString(CultureInfo.CurrentCulture));
          }

          var prodRpository = _unitOfWork?.GetRepository<Product>();
          if (!(await prodRpository.GetAll(x => x.Id == viewModel.ProductId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
          {
            throw new AlreadyExistsException(ErrorMessages.APNProdNotExist.ToString(CultureInfo.CurrentCulture));
          }
          var apn = _iAutoMapper.Mapping<APNRequestModel, APN>(viewModel);
          apn.IsDeleted = false;
          apn.CreatedAt = DateTime.UtcNow;
          apn.UpdatedAt = DateTime.UtcNow;
          apn.CreatedById = userId;
          apn.UpdatedById = userId;
          apn.Status = true;
          var result = await repository.InsertAsync(apn).ConfigureAwait(false);
          await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
          if (result != null)
          {
            resultId = result.Id;
            responseModel = await GetAPNById(resultId).ConfigureAwait(false);
          }
        }
        return responseModel;
      }
      catch (Exception ex)
      {
        if (ex is AlreadyExistsException)
        {
          throw new AlreadyExistsException(ex.Message);
        }
        if (ex is BadRequestException)
        {
          throw new BadRequestException(ex.Message);
        }
        if (ex is NullReferenceException)
        {
          throw new NullReferenceException(ex.Message);
        }
        throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
      }
    }




  }
}

