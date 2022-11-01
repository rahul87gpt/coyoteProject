using Coyote.Console.App.Models.EdiMetcashModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IEDIMetcashService
    {
         Task<PlaceOrderResponseModel> PlaceOrder(PlaceOrderRequestModel placeOrderRequestModel);
         Task<string> GetOrderSummary(OrderSummaryRequestModel orderSummaryRequestModel);
        Task<ListDocumentResponseModel> ListDocumentRequest(ListDocumentRequestModel listDocumentRequestModel);
        Task<RetrieveDocumentResponseModel> GetRetrieveDocument(RetrieveDocumentRequestModel retrieveDocumentRequestModel);
        Task<NextDocumentResponseModel> GetRetrieveDocument(NextDocumentRequestModel nextDocumentRequestModel);
        Task<List<long>> InsertInvoice(string xml);
    }
}
