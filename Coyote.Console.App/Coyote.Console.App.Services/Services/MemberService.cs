using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using FastMember;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.Services
{
  public class MemberService : IMemberService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAutoMappingServices _iAutoMapper = null;
    private readonly IUserLoggerServices _userLogger;

    public MemberService(IUnitOfWork unitOfWork, IAutoMappingServices iAutoMapper, IUserLoggerServices userLogger)
    {
      _unitOfWork = unitOfWork;
      _iAutoMapper = iAutoMapper;
      _userLogger = userLogger;
    }
    public async Task<PagedOutputModel<List<MemberResponseModel>>> GetAllActiveMembers(MemberFilter inputModel, SecurityViewModel securityViewModel)
    {
      try
      {
        var repository = _unitOfWork?.GetRepository<Members>();
        List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                new SqlParameter("@Number", inputModel?.Number),
                new SqlParameter("@SortColumn", inputModel?.Sorting),
                new SqlParameter("@SortDirection", inputModel?.Direction),
                new SqlParameter("@SkipCount", inputModel?.SkipCount),
                new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
            };
        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetALLActiveMembers, dbParams.ToArray()).ConfigureAwait(false);
        List<MemberResponseModel> membersViewModel = MappingHelpers.ConvertDataTable<MemberResponseModel>(dset.Tables[0]);
        var count = 0;
        count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
        return new PagedOutputModel<List<MemberResponseModel>>(membersViewModel, count);

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
