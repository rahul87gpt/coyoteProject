using Coyote.Console.App.Models;
using Coyote.Console.App.Models.EdiMetcashModels;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// EDI service
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EDIMetcashController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IEDIMetcashService EDIMetcashService { get; }
        /// <summary>
        /// logger
        /// </summary>
        public ILoggerManager iLogger { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EDIMetcashService"></param>
        /// <param name="logger"></param>

        public EDIMetcashController(IEDIMetcashService EDIMetcashService, ILoggerManager logger)
        {
            this.EDIMetcashService = EDIMetcashService;
            iLogger = logger;
        }
        // GET api/values
        /// <summary>    
        /// Where applicable- Method specific to store placing an order    
        /// </summary>    
        /// <returns>data</returns>  
        [HttpPost("PlaceOrder")]
        public async Task<ActionResult<PlaceOrderResponseModel>> PlaceOrder(PlaceOrderRequestModel placeOrderRequestModel)
        {
            return CreatedAtAction("PlaceOrder", await EDIMetcashService.PlaceOrder(placeOrderRequestModel).ConfigureAwait(false));

        }

        /// <summary>
        /// Where applicable- Method to return an order confirmation
        /// </summary>
        /// <param name="orderSummaryRequestModel"></param>
        [HttpPost("GetOrderSummary")]
        public async Task<ActionResult<string>> GetOrderSummary(OrderSummaryRequestModel orderSummaryRequestModel)
        {
            return CreatedAtAction("GetOrderSummary", await EDIMetcashService.GetOrderSummary(orderSummaryRequestModel).ConfigureAwait(false));
        }
        /// <summary>
        /// Insert Invoice
        /// </summary>
        /// <param name="content">XML content</param>
        /// <returns></returns>
        [HttpPost("InsertInvoice")]
        public async Task<ActionResult<ListDocumentResponseModel>> InsertInvoice([FromForm] string content)
        {
            try
            {
                return CreatedAtAction("InsertInvoice", await EDIMetcashService.InsertInvoice(content).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }

            catch (BadRequestException ex)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(ex.Message));
            }
            catch (Exception ex)
            {
                iLogger.LogError("OrdersController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Lists all available document based on parameters used
        /// </summary>
        /// <param name="listDocumentRequestModel"></param>
        /// <returns></returns>
        [HttpPost("ListDocument")]
        public async Task<ActionResult<ListDocumentResponseModel>> ListDocumentRequest(ListDocumentRequestModel listDocumentRequestModel)
        {
            return new ObjectResult(await EDIMetcashService.ListDocumentRequest(listDocumentRequestModel).ConfigureAwait(false)) { StatusCode = 200 };
        }

        /// <summary>
        /// Retrieves all available documents based on parameters used
        /// </summary>
        /// <param name="retrieveDocumentRequestModel"></param>
        /// <returns></returns>
        [HttpPost("RetrieveDocument")]
        public async Task<ActionResult<RetrieveDocumentResponseModel>> GetRetrieveDocument(RetrieveDocumentRequestModel retrieveDocumentRequestModel)
        {
            return new ObjectResult(await EDIMetcashService.GetRetrieveDocument(retrieveDocumentRequestModel).ConfigureAwait(false)) { StatusCode = 200 };
        }

        /// <summary>
        /// Retrieves the next available document- one retrieved per call
        /// </summary>
        /// <param name="nextDocumentRequestModel"></param>
        /// <returns></returns>
        [HttpPost("GetNextDocument")]
        public async Task<ActionResult<NextDocumentResponseModel>> GetNextDocument(NextDocumentRequestModel nextDocumentRequestModel)
        {
            return new ObjectResult(await EDIMetcashService.GetRetrieveDocument(nextDocumentRequestModel).ConfigureAwait(false)) { StatusCode = 200 };
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

    }
}
