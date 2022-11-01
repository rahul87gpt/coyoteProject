using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;

namespace Coyote.Console.App.Services.Services
{
    public class StockOnHandService : IStockOnHandService
    {
        private IUnitOfWork _unitOfWork = null;
        private IAutoMappingServices _iAutoMap;
        public StockOnHandService(IUnitOfWork unitOfWork, IAutoMappingServices autoMappingServices)
        {
            _unitOfWork = unitOfWork;
            _iAutoMap = autoMappingServices;
        }
        public async Task<PagedOutputModel<List<StockProductResponseViewModel>>> GetStockOnHand(StockProductsRequestModel inputModel)
        {
            try
            {
                if (inputModel == null)
                {
                    inputModel = new StockProductsRequestModel();
                }
                if (inputModel.ProductNumberTo <= 0)
                {
                    throw new NotFoundException(ErrorMessages.ProductRangeInvalid.ToString(CultureInfo.CurrentCulture));
                }
                var opRepo = _unitOfWork?.GetRepository<OutletProduct>();
                var prodRepo = _unitOfWork?.GetRepository<Product>();
                var taxRepo = _unitOfWork?.GetRepository<Tax>();
                var deptRepo = _unitOfWork?.GetRepository<Department>();
                var osRepo = _unitOfWork?.GetRepository<ZoneOutlet>();
                //int count = 0;

                var prodAll = prodRepo.GetAll();
                var outProdAll = opRepo.GetAll();
                var taxAll = taxRepo.GetAll();
                var deptAll = deptRepo.GetAll();
                var zoneOuts = osRepo.GetAll();

                #region Selected Suppliers
                List<int> suppIds = null;
                if (inputModel?.Suppliers?.Count > 0)
                {
                    suppIds = inputModel.Suppliers;
                }
                #endregion
                #region Selected Departments
                List<int> deptIds = null;
                if (inputModel?.Departments?.Count > 0)
                {
                    deptIds = inputModel.Departments;
                }
                #endregion
                #region Selected Outlets
                List<int> outIds = null;
                if (inputModel?.Outlets?.Count > 0)
                {
                    outIds = inputModel.Outlets;
                }
                #endregion
                #region Selected Commodities
                List<int> commIds = null;
                if (inputModel?.Commodities?.Count > 0)
                {
                    commIds = inputModel.Commodities;
                }
                #endregion
                #region Selected Categories
                List<int> catIds = null;
                if (inputModel?.Categories?.Count > 0)
                {
                    catIds = inputModel.Categories;
                }
                #endregion
                #region Selected Groups
                List<int> gpIds = null;
                if (inputModel?.Groups?.Count > 0)
                {
                    gpIds = inputModel.Groups;
                }
                #endregion
                #region Selected Manufacturer
                List<int> mfrIds = null;
                if (inputModel?.Manufacturers?.Count > 0)
                {
                    mfrIds = inputModel.Manufacturers;
                }
                #endregion 
                #region Selected Zones
                List<int> zoneIds = null;
                List<int> storeIds = null;
                if (inputModel?.Zones?.Count > 0)
                {
                    zoneIds = inputModel.Zones;
                    storeIds = await zoneOuts.Where(x => zoneIds.Contains(x.ZoneId)).Select(x => x.StoreId).ToListAsyncSafe().ConfigureAwait(false);
                }
                #endregion
                #region Selected Till
                List<int> tillStoreIds = null;
                if (inputModel.TillId > 0)
                {
                    tillStoreIds =await zoneOuts.Where(x => x.Id == inputModel.TillId).Select(x => x.StoreId).ToListAsyncSafe().ConfigureAwait(false);
                }
                #endregion 
                #region negative on hand as zero
                int negativeOnHand = 1;
                if (inputModel.SetNegativeOnHandAsZero)
                {
                    negativeOnHand = 0;
                }
                #endregion

                var Query = (from prod in prodAll
                             join op in outProdAll on prod.Id equals op.ProductId
                             join t in taxAll on prod.TaxId equals t.Id into tt
                             from tax in tt.DefaultIfEmpty()
                             join d in deptAll on prod.DepartmentId equals d.Id into dp
                             from dept in dp.DefaultIfEmpty()
                             
                             where !prod.IsDeleted && !op.IsDeleted && op.Status && op.QtyOnHand != 0 && !tax.IsDeleted && !dept.IsDeleted
                             && (suppIds == null || suppIds.Contains(prod.SupplierId)) && (deptIds == null || deptIds.Contains(prod.DepartmentId)) && (outIds == null || outIds.Contains(op.StoreId))
                             && (inputModel.ProductNumberFrom == null || prod.Number >= inputModel.ProductNumberFrom) && (inputModel.ProductNumberTo == null || (prod.Number) <= inputModel.ProductNumberTo)
                             && (commIds == null || commIds.Contains(prod.CommodityId)) && (catIds == null || catIds.Contains(prod.CategoryId)) && (gpIds == null || gpIds.Contains(prod.GroupId))
                             && (mfrIds == null || mfrIds.Contains(prod.ManufacturerId)) && (storeIds == null || storeIds.Contains(op.StoreId)) && (tillStoreIds == null || tillStoreIds.Contains(op.StoreId)) 
                             group new { op, prod, tax } by new
                             {
                                 DeptCode = dept.Code,
                                 Replicate = prod.Replicate,
                                 ProductDesc = prod.Desc,
                                 ProductNumber = prod.Number,
                                 CartonQty = prod.CartonQty,
                                 TaxCode = tax.Code,
                                 DeptDesc = dept.Desc,
                                 OutletId = op.StoreId,
                                 DeptId = dept.Id
                             } into data
                             select data
                          );

                var list = await Query
                    .Select(g => new StockProductResponseViewModel
                    {
                        SumOHandCost = g.Sum(data => (data.op.CartonCost * (negativeOnHand * data.op.QtyOnHand)) / (data.prod.CartonQty)),
                        SumOnHand = g.Sum(data => (negativeOnHand * data.op.QtyOnHand)),
                        SumOnHandCTNS = g.Sum(data => (negativeOnHand * data.op.QtyOnHand) / data.prod.CartonQty),
                        ProductNumber = g.Key.ProductNumber,
                        ProdDesc = g.Key.ProductDesc,
                        DepartmentCode = g.Key.DeptCode,
                        CartonQty = g.Key.CartonQty,
                        TaxCode = g.Key.TaxCode,
                        Replicate = g.Key.Replicate,
                        DepartmentDesc = g.Key.DeptDesc,
                        OutletId = g.Key.OutletId,
                        SumOnHandExCost = (float)(g.Key.TaxCode == "GST" ? g.Sum(data => ((data.op.CartonCost * (negativeOnHand * data.op.QtyOnHand)) / data.prod.CartonQty) / (data.tax.Factor)) : g.Sum(data => ((data.op.CartonCost * (negativeOnHand * data.op.QtyOnHand)) / data.prod.CartonQty)))
                    })
                    .OrderBy(x => x.DepartmentCode).ThenBy(x => x.Replicate).ThenBy(x => x.ProdDesc).ThenBy(x => x.ProductNumber).ThenBy(x => x.CartonQty).ThenBy(x => x.TaxCode).ThenBy(x => x.DepartmentDesc)
                    .ThenBy(x => x.OutletId)
                    .ToListAsyncSafe().ConfigureAwait(false);

                //if summary then group by department
                if (inputModel.Summary)
                {
                    list = list.GroupBy(x => x.DepartmentDesc).Select(
                         g => new StockProductResponseViewModel
                         {
                             DepartmentDesc = g.Key,
                             SumOnHandExCost = g.Sum(data => data.SumOnHandExCost)
                         }).OrderBy(x => x.DepartmentDesc).ToList();
                }

                return new PagedOutputModel<List<StockProductResponseViewModel>>(list, list.Count);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                } 
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
