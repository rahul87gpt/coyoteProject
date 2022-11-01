using Coyote.Console.App.Models.EdiMetcashModels;
using EDIServiceReference;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.Services.Helper.EdiMetcashExtensions
{
    public static class AuthenticationExtension
    {
        public static EDIServiceReference.Authentication ToAPIModel(this Authentic authentication)
        {
            return new EDIServiceReference.Authentication
            {
                B2BAccount = authentication?.B2BAccount,
                Password = authentication.Password,
                SecurityToken = authentication.SecurityToken
            };
        }
    }
    public static class VendorExtension
    {
        /// <summary>
        /// Parse vendordtetailmodel to Api model
        /// </summary>
        /// <param name="vendorDetails"></param>
        /// <returns></returns>
        public static EDIServiceReference.VendorDetails ToAPIModel(this VendorDetailsModel vendorDetails)
        {
            if (vendorDetails != null)
            {
                return new EDIServiceReference.VendorDetails
                {
                    Software = vendorDetails.Software,
                    Vendor = vendorDetails.Vendor,
                    Version = vendorDetails.Version
                };
            }
            else
            {
                return new EDIServiceReference.VendorDetails()
                {
                    Software = "",
                    Vendor = "",
                    Version = ""
                };
            }
        }
    }
    public static class OperatingSystemExtension
    {
        /// <summary>
        /// Parse OperatingSystemModel to Api model
        /// </summary>
        /// <param name="OperatingSystem"></param>
        /// <returns></returns>
        public static EDIServiceReference.OperatingSystem ToAPIModel(this OperatingSystemModel operatingSystem)
        {
            if (operatingSystem != null)
            {
                return new EDIServiceReference.OperatingSystem
                {
                    ServicePack = operatingSystem.ServicePack,
                    SystemName = operatingSystem.SystemName,
                    SystemType = operatingSystem.SystemType,
                    Version = operatingSystem.Version
                };
            }
            else
            {
                return new EDIServiceReference.OperatingSystem()
                {
                    ServicePack = "",
                    SystemName = "",
                    SystemType = "",
                    Version = ""
                };
            }
        }
    }
    public static class PlaceOrderExtension
    {


        /// <summary>
        /// Parse PlaceOrderModel to Api model
        /// </summary>
        /// <param name="placeOrderRequest"></param>
        /// <returns></returns>
        public static EDIServiceReference.placeOrderRequest ToAPIModel(this PlaceOrderModel placeOrderRequestModel)
        {
            return new EDIServiceReference.placeOrderRequest
            {
                CustomerId = placeOrderRequestModel?.CustomerId,
                StateCode = placeOrderRequestModel.StateCode,
                PillarId = placeOrderRequestModel.PillarId,
                OrderType = placeOrderRequestModel.OrderType,
                Order = placeOrderRequestModel.Order,
                fileName = placeOrderRequestModel.fileName
            };
        }

        public static PlaceOrderResponseModel ToEDIModel(this EDIServiceReference.placeOrderResponse placeOrderResponse)
        {
            return new PlaceOrderResponseModel
            {
                BatchId = placeOrderResponse?.orderSummary.batchId,
                Comments = placeOrderResponse.orderSummary.comments,
                Timestamp = placeOrderResponse.orderSummary.timestamp
            };
        }
    }
    public static class OrderSummaryExtension
    {
        public static getOrderSummaryRequest ToAPIModel(this OrderSummayModel orderSummayModel)
        {
            return new getOrderSummaryRequest
            {
                CustomerId = orderSummayModel?.CustomerId,
                StateCode = orderSummayModel.StateCode,
                PillarId = orderSummayModel.PillarId,
                batchId = orderSummayModel.BatchId,
                timeStamp = orderSummayModel.Timestamp,
            };
        }

        public static string ToEDIModel(this getOrderSummaryResponse getOrderSummaryResponse)
        {
            return getOrderSummaryResponse?.response;
        }
    }
    public static class ListDocumentExtension
    {
        public static listDocumentsReq ToAPIModel(this ListDocumentModel listDocumentModel)
        {
            return new listDocumentsReq
            {
                CustomerId = listDocumentModel?.CustomerId,
                StateCode = listDocumentModel.StateCode,
                DateFrom = listDocumentModel.DateFrom,
                DateTo = listDocumentModel.DateTo,
                HostType = listDocumentModel.HostType,
                ListRetrievedFlagSpecified = listDocumentModel.ListRetrievedFlagSpecified,
                DocumentType = listDocumentModel.DocumentType,
                DocumentReference = GetDocumentReference(listDocumentModel.DocumentReference),
                ListRetrievedFlag = listDocumentModel.ListRetrievedFlag,
                PillarId = listDocumentModel.PillarId,
            };
        }

        public static ListDocumentResponseModel ToEDIModel(this listDocumentsResponse listDocumentsResponse)
        {
            var data = new ListDocumentResponseModel
            {
                Authentication = new Models.EdiMetcashModels.Authentic
                {
                    B2BAccount = listDocumentsResponse?.Authentication.B2BAccount,
                    Password = listDocumentsResponse.Authentication.Password,
                    SecurityToken = listDocumentsResponse.Authentication.SecurityToken
                },
            };

            List<ListAllDocument> listDocuments = new List<ListAllDocument>();
            foreach (var item in listDocumentsResponse.listDocumentsResp)
            {
                listDocuments.Add(new ListAllDocument
                {
                    CustomerId = item.CustomerId,
                    DocDate = item.DocDate,
                    DocDateSpecified = item.DocDateSpecified,
                    DocumentGUID = item.DocumentGUID,
                    DocumentType = item.DocumentType,
                    DocumentReference = GetDocumentReferences(item.DocumentReference),
                    FileName = item.FileName,
                    HostType = item.HostType,
                });
            }

            data.ListAllDocuments = listDocuments.ToArray();

            return data;

        }

        private static Models.EdiMetcashModels.DocumentReference[] GetDocumentReferences(EDIServiceReference.DocumentReference[] documentReferences)
        {
            var documents = new List<Models.EdiMetcashModels.DocumentReference>();
            if (documentReferences != null)
            {
                foreach (var item in documentReferences)
                {
                    documents.Add(new Models.EdiMetcashModels.DocumentReference
                    {
                        Type = item.type,
                        Value = item.Value
                    });
                }
            }
            return documents.ToArray();
        }

        private static EDIServiceReference.DocumentReference GetDocumentReference(Models.EdiMetcashModels.DocumentReference documentReference)
        {
            var document = new EDIServiceReference.DocumentReference();
            document.type = documentReference?.Type;
            document.Value = documentReference?.Value;
            return document;
        }
    }
    public static class RetrieveDocumentExtension
    {
        public static retrieveDocumentReq ToAPIModel(this RetrieveDocumentModel retrieveDocumentModel)
        {
            return new retrieveDocumentReq
            {
                DocumentGUID = retrieveDocumentModel?.DocumentGUID,
                FileName = retrieveDocumentModel.FileName,
                DocumentVersion = retrieveDocumentModel.DocumentVersion,
                FlagAsCollected = retrieveDocumentModel.FlagAsCollected,
                FlagAsCollectedSpecified = retrieveDocumentModel.FlagAsCollectedSpecified,
                TransactionId = retrieveDocumentModel.TransactionId,
                ZipFlag = retrieveDocumentModel.ZipFlag,
                ZipFlagSpecified = retrieveDocumentModel.ZipFlagSpecified,
            };
        }

        public static RetrieveDocumentResponseModel ToEDIModel(this retrieveDocumentResp retrieveDocumentResp)
        {
            return new RetrieveDocumentResponseModel
            {
                DocumentGUID = retrieveDocumentResp?.DocumentGUID,
                Payload = retrieveDocumentResp.Payload,
                ZipFlag = retrieveDocumentResp.ZipFlag,
                Digest = retrieveDocumentResp.Digest,
                DocumentSize = retrieveDocumentResp.DocumentSize,
                DocumentVersion = retrieveDocumentResp.DocumentVersion
            };
        }
    }
    public static class NextDocumentExtension
    {
        public static getNextDocumentReq ToAPIModel(this NextDocumentModel nextDocumentModel)
        {
            return new getNextDocumentReq
            {
                CustomerId = nextDocumentModel?.CustomerId,
                StateCode = nextDocumentModel.StateCode,
                PillarId = nextDocumentModel.PillarId,
                HostType = nextDocumentModel.HostType,
                DocumentType = nextDocumentModel.DocumentType,
                DocumentReference = GetDocumentReference(nextDocumentModel.DocumentReference)
            };
        }

        private static EDIServiceReference.DocumentReference GetDocumentReference(Models.EdiMetcashModels.DocumentReference documentReference)
        {
            var document = new EDIServiceReference.DocumentReference();
            document.type = documentReference?.Type;
            document.Value = documentReference?.Value;
            return document;
        }

        public static NextDocumentResponseModel ToEDIModel(this getNextDocumentResp getNextDocumentResp)
        {
            return new NextDocumentResponseModel
            {
                DocumentGUID = getNextDocumentResp?.DocumentGUID,
                Payload = getNextDocumentResp.Payload,
                ZipFlag = getNextDocumentResp.ZipFlag,
                Digest = getNextDocumentResp.Digest,
                DocumentSize = getNextDocumentResp.DocumentSize,
                DocumentVersion = getNextDocumentResp.DocumentVersion
            };
        }
    }
}
