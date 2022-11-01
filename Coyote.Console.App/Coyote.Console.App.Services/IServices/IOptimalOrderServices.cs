using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.Services.IServices
{
    public interface IOptimalOrderServices
    {
        PagedOutputModel<List<OptimalOrderBatchResponseModel>> GetOptimalBatch(int? OutletId, int? OrderNo, DateTime? OrderDate);
    }
}
