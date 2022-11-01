using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IReportSchedulerServices
    {
        Task<ReportSchedulerResponseModel> InsertReportSchedulers(ReportSchedulerResponseModel requestModel, int userId);
        Task<ReportSchedulerResponseModel> UpdateReportSchedulers(ReportSchedulerResponseModel requestModel, long Id, int userId);
        Task<ReportSchedulerResponseModel> GetSchedulerById(long schedulerId);
        Task<ReportSchedulerResponseModel> GetActiveSchedulerById(long schedulerId, bool useFilter = false);
        Task<PagedOutputModel<List<ReportSchedulerResponseModel>>> GetAllScheduler(PagedInputModel inputModel);
        Task UpdateLastRun(long Id);
        Task SaveReportFailLog(long schedulerId, string reportName, string errorMessage = null);
        Task SaveReportEmailFailLog(long schedulerId, string reportName, byte[] report, string errorMessage = null);
        Task<bool> DeleteScheduler(int Id,int userId);
    }
}
