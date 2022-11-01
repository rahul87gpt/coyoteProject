using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IUserLoggerServices
    {
        Task Insert<T>(UserLogRequestModel<T> viewModel) where T : class;
        //Task<PagedOutputModel<List<UserLogResponseModel>>> GetByTable<T>(int? id) where T : class;
        Task<PagedOutputModel<List<UserLogResponseModel<T>>>> GetUserLog<T>(int? Id, string TableName, string ModuleName, DateTime? FromDate, DateTime? ToDate) where T : class;
        Task<PagedOutputModel<List<UserLogResponseModel<T>>>> GetAuditUserLog<T>(int? Id, string TableName, string ModuleName, DateTime? FromDate, DateTime? ToDate) where T : class;
        Task<PagedOutputModel<List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>>>> GetPDELoadHistory();
        Task<PagedOutputModel<List<PDELoadHistoryResponseModel<PDELoadDataLogResponseModel>>>> GetPDELoadHistoryGetById(int Id);
        Task<PagedOutputModel<List<UserActivityResponseModel>>> GetUserActivity(PagedInputModel inputModel);
    }
}
