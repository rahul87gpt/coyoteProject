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

namespace Coyote.Console.App.Services.Services
{
    public class HostUpdChangeService: IHostUpdChangeService
    {
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLoggerManager = null;
        private readonly IUnitOfWork _unitOfWork = null;


        public HostUpdChangeService(IAutoMappingServices iAutoMapperService, ILoggerManager iLoggerManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = iAutoMapperService;
            _iLoggerManager = iLoggerManager;
        }

        public async Task<PagedOutputModel<List<HOSTUPDChangeResponseModel>>> GetAllHostUpdChange(PagedInputModel inputModel, long HostId)
        {
           /* try
            {
                var repository = _unitOfWork?.GetRepository<HostUpdChange>();
                var HostUpd = repository.GetAll(x => x.HostUpdId == HostId && !x.IsDeleted);
                int count = 0;
                count= HostUpd.Count();
                List<HOSTUPDChangeResponseModel> HOSTUPDViewModels = (await HostUpd.ToListAsyncSafe().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<HostUpdChange, HOSTUPDChangeResponseModel>).ToList();
                return new PagedOutputModel<List<HOSTUPDChangeResponseModel>>(HOSTUPDViewModels, count);
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
            }*/


            try
            {
                
                var repository = _unitOfWork?.GetRepository<Product>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@HostUpdId", HostId),

            };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetHostUpdChangeByHostUpdId, dbParams.ToArray()).ConfigureAwait(false);
                List<HOSTUPDChangeResponseModel> HOSTUPDViewModels  = MappingHelpers.ConvertDataTable<HOSTUPDChangeResponseModel>(dset.Tables[0]);
               
                
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<HOSTUPDChangeResponseModel>>(HOSTUPDViewModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
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
    }
}
