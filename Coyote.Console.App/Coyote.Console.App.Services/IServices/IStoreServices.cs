using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IStoreServices
    {
        Task<bool> DeleteStore(int Id, int userId);
        Task<PagedOutputModel<List<StoreResponseModel>>> GetAllActiveStores(SecurityViewModel securityViewModel, PagedInputModel filter = null, int? groupId = null);
        Task<StoreResponseModel> GetStoreById(int storeId);
        Task<StoreResponseModel> Update(StoreRequestModel viewModel, int storeId, int userId);
        Task<StoreResponseModel> Insert(StoreRequestModel viewModel, int userId);
        Task<PagedOutputModel<List<StoreResponseModel>>> GetActiveStores(SecurityViewModel securityViewModel, PagedInputModel filter = null, int? groupId = null);

        Task<KPIReportResponseModel> KPIStorReport(KPIReportInputModel inputModel);
        Task<WeeklySalesWorkbookResponseModel> WeeklySalesWorkbook(WeeklySalesWrokBookRequestModel inputModel);

        Task<bool> ResetLastRun(int outletSupplierId, int userId);
        Task<StoreResponseModel> GetActiveStoreById(int storeId);
        Task<PagedOutputModel<List<AnonymousStoreResponseModel>>> GetActiveStoresForDigS(PagedInputModel filter = null);
        Task<AnonymousStoreResponseModel> GetActiveStoreByIdForDigS(int Id);
    }
}
