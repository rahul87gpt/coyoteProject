using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Controllers
{
    /// <summary>
    /// ProductController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [PermissionAuthorize]
    public class ProductController : Controller
    {
        private readonly IProductService _iProductService = null;
        private IImageUploadHelper _iImageUploader = null;
        private readonly ILoggerManager _iLogger = null;

        /// <summary>
        /// ProductController
        /// </summary>
        /// <param name="iProductService"></param>
        /// <param name="imageUploadHelper"></param>
        /// <param name="logger"></param>
        public ProductController(IProductService iProductService, IImageUploadHelper imageUploadHelper, ILoggerManager logger)
        {
            _iLogger = logger;
            this._iProductService = iProductService;
            _iImageUploader = imageUploadHelper;
        }

        /// <summary>
        /// Get all active Products.
        /// </summary>
        /// <returns>List of ProductViewModel</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductFilter inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<ProductResponseModel>> products = await _iProductService.GetActiveProducts(inputModel, securityViewModel).ConfigureAwait(false);
                return Ok(products);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get all active Products.
        /// </summary>
        /// <returns>List of ProductViewModel</returns>
        [HttpGet("GetActiveProducts")]
        [ProducesResponseType(typeof(List<ProductResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveProducts([FromQuery] ProductFilter inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<ProductResponseModel>> products = await _iProductService.GetActiveProducts(inputModel, securityViewModel).ConfigureAwait(false);
                return Ok(products);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get all Replicate Products.
        /// </summary>
        /// <returns>List of ProductViewModel</returns>
        [HttpGet("GetReplicateProducts")]
        [ProducesResponseType(typeof(List<ProductResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReplicateProducts([FromQuery] ProductFilter inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<ProductResponseModel>> products = await _iProductService.GetActiveReplicateProducts(inputModel, securityViewModel).ConfigureAwait(false);
                return Ok(products);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _iProductService.GetProductById(id).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpGet("GetByProductId/{id}")]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByProductId(int id, [FromQuery] PagedInputModel inputModel)
        {
            try
            {
                    return Ok(await _iProductService.GetByProductId(id, inputModel).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Add new product
        /// </summary>
        /// <param name="productModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddProduct([FromForm] ProductRequestModel productModel)
        {
            try
            {

                if (ModelState.IsValid && productModel != null)
                {
                    // To Add Outlet Product as List<OutletRequestModel> is not being recieved in Model
                    // due to the type of request[multipart-form].
                    // If changed the type of request, need to create another API for Image.
                    #region Adding Outlet Product
                    var outletProducts = Request.Form["OutletProduct"];
                    if (!string.IsNullOrEmpty(outletProducts))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OutletProductRequestModel>>(outletProducts.ToString());

                        productModel.OutletProduct = data;
                    }
                    #endregion

                    #region Add Supplier Product

                    var supplierProducts = Request.Form["SupplierProduct"];
                    if (!string.IsNullOrEmpty(supplierProducts))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierProductRequestModel>>(supplierProducts.ToString());

                        productModel.SupplierProduct = data;
                    }
                    #endregion

                    var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;

                    var result = await _iProductService.InsertProduct(productModel, securityViewModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    //productModel.Id = result;

                    if (productModel.Image != null && productModel.Image.Length > 0)
                    {
                        //not throwing exception if image upload fails
                        try
                        {
                            string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;
                            //Saved to "Product/Id" folder
                            imagePath = await _iImageUploader.UploadImage(productModel.Image, ("Product").ToString(CultureInfo.CurrentCulture), result.Id.ToString(CultureInfo.CurrentCulture)).ConfigureAwait(false);
                            //to save path update record
                            result = (await _iProductService.Update(productModel, result.Id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture), imagePath).ConfigureAwait(false));

                            result.ImageUploadStatusCode = "Ok";
                            result.ImagePath = imagePath;
                        }
                        catch (Exception ex)
                        {
                            result.ImageUploadStatusCode = ex.Message;
                        }
                        finally
                        {
                            productModel.Image = null;
                        }
                    }
                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (Newtonsoft.Json.JsonSerializationException jse)
            {

                return BadRequest(APIReponseBuilder.HandleResponse(jse.Message));

            }
            catch (AlreadyExistsException aee)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(aee.Message));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                if (ex is Newtonsoft.Json.JsonSerializationException)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.OutletProdformat));
                }
                if (ex is Newtonsoft.Json.JsonException)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.OutletProdformat));
                }
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update a Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productViewModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromForm] ProductRequestModel productViewModel)
        {
            try
            {
                if (ModelState.IsValid && productViewModel != null)
                {
                    // To Add Outlet Product as List<OutletRequestModel> is not being recieved in Model
                    // due to the type of request[multipart-form].
                    // If changed the type of request, need to create another API for Image.
                    #region Adding Outlet Product
                    var outletProducts = Request.Form["OutletProduct"];

                    if (!string.IsNullOrEmpty(outletProducts))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OutletProductRequestModel>>(outletProducts.ToString());

                        productViewModel.OutletProduct = data;
                    }

                    #endregion

                    #region Add Supplier Product
                    var supplierProducts = Request.Form["SupplierProduct"];

                    if (!string.IsNullOrEmpty(supplierProducts))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierProductRequestModel>>(supplierProducts.ToString());

                        productViewModel.SupplierProduct = data;
                    }

                    #endregion

                    var result = new ProductResponseModel();
                    string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;

                    if (productViewModel.Image != null && productViewModel.Image.Length > 0)
                    {
                        //not throwing exception if image upload fails
                        try
                        {
                            //Saved to "Product/Id" folder
                            imagePath = await _iImageUploader.UploadImage(productViewModel.Image, ("Product").ToString(CultureInfo.CurrentCulture), id.ToString(CultureInfo.CurrentCulture)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            result.ImageUploadStatusCode = ex.Message;
                        }
                        finally
                        {
                            productViewModel.Image = null;
                        }
                    }

                    var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                    result = await _iProductService.UpdateProduct(productViewModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture), securityViewModel).ConfigureAwait(false);

                    if (result != null)
                    {
                        result.ImageUploadStatusCode = "Ok";
                        result.ImagePath = imagePath;
                    }

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
                    //return Ok(productViewModel);
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (BadRequestException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                if (ex is Newtonsoft.Json.JsonSerializationException)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.OutletProdformat));
                }
                if (ex is Newtonsoft.Json.JsonException)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.OutletProdformat));
                }
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            try
            {
                if (id != 0)
                {
                    var result = await _iProductService.Delete(id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    return NoContent();
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException aee)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(aee.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }


        /// <summary>
        /// Get new number to create a product.
        /// </summary>
        /// <returns></returns>
        [HttpGet("number")]
        [ProducesResponseType(typeof(ProductNumberModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNumber(string Extend)
        {
            try
            {
                return Ok(await _iProductService.GetNewProductNumber(Extend).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get all Products Without APN.
        /// </summary>
        /// <returns>List of ProductViewModel</returns>
        [HttpGet("WithoutApn")]
        [ProducesResponseType(typeof(List<ProductResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProductsWithoutAPNAsync([FromQuery] PagedInputModel filter = null)
        {
            try
            {
                PagedOutputModel<List<ProductResponseModel>> products = await _iProductService.GetAllProductsWithoutAPN(filter).ConfigureAwait(false);
                return Ok(products);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }
        /// <summary>
        /// Get all active Products without APN.
        /// </summary>
        /// <returns>List of ProductViewModel</returns>
        [HttpGet("ProductsWithoutApn")]
        [ProducesResponseType(typeof(List<ProductResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductsWithoutAPN([FromQuery] ProductFilter inputModel)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                PagedOutputModel<List<ProductResponseModel>> products = await _iProductService.GetProductsWithoutAPN(inputModel, securityViewModel).ConfigureAwait(false);
                return Ok(products);
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get Product Purchase History
        /// </summary>
        /// <param name="inputFilter"></param>
        /// <returns></returns>
        [HttpGet("ProductDetail")]
        [ProducesResponseType(typeof(ProductTabsHistoryResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductTabs([FromQuery] ProductHistoryFilter inputFilter)
        {
            try
            {
                var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
                return Ok(await _iProductService.GetProductTabsHistory(securityViewModel, inputFilter).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Get new number to create a product.
        /// </summary>
        /// <returns></returns>
        [HttpGet("productnumber")]
        [ProducesResponseType(typeof(ProductNumberModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductNumber(string Extend, [FromQuery] PagedInputModel inputModel)
        {
            try
            {
                return Ok(await _iProductService.GetNewProductNo(inputModel, Extend).ConfigureAwait(false));
            }
            catch (NotFoundException nf)
            {
                return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
            }
            catch (NullReferenceException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Add new product
        /// </summary>
        /// <param name="productModel"></param>
        /// <returns></returns>
        [HttpPost("InsertProduct")]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertProduct([FromForm] ProductRequestModel productModel)
        {
            try
            {

                if (ModelState.IsValid && productModel != null)
                {
                    // To Add Outlet Product as List<OutletRequestModel> is not being recieved in Model
                    // due to the type of request[multipart-form].
                    // If changed the type of request, need to create another API for Image.
                    #region Adding Outlet Product
                    var outletProducts = Request.Form["OutletProduct"];
                    if (!string.IsNullOrEmpty(outletProducts))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OutletProductRequestModel>>(outletProducts.ToString());

                        productModel.OutletProduct = data;
                    }
                    #endregion

                    #region Add Supplier Product

                    var supplierProducts = Request.Form["SupplierProduct"];
                    if (!string.IsNullOrEmpty(supplierProducts))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierProductRequestModel>>(supplierProducts.ToString());

                        productModel.SupplierProduct = data;
                    }
                    #endregion

                    var result = await _iProductService.Insert(productModel, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);
                    //productModel.Id = result;

                    if (productModel.Image != null && productModel.Image.Length > 0)
                    {
                        //not throwing exception if image upload fails
                        try
                        {
                            string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;
                            //Saved to "Product/Id" folder
                            imagePath = await _iImageUploader.UploadImage(productModel.Image, ("Product").ToString(CultureInfo.CurrentCulture), result.Id.ToString(CultureInfo.CurrentCulture)).ConfigureAwait(false);
                            //to save path update record
                            result = (await _iProductService.Update(productModel, result.Id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture), imagePath).ConfigureAwait(false));

                            result.ImageUploadStatusCode = "Ok";
                            result.ImagePath = imagePath;
                        }
                        catch (Exception ex)
                        {
                            result.ImageUploadStatusCode = ex.Message;
                        }
                        finally
                        {
                            productModel.Image = null;
                        }
                    }
                    return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
                }
                return BadRequest(ModelState);
            }
            catch (Newtonsoft.Json.JsonSerializationException jse)
            {

                return BadRequest(APIReponseBuilder.HandleResponse(jse.Message));

            }
            catch (AlreadyExistsException aee)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(aee.Message));
            }
            catch (BadRequestException br)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(br.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                if (ex is Newtonsoft.Json.JsonSerializationException)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.OutletProdformat));
                }
                if (ex is Newtonsoft.Json.JsonException)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.OutletProdformat));
                }
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }
        }

        /// <summary>
        /// Update a Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productViewModel"></param>
        /// <returns></returns>
        [HttpPut("UpdateProduct/{id}")]
        [ProducesResponseType(typeof(ProductResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductRequestModel productViewModel)
        {
            try
            {
                if (ModelState.IsValid && productViewModel != null)
                {
                    // To Add Outlet Product as List<OutletRequestModel> is not being recieved in Model
                    // due to the type of request[multipart-form].
                    // If changed the type of request, need to create another API for Image.
                    #region Adding Outlet Product
                    var outletProducts = Request.Form["OutletProduct"];

                    if (!string.IsNullOrEmpty(outletProducts))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OutletProductRequestModel>>(outletProducts.ToString());

                        productViewModel.OutletProduct = data;
                    }

                    #endregion


                    #region Add Supplier Product
                    var supplierProducts = Request.Form["SupplierProduct"];

                    if (!string.IsNullOrEmpty(supplierProducts))
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierProductRequestModel>>(supplierProducts.ToString());

                        productViewModel.SupplierProduct = data;
                    }

                    #endregion
                    var result = new ProductResponseModel();
                    string imagePath = string.Empty; string imageUploadStatusCode = string.Empty;

                    if (productViewModel.Image != null && productViewModel.Image.Length > 0)
                    {
                        //not throwing exception if image upload fails
                        try
                        {
                            //Saved to "Product/Id" folder
                            imagePath = await _iImageUploader.UploadImage(productViewModel.Image, ("Product").ToString(CultureInfo.CurrentCulture), id.ToString(CultureInfo.CurrentCulture)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            result.ImageUploadStatusCode = ex.Message;
                        }
                        finally
                        {
                            productViewModel.Image = null;
                        }
                    }
                    result = await _iProductService.Update(productViewModel, id, Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value, CultureInfo.CurrentCulture)).ConfigureAwait(false);

                    if (result != null)
                    {
                        result.ImageUploadStatusCode = "Ok";
                        result.ImagePath = imagePath;
                    }

                    return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
                    //return Ok(productViewModel);
                }
                return BadRequest(ModelState);
            }
            catch (AlreadyExistsException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (BadRequestException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (NullReferenceCustomException nr)
            {
                return BadRequest(APIReponseBuilder.HandleResponse(nr.Message));
            }
            catch (Exception ex)
            {
                if (ex is Newtonsoft.Json.JsonSerializationException)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.OutletProdformat));
                }
                if (ex is Newtonsoft.Json.JsonException)
                {
                    return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.OutletProdformat));
                }
                _iLogger.LogError("ProductController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
                return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
            }

        }
    }
}
