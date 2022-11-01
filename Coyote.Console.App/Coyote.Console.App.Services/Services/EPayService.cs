using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Coyote.Console.App.Services.Services
{
    public class EPayService : IEPayService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;

        public EPayService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMapping;
        }

        /// <summary>
        /// Get all Paths
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<PagedOutputModel<List<EPayResponseModel>>> GetAllEPay(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<EPay>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                      new SqlParameter("@PageNumber", inputModel?.SkipCount),
                      new SqlParameter("@PageSize", inputModel?.MaxResultCount),
                      new SqlParameter("@SortColumn", inputModel?.Sorting),
                      new SqlParameter("@SortDirection", inputModel?.Direction)
                };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllEPAY, dbParams.ToArray()).ConfigureAwait(false);

                List<EPayResponseModel> EpayReponseModel = new List<EPayResponseModel>();
                if (dset.Tables.Count > 0)
                {
                    EpayReponseModel = MappingHelpers.ConvertDataTable<EPayResponseModel>(dset.Tables[0]);
                }
                var count = 0;
                if (dset.Tables.Count > 1)
                {
                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                }
                return new PagedOutputModel<List<EPayResponseModel>>(EpayReponseModel, count);
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
        public async Task<EPayResponseModel> GetEPayById(int EpayId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<EPay>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                      new SqlParameter("@Id", EpayId)
                    };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllEPAY, dbParams.ToArray()).ConfigureAwait(false);
                EPayResponseModel EpayReponseModel = new EPayResponseModel();
                if (dset.Tables.Count > 0)
                {
                    EpayReponseModel = MappingHelpers.ConvertDataTable<EPayResponseModel>(dset.Tables[0]).FirstOrDefault();
                }
                var count = 0;
                if (dset.Tables.Count > 1)
                {
                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                }
                return EpayReponseModel;

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

    }
}
