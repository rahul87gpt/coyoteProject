using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface ISchedulerService
    {
        Task<byte[]> GetReportBySchedulerId(ReportSchedulerResponseModel scheduler);
        Task GetSchedulerInQueue();

        Task TestHangfire();
    }
}
