using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IOrdersService
    {
        Task<PagedOutputModel<List<OrderHeaderResponseViewModel>>> GetAllActiveOrderHeaders(OrderInvoiceRequestModel input, SecurityViewModel securityViewModel);
        Task<OrderDetailResponseViewModel> GetOrderById(long id);
        Task<OrderDetailResponseViewModel> Update(OrderRequestModel viewModel, long id, int userId);
        Task<OrderDetailResponseViewModel> Insert(OrderRequestModel viewModel, int userId);
        Task<bool> DeleteOrder(long id, int userId);
        Task<bool> DeleteOrderDetailItem(long id, long itemId, int userId);
        Task EDIForLionSupplier(long orderId, string deliveryInstructions);
        Task EDIForCocaColaSupplier(long orderId, string deliveryInstructions);
        Task EDIForDistributorSupplier(long orderId, string deliveryInstructions);
        Task<long> GetNewOrderNumber(int storeId);
        Task<List<OrderDetailViewModel>> UpdateSupplierProductAsync(List<OrderDetailRequestModel> productList, int SupplierId);
        Task<OrderDetailResponseViewModel> SendOrder(OrderSendRequestModel requestModel, int userId);
        Task<OrderDetailResponseViewModel> PostOrder(OrderPostRequestModel requestModel,int userId);
        Task<OrderDetailResponseViewModel> FinishOrder(OrderFinishRequestModel requestModel, int userId);
        Task<List<OrderDetailViewModel>> RefreshOrder(List<OrderDetailRefreshRequestModel> productList, int outletId, long orderNo, int? supplierId = null);
        Task<List<OrderTabletLoadResponseModel>> TabletLoad(OrderTabletLoadRequestModel requestModel);
        Task<bool> UpliftRecalcOrder(OrderUpliftRecalcRequestModel requestModel);
        Task<AutomaticOrderResponseModel> AutomaticOrder(AutomaticOrderRequestModel viewModel, int userId);
        Task<PagedOutputModel<List<OrderHeaderResponseViewModel>>> GetActiveOrderHeaders(OrderInvoiceRequestModel inputModel, SecurityViewModel securityViewModel);
        Task<int> GetCoverDaysForOutletSupplier(int outletId, int supplierId);
        Task<OrderDetailResponseViewModel> PostOrderClick(OrderPostRequestModel requestModel, int userId);
        Task<AutoOrderDefaultSettingsResponseViewModel> GetDefaultSettingsForOutlet(int outletId,int supplierId);
    }
}
