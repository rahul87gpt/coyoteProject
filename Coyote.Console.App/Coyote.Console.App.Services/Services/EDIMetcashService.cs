using Coyote.Console.App.Models;
using Coyote.Console.App.Models.EdiMetcashModels;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper.EdiMetcashExtensions;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using EDIServiceReference;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.Services
{
    public class EDIMetcashService : IEDIMetcashService
    {
        private IUnitOfWork _unitOfWork = null;
        public EDIMetcashService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PlaceOrderResponseModel> PlaceOrder(PlaceOrderRequestModel placeOrderRequestModel)
        {
            RWSv7Services rWSv7Services = new RWSv7ServicesClient();
            PlaceOrderModel placeOrderModel = placeOrderRequestModel?.PlaceOrderModel;
            placeOrderModel.Order = MultilineText(placeOrderRequestModel?.PlaceOrderModel?.Order);
            placeOrderRequest1 listDocumentsInternalRequest = new placeOrderRequest1
            {
                Authentication = placeOrderRequestModel.Authentication.ToAPIModel(),
                OperatingSystem = placeOrderRequestModel.operatingSystemModel.ToAPIModel(),
                VendorDetails = placeOrderRequestModel.vendorDetailsModel.ToAPIModel(),
                placeOrderRequest = placeOrderRequestModel.PlaceOrderModel.ToAPIModel(),
            };
            try
            {
                var response = await rWSv7Services.placeOrderAsync(listDocumentsInternalRequest).ConfigureAwait(false);
                return response.placeOrderResponse.ToEDIModel();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }
        private string MultilineText(string singleLine)
        {
            var strArray = singleLine.Split('\n');
            var multi = new StringBuilder();
            foreach (var item in strArray)
            {
                multi.AppendFormat(item).AppendLine();
            }
            return multi.ToString();
        }
        public async Task<string> GetOrderSummary(Models.EdiMetcashModels.OrderSummaryRequestModel orderSummaryRequestModel)
        {
            RWSv7Services rWSv7Services = new RWSv7ServicesClient();
            getOrderSummaryRequest1 getOrderSummaryRequest = new getOrderSummaryRequest1
            {
                Authentication = orderSummaryRequestModel?.Authentication.ToAPIModel(),
                OperatingSystem = orderSummaryRequestModel.operatingSystemModel.ToAPIModel(),
                VendorDetails = orderSummaryRequestModel.vendorDetailsModel.ToAPIModel(),
                getOrderSummaryRequest = orderSummaryRequestModel.OrderSummayModel.ToAPIModel()
            };

            try
            {
                var response = await rWSv7Services.getOrderSummaryAsync(getOrderSummaryRequest).ConfigureAwait(false);
                return response.getOrderSummaryResponse.ToEDIModel();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<long>> InsertInvoice(string xml)
        {
            var result = new List<long>();
            //var invoiceBatch = EDIMetcashInvoceHelpers.ParseXMLToInvoiceModel(@"E:\ArjunThakur\Projects\coyote\Console\cdn-coyote-core\Coyote.Console.App\Coyote.Console.App.Services\62999999-EINV7--98918-20190523212127.XML");
            var invoiceBatch = EDIMetcashInvoceHelpers.GenrateInvoiceText(xml);

            var InvoiceOrderHeaderRepo = _unitOfWork.GetRepository<OrderHeader>();
            var InvoiceOrderDetailRepo = _unitOfWork.GetRepository<OrderDetail>();
            var MasterListItemsRepo = _unitOfWork.GetRepository<MasterListItems>();
            var SupplierProductRepo = _unitOfWork.GetRepository<SupplierProduct>();
            var OutletSuppRepo = _unitOfWork.GetRepository<OutletSupplierSetting>();
            var TypeInvoice = MasterListItemsRepo?.GetAll()?.Include(x => x.MasterList).Where(x => x.MasterList.Code == "OrderDocType" && x.Code == "INVOICE").FirstOrDefault();
            var StatusInvoice = MasterListItemsRepo?.GetAll()?.Include(x => x.MasterList).Where(x => x.MasterList.Code == "OrderDocStatus" && x.Code == "NEW").FirstOrDefault();
            var OrderType = MasterListItemsRepo?.GetAll()?.Include(x => x.MasterList).Where(x => x.MasterList.Code == "ORDERDETAILTYPE" && x.Code == "NORMALBUY").FirstOrDefault();
            var CreationType = MasterListItemsRepo?.GetAll()?.Include(x => x.MasterList).Where(x => x.MasterList.Code == "OrderCreationType" && x.Code == "MANUAL").FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (invoiceBatch != null)
            {
                foreach (var item in invoiceBatch.Invoice)
                {
                    //Invoice header line data
                    string headerLine = $"H,{item.InvoiceNumber},{item.Header.CustomerNumber},{item.Header.OrderNumber},{item.Header.InvoiceDate},{item.Header.CustomerName}";
                    sb.AppendLine(headerLine);
                    var outletSuppSett = OutletSuppRepo.GetAll(x => x.CustomerNumber == item.Header.CustomerNumber).FirstOrDefault();
                    if (outletSuppSett == null)
                        throw new NotFoundException("Customer not found with customer number " + item.Header.CustomerNumber);
                    long maxOrderId = InvoiceOrderHeaderRepo.GetAll(x => x.OutletId == outletSuppSett.StoreId).Max(x => x.OrderNo);
                    var OrderHeader = new OrderHeader
                    {
                        InvoiceNo = item?.InvoiceNumber,
                        Reference = item.Header.OrderNumber,
                        InvoiceDate = item.Header.InvoiceDate,
                        StatusId = StatusInvoice.Id,
                        TypeId = TypeInvoice.Id,
                        OrderNo = ++maxOrderId,
                        OutletId = outletSuppSett == null ? 0 : outletSuppSett.StoreId,
                        SupplierId = outletSuppSett.SupplierId,
                        InvoiceTotal = item.Trailer.Total,
                        CreatedAt = DateTime.UtcNow,
                        CreatedById = 1,
                        GstAmt = item.Trailer.TotalGST,
                        CreatedDate = DateTime.UtcNow,
                        CreationTypeId = CreationType.Id,
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedById = 1,
                        OrderDetail = new List<OrderDetail>()
                    };
                    var LineNo = 1;
                    //Invoice Details Lines
                    foreach (var itemDetail in item.Detail)
                    {
                        string detailLine = $"D,{item.InvoiceNumber},{itemDetail.InvoicedProductCode},{itemDetail.PackQuantity},{itemDetail.UnitsOrdered},{itemDetail.UnitsInvoiced},{itemDetail.LandedUnitCost * itemDetail.PackQuantity},{itemDetail.GSTAmount},{itemDetail.CostTotal},{itemDetail.GTINPLUS.GTINPLUUnit.GTINPLU.Number},{itemDetail.ProductDescription}";
                        sb.AppendLine(detailLine);
                        var suppProduct = SupplierProductRepo.GetAll(x => x.SupplierItem == itemDetail.InvoicedProductCode && x.SupplierId == 3).FirstOrDefault();
                        if (suppProduct == null)
                            throw new NotFoundException("Supplier product not found for number " + itemDetail.InvoicedProductCode);
                        OrderHeader.OrderDetail.Add(
                            new OrderDetail
                            {
                                OrderTypeId = OrderType.Id,
                                LineNo = LineNo++,
                                ProductId = suppProduct.ProductId,
                                CartonQty = itemDetail.PackQuantity,
                                CartonCost = itemDetail.LandedUnitCost * itemDetail.PackQuantity,
                                Cartons = itemDetail.UnitsOrdered / itemDetail.PackQuantity,
                                Units = itemDetail.UnitsInvoiced,
                                TotalUnits = itemDetail.UnitsOrdered,
                                LineTotal = itemDetail.CostTotal,
                                FinalCartonCost = itemDetail.CostTotal,
                                FinalLineTotal = itemDetail.CostTotal,
                                NewProduct = false,
                                CreatedAt = DateTime.UtcNow,
                                CreatedById = 1,
                                IsDeleted = false,
                                UpdatedAt = DateTime.UtcNow,
                                UpdatedById = 1
                            });
                        //need to discuss
                        //{itemDetail.SupplierCode}=SuppProdNo,{itemDetail.PackQuantity}=CtnQty,{itemDetail.CostTotal}=CtnCost,,{itemDetail.LineNumber}=LineTotal,{itemDetail.InvoicedProductCode}=APN
                    }
                    //Invoice Trailer line data
                    string trailerLine = $"T,{item.InvoiceNumber},{item.Trailer.Total},00000.00,00000.00,00000.00,00000.00,00000.00";
                    sb.AppendLine(trailerLine);
                    var order = await InvoiceOrderHeaderRepo.InsertAsync(OrderHeader).ConfigureAwait(false);
                    result.Add(OrderHeader.OrderNo);
                }
                if (result.Count == 0)
                    throw new NotFoundException(ErrorMessages.InvoiceOrderNotFound.ToString(CultureInfo.CurrentCulture));
                if (await _unitOfWork.SaveChangesAsync().ConfigureAwait(false))
                    return result;
            }
            return null;
        }
        public async Task<Models.EdiMetcashModels.ListDocumentResponseModel> ListDocumentRequest(Models.EdiMetcashModels.ListDocumentRequestModel listDocumentRequestModel)
        {
            RWSv7Services rWSv7Services = new RWSv7ServicesClient();

            listDocumentsInternalRequest listDocumentsReq = new listDocumentsInternalRequest
            {
                Authentication = listDocumentRequestModel?.Authentication.ToAPIModel(),
                OperatingSystem = listDocumentRequestModel.operatingSystemModel.ToAPIModel(),
                VendorDetails = listDocumentRequestModel.vendorDetailsModel.ToAPIModel(),
                listDocumentsReq = listDocumentRequestModel.ListDocumentModel.ToAPIModel(),
            };

            try
            {
                var response = await rWSv7Services.listDocumentsAsync(listDocumentsReq).ConfigureAwait(false);
                return response.ToEDIModel();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw;
            }

        }
        public async Task<RetrieveDocumentResponseModel> GetRetrieveDocument(RetrieveDocumentRequestModel retrieveDocumentRequestModel)
        {
            RWSv7Services rWSv7Services = new RWSv7ServicesClient();
            retrieveDocumentRequest retrieveDocumentRequest = new retrieveDocumentRequest
            {
                Authentication = retrieveDocumentRequestModel?.Authentication.ToAPIModel(),
                OperatingSystem = retrieveDocumentRequestModel.operatingSystemModel.ToAPIModel(),
                VendorDetails = retrieveDocumentRequestModel.vendorDetailsModel.ToAPIModel(),
                retrieveDocumentReq = retrieveDocumentRequestModel.RetrieveDocumentModel.ToAPIModel()
            };

            try
            {
                var response = await rWSv7Services.retrieveDocumentAsync(retrieveDocumentRequest).ConfigureAwait(false);
                return response.retrieveDocumentResp.ToEDIModel();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<NextDocumentResponseModel> GetRetrieveDocument(NextDocumentRequestModel nextDocumentRequestModel)
        {
            RWSv7Services rWSv7Services = new RWSv7ServicesClient();
            getNextDocumentRequest nextDocumentRequest = new getNextDocumentRequest
            {
                Authentication = nextDocumentRequestModel?.Authentication?.ToAPIModel(),
                OperatingSystem = nextDocumentRequestModel.operatingSystemModel.ToAPIModel(),
                VendorDetails = nextDocumentRequestModel.vendorDetailsModel.ToAPIModel(),
                getNextDocumentReq = nextDocumentRequestModel.NextDocumentModel.ToAPIModel()
            };

            var response = await rWSv7Services.getNextDocumentAsync(nextDocumentRequest).ConfigureAwait(false);

            return response.getNextDocumentResp.ToEDIModel();
        }
    }
}