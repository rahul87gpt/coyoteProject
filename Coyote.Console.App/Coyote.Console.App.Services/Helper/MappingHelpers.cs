using AutoMapper;
using Coyote.Console.App.Models;
using Coyote.Console.Common;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using FastMember;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Coyote.Console.App.Services.Helper
{
    public static class MappingHelpers
    {
        static IMapper iMapper;

        /// <summary>
        /// Mapper for Till
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static RecipeSPRequestModel CreateMap(RecipeDetailRequestModel Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RecipeDetailRequestModel, RecipeSPRequestModel>();
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<RecipeDetailRequestModel, RecipeSPRequestModel>(Mapper);
            AfterMap.Description = "";
            AfterMap.IsParents = 0;
            return AfterMap;
        }

        /// <summary>
        /// Mapper for Till
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static TillResponseModel CreateMap(Till Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Till, TillResponseModel>()
                .ForMember(dest => dest.OutletName, mo => mo.MapFrom(src => src.Store.Desc))
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.TypeCode, mo => mo.MapFrom(src => src.Type.Code))
                .ForMember(dest => dest.TypeName, mo => mo.MapFrom(src => src.Type.Name))
                .ForMember(dest => dest.KeypadCode, mo => mo.MapFrom(src => src.Keypad.Code))
                .ForMember(dest => dest.KeypadName, mo => mo.MapFrom(src => src.Keypad.Desc));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<Till, TillResponseModel>(Mapper);
            return AfterMap;
        }

        /// <summary>
        /// Mapper for Store
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static StoreResponseModel CreateMap(Store Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Store, StoreResponseModel>()
                .ForMember(dest => dest.GroupCode, mo => mo.MapFrom(src => src.StoreGroups.Code))
                .ForMember(dest => dest.GroupName, mo => mo.MapFrom(src => src.StoreGroups.Name))
                .ForMember(dest => dest.GroupIsDeleted, mo => mo.MapFrom(src => src.StoreGroups.IsDeleted))
                .ForMember(dest => dest.LabelTypePromoIsDeleted, mo => mo.MapFrom(src => src.LabelTypePromo.IsDeleted))
                .ForMember(dest => dest.LabelTypePromoCode, mo => mo.MapFrom(src => src.LabelTypePromo.Code))
                .ForMember(dest => dest.LabelTypePromoDesc, mo => mo.MapFrom(src => src.LabelTypePromo.Desc))
                .ForMember(dest => dest.LabelTypeShelfIsDeleted, mo => mo.MapFrom(src => src.LabelTypeShelf.IsDeleted))
                .ForMember(dest => dest.LabelTypeShelfCode, mo => mo.MapFrom(src => src.LabelTypeShelf.Code))
                .ForMember(dest => dest.LabelTypeShelfDesc, mo => mo.MapFrom(src => src.LabelTypeShelf.Desc))
                .ForMember(dest => dest.LabelTypeShortIsDeleted, mo => mo.MapFrom(src => src.LabelTypeShort.IsDeleted))
                .ForMember(dest => dest.LabelTypeShortCode, mo => mo.MapFrom(src => src.LabelTypeShort.Code))
                .ForMember(dest => dest.LabelTypeShortDesc, mo => mo.MapFrom(src => src.LabelTypeShort.Desc))
                .ForMember(dest => dest.WarehouseIsDeleted, mo => mo.MapFrom(src => src.Warehouse.IsDeleted))
                .ForMember(dest => dest.WarehouseCode, mo => mo.MapFrom(src => src.Warehouse.Code))
                .ForMember(dest => dest.WarehouseDesc, mo => mo.MapFrom(src => src.Warehouse.Desc))
                .ForMember(dest => dest.PriceZoneIsdeleted, mo => mo.MapFrom(src => src.PriceZone.IsActive))
                .ForMember(dest => dest.PriceZoneCode, mo => mo.MapFrom(src => src.PriceZone.Code))
                .ForMember(dest => dest.PriceZoneDesc, mo => mo.MapFrom(src => src.PriceZone.Description))
                .ForMember(dest => dest.CostZoneIsDeleted, mo => mo.MapFrom(src => src.CostZone.IsActive))
                .ForMember(dest => dest.CostZoneCode, mo => mo.MapFrom(src => src.CostZone.Code))
                .ForMember(dest => dest.CostZoneDesc, mo => mo.MapFrom(src => src.CostZone.Description))
                .ForMember(dest => dest.Zones, mo => mo.MapFrom(src => src.ZoneOutlets.Select(x => x.ZoneId).ToList()));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<Store, StoreResponseModel>(Mapper);

            AfterMap.StoreDetail = $"{Mapper?.Desc}   {Mapper.Code}";

            if (Mapper?.OutletTradingHours != null)
            {
                var tradingHours = Mapper.OutletTradingHours.Where(x => !x.IsDeleted).FirstOrDefault();
                if (tradingHours != null)
                {
                    AfterMap.StoreTradingHours = Mapping<OutletTradingHours, StoreTradingHoursRequest>(tradingHours);
                }
            }

            return AfterMap;
        }



        /// <summary>
        /// Mapper for Store
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static Store CreateRequestMap(StoreRequestModel Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StoreRequestModel, Store>()
                .ForMember(dest => dest.AppOrders, mo => mo.MapFrom(src => src.AppStoreDetails.AppOrders))
                .ForMember(dest => dest.AddressOnApp, mo => mo.MapFrom(src => src.AppStoreDetails.AddressOnApp))
                .ForMember(dest => dest.DisplayOnApp, mo => mo.MapFrom(src => src.AppStoreDetails.DisplayOnApp))
                .ForMember(dest => dest.Email, mo => mo.MapFrom(src => src.AppStoreDetails.Email))
                .ForMember(dest => dest.Latitude, mo => mo.MapFrom(src => src.AppStoreDetails.Latitude))
                .ForMember(dest => dest.Longitude, mo => mo.MapFrom(src => src.AppStoreDetails.Longitude))
                .ForMember(dest => dest.NameOnApp, mo => mo.MapFrom(src => src.AppStoreDetails.NameOnApp))
                .ForMember(dest => dest.OpenHours, mo => mo.MapFrom(src => src.AppStoreDetails.OpenHours));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<StoreRequestModel, Store>(Mapper);

            return AfterMap;
        }

        /// <summary>
        /// Mapper for Cashier
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static CashierResponseModel CreateMap(Cashier Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Cashier, CashierResponseModel>()
                .ForMember(dest => dest.AccessLevel, mo => mo.MapFrom(src => src.AccessLevel.Name))
                .ForMember(dest => dest.OutletName, mo => mo.MapFrom(src => !src.Outlet.IsDeleted ? src.Outlet.Code : string.Empty))
                .ForMember(dest => dest.OutletDesc, mo => mo.MapFrom(src => !src.Outlet.IsDeleted ? src.Outlet.Desc : string.Empty))
                .ForMember(dest => dest.IsStoreGroupDeleted, mo => mo.MapFrom(src => !src.StoreGroup.IsDeleted))
                .ForMember(dest => dest.StoreGroup, mo => mo.MapFrom(src => !src.StoreGroup.IsDeleted ? src.StoreGroup.Code : string.Empty))
                .ForMember(dest => dest.StoreGroupDesc, mo => mo.MapFrom(src => !src.StoreGroup.IsDeleted ? src.StoreGroup.Name : string.Empty))
                .ForMember(dest => dest.ZoneCode, mo => mo.MapFrom(src => !src.StoreGroup.IsDeleted ? src.Zone.Code : string.Empty))
                .ForMember(dest => dest.ZoneDesc, mo => mo.MapFrom(src => !src.StoreGroup.IsDeleted ? src.Zone.Name : string.Empty))
                .ForMember(dest => dest.TypeName, mo => mo.MapFrom(src => !src.IsDeleted ? src.Type.Name : string.Empty));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<Cashier, CashierResponseModel>(Mapper);

            AfterMap.CashierName = $"{Mapper?.Surname}  {Mapper?.FirstName}  {Mapper?.Number}";

            return AfterMap;
        }

        /// <summary>
        /// Mapper for APN
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static APNResponseViewModel CreateMap(APN Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<APN, APNResponseViewModel>()
                .ForMember(dest => dest.ProductNumber, mo => mo.MapFrom(src => src.Product.Number))
                .ForMember(dest => dest.ProductDesc, mo => mo.MapFrom(src => src.Product.Desc));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<APN, APNResponseViewModel>(Mapper);
            return AfterMap;
        }

        /// <summary>
        /// Mapper for Keypad
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static KeypadResponseModel CreateMap(Keypad Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Keypad, KeypadResponseModel>()
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.OutletName, mo => mo.MapFrom(src => src.Store.Desc));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<Keypad, KeypadResponseModel>(Mapper);
            //if(Mapper?.KeyPadButtonJSONData !=null)
            //{
            //  AfterMap.KeyPadButtonJSONData = Newtonsoft.Json.JsonConvert.DeserializeObject(Mapper?.KeyPadButtonJSONData);
            //}
            return AfterMap;
        }

        /// <summary>
        /// Mapper for Product
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static ProductResponseModel CreateMap(Product Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductResponseModel>()
                .ForMember(dest => dest.Commodity, mo => mo.MapFrom(src => src.Commodity.Desc))
                .ForMember(dest => dest.CommodityIsDeleted, mo => mo.MapFrom(src => src.Commodity.IsDeleted))
                .ForMember(dest => dest.CommodityCode, mo => mo.MapFrom(src => src.Commodity.Code))
                .ForMember(dest => dest.Department, mo => mo.MapFrom(src => src.Department.Desc))
                .ForMember(dest => dest.DepartmentIsDeleted, mo => mo.MapFrom(src => src.Department.IsDeleted))
                .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Department.Code))
                .ForMember(dest => dest.NationalRange, mo => mo.MapFrom(src => src.NationalRangeMasterListItem.Name))
                .ForMember(dest => dest.NationalRangeIsDeleted, mo => mo.MapFrom(src => src.NationalRangeMasterListItem.IsDeleted))
                .ForMember(dest => dest.NationalRangeCode, mo => mo.MapFrom(src => src.NationalRangeMasterListItem.Code))
                .ForMember(dest => dest.Category, mo => mo.MapFrom(src => src.CategoryMasterListItem.Name))
                .ForMember(dest => dest.CategoryIsDeleted, mo => mo.MapFrom(src => src.CategoryMasterListItem.IsDeleted))
                .ForMember(dest => dest.CategoryCode, mo => mo.MapFrom(src => src.CategoryMasterListItem.Code))
                .ForMember(dest => dest.Group, mo => mo.MapFrom(src => src.GroupMasterListItem.Name))
                .ForMember(dest => dest.GroupIsDeleted, mo => mo.MapFrom(src => src.GroupMasterListItem.IsDeleted))
                .ForMember(dest => dest.GroupCode, mo => mo.MapFrom(src => src.GroupMasterListItem.Code))
                .ForMember(dest => dest.Supplier, mo => mo.MapFrom(src => src.Supplier.Desc))
                .ForMember(dest => dest.SupplierIsDeleted, mo => mo.MapFrom(src => src.Supplier.IsDeleted))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                .ForMember(dest => dest.Tax, mo => mo.MapFrom(src => src.Tax.Desc))
                .ForMember(dest => dest.TaxIsDeleted, mo => mo.MapFrom(src => src.Tax.IsDeleted))
                .ForMember(dest => dest.TaxCode, mo => mo.MapFrom(src => src.Tax.Code))
                .ForMember(dest => dest.Type, mo => mo.MapFrom(src => src.TypeMasterListItem.Name))
                .ForMember(dest => dest.TypeIsDeleted, mo => mo.MapFrom(src => src.TypeMasterListItem.IsDeleted))
                .ForMember(dest => dest.TypeCode, mo => mo.MapFrom(src => src.TypeMasterListItem.Code))
                .ForMember(dest => dest.Manufacturer, mo => mo.MapFrom(src => src.ManufacturerMasterListItem.Name))
                .ForMember(dest => dest.ManufacturerIsDeleted, mo => mo.MapFrom(src => src.ManufacturerMasterListItem.IsDeleted))
                .ForMember(dest => dest.ManufacturerCode, mo => mo.MapFrom(src => src.ManufacturerMasterListItem.Code))
                .ForMember(dest => dest.UnitMeasure, mo => mo.MapFrom(src => src.UnitMeasureMasterListItem.Name))
                .ForMember(dest => dest.UnitMeasureIsDeleted, mo => mo.MapFrom(src => src.UnitMeasureMasterListItem.IsDeleted))
                .ForMember(dest => dest.UnitMeasureCode, mo => mo.MapFrom(src => src.UnitMeasureMasterListItem.Code));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<Product, ProductResponseModel>(Mapper);

            AfterMap.ActiveProductCount = Mapper?.OutletProductProduct.Where(x => x.Status == true && !x.IsDeleted).Count();

            if (Mapper?.SupplierProduct != null)
            {
                var supplierProd = (Mapper.SupplierProduct).Where(x => !x.IsDeleted).Select(CreateMap);
                if (supplierProd != null)
                {
                    AfterMap.SupplierProducts.AddRange(supplierProd);
                }

                AfterMap.SupplierProductCount = Mapper?.SupplierProduct.Where(x => x.Status == true && !x.IsDeleted).Count();
            }

            AfterMap.HostCode = CommonMessages.WarehouseCode1;
            AfterMap.HostCode2 = CommonMessages.WarehouseCode2;
            AfterMap.HostCode3 = CommonMessages.WarehouseCode3;

            if (!string.IsNullOrEmpty(Mapper?.AccessOutletIds))
            {
                var accessOutlets = Mapper.AccessOutletIds.Split(',').ToList();
                foreach (var outletId in accessOutlets)
                {
                    AfterMap.AccessOutlets.Add(Convert.ToInt32(outletId));
                }
            }
            return AfterMap;
        }

        /// <summary>
        /// Mapper for Supplier Product
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static SupplierProductResponseViewModel CreateMap(SupplierProduct Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SupplierProduct, SupplierProductResponseViewModel>()
                .ForMember(dest => dest.SupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code));
                // .ForMember(dest => dest.LastInvoiceCost, mo => mo.MapFrom(src => src.OrderDetailSupplierItem.ToList().OrderByDescending(x => x.CreatedAt).Select(x => x.FinalLineTotal)));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<SupplierProduct, SupplierProductResponseViewModel>(Mapper);



            return afterMap;
        }

        /// <summary>
        /// Mapper for Order Details
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static OrderDetailViewModel CreateMap(OrderDetail Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
             {
                 cfg.CreateMap<OrderDetail, OrderDetailViewModel>()
                 .ForMember(dest => dest.OrderTypeName, mo => mo.MapFrom(src => src.OrderType.Name))
                 .ForMember(dest => dest.OrderTypeCode, mo => mo.MapFrom(src => src.OrderType.Code))
                 .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
                 .ForMember(dest => dest.Desc, mo => mo.MapFrom(src => src.Product.Desc))
                 .ForMember(dest => dest.TaxCode, mo => mo.MapFrom(src => src.Product.Tax.Code))
                 .ForMember(dest => dest.SupplierProductDesc, mo => mo.MapFrom(src => src.SupplierProduct.Desc))
                 .ForMember(dest => dest.SupplierProductItem, mo => mo.MapFrom(src => src.SupplierProduct.SupplierItem))
                 .ForMember(dest => dest.SupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc))
                 .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code));
             });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<OrderDetail, OrderDetailViewModel>(Mapper);
            return afterMap;
        }

        public static PurchaseHistoryModel CreatePurchaseHistory(OrderDetail Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderDetail, PurchaseHistoryModel>()
                .ForMember(dest => dest.Outlet, mo => mo.MapFrom(src => src.OrderHeader.OutletId))
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.OrderHeader.Store.Code))
                .ForMember(dest => dest.OutletName, mo => mo.MapFrom(src => src.OrderHeader.Store.Desc))
                .ForMember(dest => dest.OrderNo, mo => mo.MapFrom(src => src.OrderHeader.OrderNo))
                .ForMember(dest => dest.OrderDate, mo => mo.MapFrom(src => src.OrderHeader.CreatedDate))
                .ForMember(dest => dest.DatePosted, mo => mo.MapFrom(src => src.PostedDate))
                .ForMember(dest => dest.DocType, mo => mo.MapFrom(src => src.OrderHeader.Type.Code))
                .ForMember(dest => dest.DocStatus, mo => mo.MapFrom(src => src.OrderHeader.Status.Code))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                .ForMember(dest => dest.Supplier, mo => mo.MapFrom(src => src.Supplier.Desc))
                .ForMember(dest => dest.InvoiceNo, mo => mo.MapFrom(src => src.OrderHeader.InvoiceNo))
                .ForMember(dest => dest.InvoiceDate, mo => mo.MapFrom(src => src.OrderHeader.InvoiceDate))
                //.ForMember(dest => dest.Cartons, mo => mo.MapFrom(src => src.Cartons))
                //.ForMember(dest => dest.Units, mo => mo.MapFrom(src => src.Units))
                //.ForMember(dest => dest.CartonCost, mo => mo.MapFrom(src => src.CartonCost))
                //.ForMember(dest => dest.LineTotal, mo => mo.MapFrom(src => src.LineTotal))
                //.ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.CartonQty))
                .ForMember(dest => dest.SupplierItem, mo => mo.MapFrom(src => src.SupplierProduct.SupplierItem))
                .ForMember(dest => dest.InvoiceTotal, mo => mo.MapFrom(src => src.OrderHeader.InvoiceTotal))
                .ForMember(dest => dest.DelDocNo, mo => mo.MapFrom(src => src.OrderHeader.DeliveryNo))
                .ForMember(dest => dest.DelDocDate, mo => mo.MapFrom(src => src.OrderHeader.DeliveryDate))
                .ForMember(dest => dest.Timestamp, mo => mo.MapFrom(src => src.OrderHeader.CreatedDate));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<OrderDetail, PurchaseHistoryModel>(Mapper);
            return afterMap;
        }
        public static ProductChildModel CreateChildProduct(Product Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductChildModel>()
                .ForMember(dest => dest.Type, mo => mo.MapFrom(src => src.TypeMasterListItem.Name))
                .ForMember(dest => dest.TypeCode, mo => mo.MapFrom(src => src.TypeMasterListItem.Code));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<Product, ProductChildModel>(Mapper);
            return afterMap;
        }

        public static ProductStockMovementModel CreateStockMovement(Transaction Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, ProductStockMovementModel>()
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.OutletDesc, mo => mo.MapFrom(src => src.Store.Desc))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                .ForMember(dest => dest.SupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<Transaction, ProductStockMovementModel>(Mapper);
            return afterMap;
        }

        public static ProductZonePricingModel CreateZonePricing(ZonePricing Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ZonePricing, ProductZonePricingModel>()
                .ForMember(dest => dest.PriceZoneCode, mo => mo.MapFrom(src => src.PriceZoneCostPrice.Code))
                .ForMember(dest => dest.PriceZoneName, mo => mo.MapFrom(src => src.PriceZoneCostPrice.Description));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<ZonePricing, ProductZonePricingModel>(Mapper);
            return afterMap;
        }

        public static TransactionHistoryModel CreateTransactionMap(Transaction Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, TransactionHistoryModel>()
                //.ForMember(dest => dest.Id, mo => mo.MapFrom(src => src.Id))
                //.ForMember(dest => dest.Date, mo => mo.MapFrom(src => src.Date))
                //.ForMember(dest => dest.Day, mo => mo.MapFrom(src => src.Day))
                //.ForMember(dest => dest.Type, mo => mo.MapFrom(src => src.Type))
                //  .ForMember(dest => dest.OutletId, mo => mo.MapFrom(src => src.OutletId))
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.OutletDesc, mo => mo.MapFrom(src => src.Store.Desc))
                .ForMember(dest => dest.TillId, mo => mo.MapFrom(src => src.TillId))
                .ForMember(dest => dest.TillCode, mo => mo.MapFrom(src => src.Till.Code))
                .ForMember(dest => dest.TillDesc, mo => mo.MapFrom(src => src.Till.Desc))
                //.ForMember(dest => dest.Qty, mo => mo.MapFrom(src => src.Qty))
                //.ForMember(dest => dest.Amt, mo => mo.MapFrom(src => src.Amt))
                .ForMember(dest => dest.AmtGst, mo => mo.MapFrom(src => src.ExGSTAmt))
                .ForMember(dest => dest.Cost, mo => mo.MapFrom(src => src.Cost))
                .ForMember(dest => dest.CostGst, mo => mo.MapFrom(src => src.ExGSTCost))
                //.ForMember(dest => dest.Discount, mo => mo.MapFrom(src => src.Discount))
                .ForMember(dest => dest.SubType, mo => mo.MapFrom(src => src.Tender))
                .ForMember(dest => dest.PromSellId, mo => mo.MapFrom(src => src.PromoSellId))
                .ForMember(dest => dest.PromSellCode, mo => mo.MapFrom(src => src.PromotionSell.Code))
                .ForMember(dest => dest.PromSellDesc, mo => mo.MapFrom(src => src.PromotionSell.Desc))
                .ForMember(dest => dest.PromBuyId, mo => mo.MapFrom(src => src.PromoBuyId))
                .ForMember(dest => dest.PromBuyCode, mo => mo.MapFrom(src => src.PromotionBuy.Code))
                .ForMember(dest => dest.PromBuyDesc, mo => mo.MapFrom(src => src.PromotionBuy.Desc))
                .ForMember(dest => dest.StockUnitMovement, mo => mo.MapFrom(src => src.StockMovement))
                //.ForMember(dest => dest.Parent, mo => mo.MapFrom(src => src.Parent))
                //.ForMember(dest => dest.ProductId, mo => mo.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductNumber, mo => mo.MapFrom(src => src.Product.Number))
                .ForMember(dest => dest.ProductDesc, mo => mo.MapFrom(src => src.Product.Desc))
                 //.ForMember(dest => dest.Member, mo => mo.MapFrom(src => src.Member))
                 .ForMember(dest => dest.NewUnitOnHand, mo => mo.MapFrom(src => src.NewOnHand))
                //  .ForMember(dest => dest.Reference, mo => mo.MapFrom(src => src.Reference))
                .ForMember(dest => dest.Manual, mo => mo.MapFrom(src => src.ManualInd))
                 // .ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.CartonQty))
                 .ForMember(dest => dest.SellUnit, mo => mo.MapFrom(src => src.UnitQty))
                // .ForMember(dest => dest.UserId, mo => mo.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, mo => mo.MapFrom(src => src.User.FirstName + ' ' + src.User.FirstName))
                .ForMember(dest => dest.DateTimeStamp, mo => mo.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.HistDeptId, mo => mo.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.HistDeptCode, mo => mo.MapFrom(src => src.Department.Code))
                .ForMember(dest => dest.HistDeptDesc, mo => mo.MapFrom(src => src.Department.Desc))
                .ForMember(dest => dest.HistSupplierId, mo => mo.MapFrom(src => src.SupplierId))
                .ForMember(dest => dest.HistSupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                .ForMember(dest => dest.HistSupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<Transaction, TransactionHistoryModel>(Mapper);
            return afterMap;
        }


        public static OrderHeaderResponseViewModel CreateMap(OrderHeader Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
             {
                 cfg.CreateMap<OrderHeader, OrderHeaderResponseViewModel>()
                 .ForMember(dest => dest.StoreDesc, mo => mo.MapFrom(src => src.Store.Desc))
                 .ForMember(dest => dest.StoreCode, mo => mo.MapFrom(src => src.Store.Code))
                  .ForMember(dest => dest.SupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc))
                 .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                  .ForMember(dest => dest.CreationTypeCode, mo => mo.MapFrom(src => src.CreationType.Code))
                 .ForMember(dest => dest.CreationTypeName, mo => mo.MapFrom(src => src.CreationType.Name))
                  .ForMember(dest => dest.TypeCode, mo => mo.MapFrom(src => src.Type.Code))
                 .ForMember(dest => dest.TypeName, mo => mo.MapFrom(src => src.Type.Name))
                  .ForMember(dest => dest.StatusCode, mo => mo.MapFrom(src => src.Status.Code))
                 .ForMember(dest => dest.StatusName, mo => mo.MapFrom(src => src.Status.Name));
             });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<OrderHeader, OrderHeaderResponseViewModel>(Mapper);
            return afterMap;
        }

        /// <summary>
        /// Mapper for print label type.
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        //public static PrintLabelTypeResponseModel CreateMap(PrintLabelType Mapper)
        //{
        //    MapperConfiguration config = new MapperConfiguration(cfg =>
        //    {
        //        cfg.CreateMap<PrintLabelType, PrintLabelTypeResponseModel>()
        //         //.ForMember(dest => dest.PrintBarCodeTypeCode, mo => mo.MapFrom(src => src.PrintBarCodeType.Code))
        //         //.ForMember(dest => dest.PrintBarCodeTypeName, mo => mo.MapFrom(src => src.PrintBarCodeType.Name));
        //    });
        //    iMapper = config.CreateMapper();
        //    var afterMap = iMapper.Map<PrintLabelType, PrintLabelTypeResponseModel>(Mapper);
        //    return afterMap;
        //}

        /// <summary>
        /// Commodity mapper.
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static CommodityResponseModel CreateMap(Commodity Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Commodity, CommodityResponseModel>()
                .ForMember(dest => dest.DepartmentDesc, mo => mo.MapFrom(src => src.Departments.Desc))
                .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Departments.Code));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<Commodity, CommodityResponseModel>(Mapper);
            return AfterMap;
        }

        public static PromotionResponseViewModel CreateMap(Promotion Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Promotion, PromotionResponseViewModel>()
                .ForMember(dest => dest.Frequency, mo => mo.MapFrom(src => src.PromotionFrequency.Code))
                .ForMember(dest => dest.FrequencyDesc, mo => mo.MapFrom(src => src.PromotionFrequency.Name))
                .ForMember(dest => dest.PromotionType, mo => mo.MapFrom(src => src.PromotionType.Code))
                .ForMember(dest => dest.Zone, mo => mo.MapFrom(src => src.PromotionZone.Name))
                ;
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<Promotion, PromotionResponseViewModel>(Mapper);

            AfterMap.PromotionDetail = $"{Mapper?.Desc}       {Mapper.Code}  {Mapper.Start} {Mapper.End} ";

            AfterMap.Source = Enum.GetName(typeof(PromotionSource), Mapper?.SourceId);

            //for prices from MemberOffer
            if (Mapper?.PromotionMemberOffer != null)
            {
                var member = Mapper?.PromotionMemberOffer.ToList();
                foreach (var prod in member)
                {
                    AfterMap.Price1 = prod.Price1;
                    AfterMap.Price2 = prod.Price2;
                    AfterMap.Price3 = prod.Price3;
                    AfterMap.Price4 = prod.Price4;

                    AfterMap.Action = prod.Action;
                }
            }

            if (Mapper?.PromotionSelling != null)
            {
                //for prices from Sellings
                var sellings = Mapper?.PromotionSelling.ToList();
                foreach (var prod in sellings)
                {
                    AfterMap.Price1 = prod.Price1;
                    AfterMap.Price2 = prod.Price2;
                    AfterMap.Price3 = prod.Price3;
                    AfterMap.Price4 = prod.Price4;

                    AfterMap.Action = prod.Action;
                }
            }

            if (Mapper?.PromotionBuying != null)
            {
                //for carton cost from Buying
                var buying = Mapper?.PromotionBuying.ToList();
                foreach (var prod in buying)
                {
                    AfterMap.CartonCost = prod.CartonCost;

                    AfterMap.Action = prod.Action;
                }
            }

            return AfterMap;
        }


        public static PromotionProductRequestModel CreateMap(PromotionMemberOffer Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PromotionMemberOffer, PromotionProductRequestModel>()
              .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
              .ForMember(dest => dest.Price, mo => mo.MapFrom(src => src.Price1))
              .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Product.Department.Code))
              .ForMember(dest => dest.DepartmentDesc, mo => mo.MapFrom(src => src.Product.Department.Desc))
              .ForMember(dest => dest.CartonCost, mo => mo.MapFrom(src => src.Product.CartonCost))
              .ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.Product.CartonQty));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<PromotionMemberOffer, PromotionProductRequestModel>(Mapper);
            AfterMap.PromotionType = "MemberOffer";
            return AfterMap;
        }
        public static PromotionProductRequestModel CreateMap(PromotionMixmatchProduct Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PromotionMixmatchProduct, PromotionProductRequestModel>()
                .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
                .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Product.Department.Code))
                .ForMember(dest => dest.DepartmentDesc, mo => mo.MapFrom(src => src.Product.Department.Desc))
                .ForMember(dest => dest.CartonCost, mo => mo.MapFrom(src => src.Product.CartonCost))
                .ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.Product.CartonQty))
                .ForMember(dest => dest.PromotionId, mo => mo.MapFrom(src => src.PromotionMixmatchId))
                .ForMember(dest => dest.PromoCode, mo => mo.MapFrom(src => src.PromotionMixmatch.Promotion.Code));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<PromotionMixmatchProduct, PromotionProductRequestModel>(Mapper);
            AfterMap.PromotionType = "MIXMATCH";
            return AfterMap;
        }

        public static PromotionProductRequestModel CreateMap(PromotionBuying Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PromotionBuying, PromotionProductRequestModel>()
              .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
                 .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Product.Department.Code))
                   .ForMember(dest => dest.DepartmentDesc, mo => mo.MapFrom(src => src.Product.Department.Desc))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<PromotionBuying, PromotionProductRequestModel>(Mapper);
            AfterMap.PromotionType = "BUYING";
            return AfterMap;
        }

        public static PromotionProductRequestModel CreateMap(PromotionOfferProduct Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PromotionOfferProduct, PromotionProductRequestModel>()
                  .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
                  .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Product.Department.Code))
                  .ForMember(dest => dest.DepartmentDesc, mo => mo.MapFrom(src => src.Product.Department.Desc))
               .ForMember(dest => dest.OfferGroup, mo => mo.MapFrom(src => src.OfferGroup))
                  .ForMember(dest => dest.CartonCost, mo => mo.MapFrom(src => src.Product.CartonCost))
                .ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.Product.CartonQty))
                .ForMember(dest => dest.PromotionId, mo => mo.MapFrom(src => src.PromotionOfferId))
                .ForMember(dest => dest.PromoCode, mo => mo.MapFrom(src => src.PromotionOffer.Promotion.Code));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<PromotionOfferProduct, PromotionProductRequestModel>(Mapper);
            AfterMap.PromotionType = "OFFER";
            return AfterMap;
        }

        public static PromotionProductRequestModel CreateMap(PromotionSelling Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PromotionSelling, PromotionProductRequestModel>()
                .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
                 .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Product.Department.Code))
                 .ForMember(dest => dest.DepartmentDesc, mo => mo.MapFrom(src => src.Product.Department.Desc))
                .ForMember(dest => dest.CartonCost, mo => mo.MapFrom(src => src.Product.CartonCost))
                .ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.Product.CartonQty));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<PromotionSelling, PromotionProductRequestModel>(Mapper);
            AfterMap.PromotionType = "OFFER";
            return AfterMap;
        }

        /// <summary>
        /// Stock Adjust Mapper
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static StockAdjustHeaderResponseViewModel CreateMap(StockAdjustHeader Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StockAdjustHeader, StockAdjustHeaderResponseViewModel>()
                  .ForMember(dest => dest.OutletDesc, mo => mo.MapFrom(src => src.Description))
                 .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<StockAdjustHeader, StockAdjustHeaderResponseViewModel>(Mapper);
            return afterMap;
        }
        public static StockAdjustDetailResponseModel CreateMap(StockAdjustDetail Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StockAdjustDetail, StockAdjustDetailResponseModel>()
                    .ForMember(dest => dest.ReasonName, mo => mo.MapFrom(src => src.Reason.Name))
                 .ForMember(dest => dest.ReasonCode, mo => mo.MapFrom(src => src.Reason.Code))
                  .ForMember(dest => dest.ProductDesc, mo => mo.MapFrom(src => src.Product.Desc))
                 .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
                 .ForMember(dest => dest.UnitOnHand, mo => mo.MapFrom(src => src.OutletProduct.QtyOnHand));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<StockAdjustDetail, StockAdjustDetailResponseModel>(Mapper);
            return afterMap;
        }


        /// <summary>
        /// Stock Take Mapper
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static StockTakeHeaderResponseViewModel CreateMap(StockTakeHeader Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StockTakeHeader, StockTakeHeaderResponseViewModel>()
                  .ForMember(dest => dest.OutletDesc, mo => mo.MapFrom(src => src.Store.Desc))
                 .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<StockTakeHeader, StockTakeHeaderResponseViewModel>(Mapper);
            return afterMap;
        }


        public static StockTakeDetailResponseModel CreateMap(StockTakeDetail Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StockTakeDetail, StockTakeDetailResponseModel>()
                  .ForMember(dest => dest.ProductId, mo => mo.MapFrom(src => src.OutletProduct.Product.Id))
                  .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.OutletProduct.Product.Number))
                  .ForMember(dest => dest.ProductDesc, mo => mo.MapFrom(src => src.OutletProduct.Product.Desc))
                 .ForMember(dest => dest.UnitCost, mo => mo.MapFrom(src => src.OutletProduct.CartonCost / src.OutletProduct.Product.CartonQty))
                 .ForMember(dest => dest.VarUnits, mo => mo.MapFrom(src => src.Quantity - src.OnHandUnits))
                 .ForMember(dest => dest.VarCost, mo => mo.MapFrom(src => src.VarQty * (src.OutletProduct.CartonCost / src.OutletProduct.Product.CartonQty)))
                 .ForMember(dest => dest.UnitCount, mo => mo.MapFrom(src => src.Quantity))
                 .ForMember(dest => dest.ItemCount, mo => mo.MapFrom(src => src.Quantity))
                 .ForMember(dest => dest.LineTotal, mo => mo.MapFrom(src => src.Quantity * (src.OutletProduct.CartonCost / src.OutletProduct.Product.CartonQty)));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<StockTakeDetail, StockTakeDetailResponseModel>(Mapper);
            return afterMap;
        }

        /// <summary>
        /// Competition Mapper
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static CompetitionResponseViewModel CreateMap(CompetitionDetail Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CompetitionDetail, CompetitionResponseViewModel>()
                  .ForMember(dest => dest.FrequencyCode, mo => mo.MapFrom(src => src.Promotion.PromotionFrequency.Code))
                 .ForMember(dest => dest.FrequencyDesc, mo => mo.MapFrom(src => src.Promotion.PromotionFrequency.Name))
                  .ForMember(dest => dest.SourceCode, mo => mo.MapFrom(src => src.Promotion.PromotionSource.Code))
                 .ForMember(dest => dest.SourceDesc, mo => mo.MapFrom(src => src.Promotion.PromotionSource.Name))
                  .ForMember(dest => dest.Start, mo => mo.MapFrom(src => src.Promotion.Start))
                 .ForMember(dest => dest.End, mo => mo.MapFrom(src => src.Promotion.End))
                 .ForMember(dest => dest.Availibility, mo => mo.MapFrom(src => src.Promotion.Availibility))
                  .ForMember(dest => dest.TriggerTypeCode, mo => mo.MapFrom(src => src.TriggerType.Code))
                 .ForMember(dest => dest.TriggerTypeDesc, mo => mo.MapFrom(src => src.TriggerType.Name))
                  .ForMember(dest => dest.RewardTypeCode, mo => mo.MapFrom(src => src.RewardType.Code))
                 .ForMember(dest => dest.RewardTypeDesc, mo => mo.MapFrom(src => src.RewardType.Name))
                  .ForMember(dest => dest.ZoneCode, mo => mo.MapFrom(src => src.CompetitionZone.Code))
                 .ForMember(dest => dest.ZoneDesc, mo => mo.MapFrom(src => src.CompetitionZone.Name))
                  .ForMember(dest => dest.CompetitionTypeCode, mo => mo.MapFrom(src => src.CompetitionType.Code))
                 .ForMember(dest => dest.CompetitionTypeDesc, mo => mo.MapFrom(src => src.CompetitionType.Name))
                  .ForMember(dest => dest.ResetCycleCode, mo => mo.MapFrom(src => src.CompetitionResetCycle.Code))
                 .ForMember(dest => dest.ResetCycleDesc, mo => mo.MapFrom(src => src.CompetitionResetCycle.Name));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<CompetitionDetail, CompetitionResponseViewModel>(Mapper);
            return afterMap;
        }
        public static CompTriggerResponseViewModel CreateMap(PromotionCompetition Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PromotionCompetition, CompTriggerResponseViewModel>()
                  .ForMember(dest => dest.CompPromoId, mo => mo.MapFrom(src => src.Id))
                  .ForMember(dest => dest.Id, mo => mo.MapFrom(src => src.Triggers.Id))
                  .ForMember(dest => dest.Share, mo => mo.MapFrom(src => src.Triggers.Share))
                  .ForMember(dest => dest.LoyaltyFactor, mo => mo.MapFrom(src => src.Triggers.LoyaltyFactor))
                  .ForMember(dest => dest.ProductNumber, mo => mo.MapFrom(src => src.Product.Number))
                  .ForMember(dest => dest.Desc, mo => mo.MapFrom(src => src.Product.Desc))
                  .ForMember(dest => dest.TriggerProductGroupID, mo => mo.MapFrom(src => src.Triggers.TriggerProductGroupID))
                  .ForMember(dest => dest.ProductGroupCode, mo => mo.MapFrom(src => src.Triggers.TriggerProductGroup.Code))
                 .ForMember(dest => dest.ProductGroupDesc, mo => mo.MapFrom(src => src.Triggers.TriggerProductGroup.Name));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<PromotionCompetition, CompTriggerResponseViewModel>(Mapper);
            return afterMap;
        }
        public static PromotionProductRequestModel CreateMap1(PromotionCompetition Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PromotionCompetition, PromotionProductRequestModel>().ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
                .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Product.Department.Code))
                .ForMember(dest => dest.DepartmentDesc, mo => mo.MapFrom(src => src.Product.Department.Desc));
                //.ForMember(dest => dest.OfferGroup, mo => mo.MapFrom(src => src.CompetitionDetail.));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<PromotionCompetition, PromotionProductRequestModel>(Mapper);
            afterMap.PromotionType = "COMPETITION";
            return afterMap;
        }
        
        public static CompRewardResponseViewModel CreateMapRewards(PromotionCompetition Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PromotionCompetition, CompRewardResponseViewModel>()
                  .ForMember(dest => dest.CompPromoId, mo => mo.MapFrom(src => src.Id))
                 .ForMember(dest => dest.ProductNumber, mo => mo.MapFrom(src => src.Product.Number))
                  .ForMember(dest => dest.Desc, mo => mo.MapFrom(src => src.Product.Desc))
                  .ForMember(dest => dest.Count, mo => mo.MapFrom(src => src.Rewards.Count))
                  .ForMember(dest => dest.Id, mo => mo.MapFrom(src => src.Rewards.Id));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<PromotionCompetition, CompRewardResponseViewModel>(Mapper);
            return afterMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static StockTRXSheetResponseViewModel CreateMap(Transaction Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, StockTRXSheetResponseViewModel>()
                .ForMember(dest => dest.Manual, mo => mo.MapFrom(src => src.ManualInd))
                .ForMember(dest => dest.SellUnitQTY, mo => mo.MapFrom(src => src.UnitQty))
                .ForMember(dest => dest.ExGstCost, mo => mo.MapFrom(src => (src.Cost - src.ExGSTCost)))
                .ForMember(dest => dest.WeekEnding, mo => mo.MapFrom(src => src.Weekend))
                .ForMember(dest => dest.Parent, mo => mo.MapFrom(src => src.Parent))
                .ForMember(dest => dest.CtnQTY, mo => mo.MapFrom(src => src.CartonQty))
                 .ForMember(dest => dest.Product, mo => mo.MapFrom(src => src.Product.Number))
                  .ForMember(dest => dest.Description, mo => mo.MapFrom(src => src.Product.Desc))
                  .ForMember(dest => dest.Outlet, mo => mo.MapFrom(src => src.Store.Desc))
                  .ForMember(dest => dest.Till, mo => mo.MapFrom(src => src.Till.Code))
                  .ForMember(dest => dest.Supplier, mo => mo.MapFrom(src => src.Supplier.Desc))
                  .ForMember(dest => dest.Commodity, mo => mo.MapFrom(src => src.Commodity.Desc))
                  .ForMember(dest => dest.Department, mo => mo.MapFrom(src => src.Department.Desc))
                  .ForMember(dest => dest.SubType, mo => mo.MapFrom(src => src.Tender))
                  .ForMember(dest => dest.Category, mo => mo.MapFrom(src => src.Category.Name));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<Transaction, StockTRXSheetResponseViewModel>(Mapper);
            return afterMap;
        }


        /// <summary>
        /// Mapper for keypad design
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static KeypadDesignResponseModel KeypadCreateMap(KeypadLevel Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<KeypadLevel, KeypadDesignResponseModel>()
                    .ForMember(dest => dest.Id, mo => mo.MapFrom(src => src.Keypad.Id))
                 .ForMember(dest => dest.Code, mo => mo.MapFrom(src => src.Keypad.Code))
                  .ForMember(dest => dest.Desc, mo => mo.MapFrom(src => src.Keypad.Desc))
                 .ForMember(dest => dest.OutletId, mo => mo.MapFrom(src => src.Keypad.OutletId))
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Keypad.Store.Code))
                .ForMember(dest => dest.OutletName, mo => mo.MapFrom(src => src.Keypad.Store.Desc))
                .ForMember(dest => dest.KeyPadButtonJSONData, mo => mo.MapFrom(src => src.Keypad.KeyPadButtonJSONData))
                .ForMember(dest => dest.Status, mo => mo.MapFrom(src => src.Keypad.Status));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<KeypadLevel, KeypadDesignResponseModel>(Mapper);
            return afterMap;
        }
        /// <summary>
        /// Mapper for keypad design
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static DesignKeypadResponseModel KeypadDesignCreateMap(KeypadLevel Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<KeypadLevel, DesignKeypadResponseModel>()
           .ForMember(dest => dest.Code, mo => mo.MapFrom(src => src.Keypad.Code))
            .ForMember(dest => dest.Desc, mo => mo.MapFrom(src => src.Keypad.Desc))
           .ForMember(dest => dest.OutletId, mo => mo.MapFrom(src => src.Keypad.OutletId))
          .ForMember(dest => dest.Status, mo => mo.MapFrom(src => src.Keypad.Status));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<KeypadLevel, DesignKeypadResponseModel>(Mapper);

            return afterMap;
        }
        public static KeypadDesignResponseModel KeypadCreateMap(Keypad Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Keypad, KeypadDesignResponseModel>()
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.OutletName, mo => mo.MapFrom(src => src.Store.Desc));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<Keypad, KeypadDesignResponseModel>(Mapper);
            return afterMap;
        }

        /// <summary>
        /// Mapper for Keypad Level
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static KeypadLevelResponseModel CreateMap(KeypadLevel Mapper)
        {

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<KeypadLevel, KeypadLevelResponseModel>()
                .ForMember(dest => dest.LevelId, mo => mo.MapFrom(src => src.Id))
                .ForMember(dest => dest.LevelIndex, mo => mo.MapFrom(src => src.LevelIndex))
                .ForMember(dest => dest.LevelDesc, mo => mo.MapFrom(src => src.Desc));
            });
            iMapper = config.CreateMapper();

            var afterMap = iMapper.Map<KeypadLevel, KeypadLevelResponseModel>(Mapper);
            afterMap.KeypadButtons.AddRange(Mapper?.KeypadButton.Select(CreateMap).ToList());
            return afterMap;
        }
        /// <summary>
        /// Mapper for Keypad Level
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static KeypadLevelsResponseModel CreateLevelMap(KeypadLevel Mapper)
        {

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<KeypadLevel, KeypadLevelsResponseModel>();
            });
            iMapper = config.CreateMapper();

            var afterMap = iMapper.Map<KeypadLevel, KeypadLevelsResponseModel>(Mapper);
            afterMap.Levels.AddRange(Mapper?.KeypadButton.Select(CreateMap).ToList());
            return afterMap;
        }
        public static KeypadButtonResponseModel CreateMap(KeypadButton Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<KeypadButton, KeypadButtonResponseModel>()
                .ForMember(dest => dest.TypeCode, mo => mo.MapFrom(src => src.ButtonType.Code))
                .ForMember(dest => dest.ButtonIndex, mo => mo.MapFrom(src => src.ButtonIndex))
                .ForMember(dset => dset.ProductId, mo => mo.MapFrom(src => src.ProductId))
                .ForMember(dset => dset.CategoryId, mo => mo.MapFrom(src => src.CategoryId))
                .ForMember(dset => dset.LevelId, mo => mo.MapFrom(src => src.BtnKeypadLevelId))
                .ForMember(dset => dset.ButtonLevelIndex, mo => mo.MapFrom(src => src.BtnKeypadLevel.LevelIndex))
                .ForMember(dest => dest.TypeDesc, mo => mo.MapFrom(src => src.ButtonType.Name));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<KeypadButton, KeypadButtonResponseModel>(Mapper);
            AfterMap.SizeDesc = Enum.GetName(typeof(ButtonSize), AfterMap?.Size);
            return AfterMap;
        }

        /// <summary>
        /// Mapper for Outlet Supplier Setting
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static OutletSupplierResponseModel CreateMap(OutletSupplierSetting Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OutletSupplierSetting, OutletSupplierResponseModel>()
                .ForMember(dest => dest.StoreCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.StoreName, mo => mo.MapFrom(src => src.Store.Desc))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                .ForMember(dest => dest.SupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc))
                .ForMember(dest => dest.StateCode, mo => mo.MapFrom(src => src.StateMasterListItem.Code))
                .ForMember(dest => dest.StateName, mo => mo.MapFrom(src => src.StateMasterListItem.Name))
                .ForMember(dest => dest.DivisionCode, mo => mo.MapFrom(src => src.DivisionMasterListItem.Code))
                .ForMember(dest => dest.DivisionName, mo => mo.MapFrom(src => src.DivisionMasterListItem.Name));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<OutletSupplierSetting, OutletSupplierResponseModel>(Mapper);
            return AfterMap;
        }


        /// <summary>
        /// Mapper for GLAccount
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static GLAccountResponseModel CreateMap(GLAccount Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GLAccount, GLAccountResponseModel>()
                .ForMember(dest => dest.StoreCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.StoreDesc, mo => mo.MapFrom(src => src.Store.Desc))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                .ForMember(dest => dest.SupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc))
                .ForMember(dest => dest.TypeCode, mo => mo.MapFrom(src => src.TypeMasterListItem.Code))
                .ForMember(dest => dest.TypeName, mo => mo.MapFrom(src => src.TypeMasterListItem.Name));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<GLAccount, GLAccountResponseModel>(Mapper);
            AfterMap.AccountSystemId = (int)Enum.Parse(typeof(GLAccountSystem), Mapper?.AccountSystem);
            AfterMap.AccountSystem = Enum.GetName(typeof(GLAccountSystem), AfterMap.AccountSystemId);
            return AfterMap;
        }

        /// <summary>
        /// Mapper for Xero Account
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static XeroAccountReponseModel CreateMap(XeroAccount Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<XeroAccount, XeroAccountReponseModel>()
                .ForMember(dest => dest.StoreCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.StoreDesc, mo => mo.MapFrom(src => src.Store.Desc));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<XeroAccount, XeroAccountReponseModel>(Mapper);
            return AfterMap;
        }

        public static OutletProductResponseViewModel CreateMap(OutletProduct Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OutletProduct, OutletProductResponseViewModel>()
                .ForMember(dest => dest.ProductNumber, mo => mo.MapFrom(src => src.Product.Number))
                .ForMember(dest => dest.ProductDesc, mo => mo.MapFrom(src => src.Product.Desc))
                .ForMember(dest => dest.Parent, mo => mo.MapFrom(src => src.Product.Parent))
                .ForMember(dest => dest.TypeId, mo => mo.MapFrom(src => src.Product.TypeId))
                .ForMember(dest => dest.TypeCode, mo => mo.MapFrom(src => src.Product.TypeMasterListItem.Code))
                .ForMember(dest => dest.Type, mo => mo.MapFrom(src => src.Product.TypeMasterListItem.Name))
                .ForMember(dest => dest.GroupId, mo => mo.MapFrom(src => src.Product.GroupId))
                .ForMember(dest => dest.GroupCode, mo => mo.MapFrom(src => src.Product.GroupMasterListItem.Code))
                .ForMember(dest => dest.GroupDesc, mo => mo.MapFrom(src => src.Product.GroupMasterListItem.Name))
                .ForMember(dest => dest.DepartmentId, mo => mo.MapFrom(src => src.Product.DepartmentId))
                .ForMember(dest => dest.DepartmentCode, mo => mo.MapFrom(src => src.Product.Department.Code))
                .ForMember(dest => dest.DepartmentDesc, mo => mo.MapFrom(src => src.Product.Department.Desc))
                .ForMember(dest => dest.TaxId, mo => mo.MapFrom(src => src.Product.TaxId))
                .ForMember(dest => dest.TaxCode, mo => mo.MapFrom(src => src.Product.Tax.Code))
                .ForMember(dest => dest.TaxDesc, mo => mo.MapFrom(src => src.Product.Tax.Desc))
                .ForMember(dest => dest.CategoryId, mo => mo.MapFrom(src => src.Product.CategoryId))
                .ForMember(dest => dest.CategoryCode, mo => mo.MapFrom(src => src.Product.CategoryMasterListItem.Code))
                .ForMember(dest => dest.CategoryDesc, mo => mo.MapFrom(src => src.Product.CategoryMasterListItem.Name))
                .ForMember(dest => dest.CommodityId, mo => mo.MapFrom(src => src.Product.CommodityId))
                .ForMember(dest => dest.CommodityCode, mo => mo.MapFrom(src => src.Product.Commodity.Code))
                .ForMember(dest => dest.CommodityDesc, mo => mo.MapFrom(src => src.Product.Commodity.Desc))
                .ForMember(dest => dest.Replicate, mo => mo.MapFrom(src => src.Product.Replicate))
                .ForMember(dest => dest.UnitQty, mo => mo.MapFrom(src => src.Product.UnitQty))
                .ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.Product.CartonQty))
                .ForMember(dest => dest.StoreDesc, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.StoreDesc, mo => mo.MapFrom(src => src.Store.Desc))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                .ForMember(dest => dest.SupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc))
                //.ForMember(dest => dest.ItemCost, mo => mo.MapFrom(src => (src.CartonCost / src.Product.CartonQty)))
                .ForMember(dest => dest.MemberOfferCode, mo => mo.MapFrom(src => src.PromotionMember.Code))

                .ForMember(dest => dest.MixMatch1PromoCode, mo => mo.MapFrom(src => src.PromotionMixMatch1.Code))
                .ForMember(dest => dest.MixMatch2PromoCode, mo => mo.MapFrom(src => src.PromotionMixMatch2.Code))

                .ForMember(dest => dest.Offer1PromoCode, mo => mo.MapFrom(src => src.PromotionOffer1.Code))
                .ForMember(dest => dest.Offer2PromoCode, mo => mo.MapFrom(src => src.PromotionOffer2.Code))
                .ForMember(dest => dest.Offer3PromoCode, mo => mo.MapFrom(src => src.PromotionOffer3.Code))
                .ForMember(dest => dest.Offer4PromoCode, mo => mo.MapFrom(src => src.PromotionOffer4.Code))

                .ForMember(dest => dest.BuyPromoCode, mo => mo.MapFrom(src => src.PromotionBuy.Code))

                .ForMember(dest => dest.SellPromoCode, mo => mo.MapFrom(src => src.PromotionSell.Code));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<OutletProduct, OutletProductResponseViewModel>(Mapper);

            float? itemCost = 0;
            if (Mapper?.Product?.CartonQty > 0 && Mapper?.Product?.UnitQty > 0)
                itemCost = (Mapper.CartonCost / Mapper.Product.CartonQty) * Mapper.Product.UnitQty;
            AfterMap.ItemCost = itemCost;

            if (AfterMap.NormalPrice1 > 0)
            {
                float? calculatedItemCost = null;
                int? prodCartonQty = null;
                if (Mapper.Product.CartonQty > 0)
                {
                    prodCartonQty = Mapper.Product.CartonQty;
                    calculatedItemCost = (Mapper.CartonCost / prodCartonQty) * Mapper.Product.UnitQty;
                }
                AfterMap.GP = Math.Round(Convert.ToDecimal(((Mapper.NormalPrice1 - (calculatedItemCost)) * 100) / (Mapper.NormalPrice1)), 1);
            }
            return AfterMap;
        }

        public static SupplierOrderScheduleResponseModel CreateMap(SupplierOrderingSchedule Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SupplierOrderingSchedule, SupplierOrderScheduleResponseModel>()
                .ForMember(dest => dest.StoreCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.StoreDesc, mo => mo.MapFrom(src => src.Store.Desc))
                .ForMember(dest => dest.UserName, mo => mo.MapFrom(src => src.UserCreatedBy.UserName))
                .ForMember(dest => dest.SupplierCode, mo => mo.MapFrom(src => src.Supplier.Code))
                .ForMember(dest => dest.SupplierDesc, mo => mo.MapFrom(src => src.Supplier.Desc));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<SupplierOrderingSchedule, SupplierOrderScheduleResponseModel>(Mapper);

            AfterMap.GenerateOrderDOW = Enum.GetName(typeof(DaysOfWeek), Mapper?.DOWGenerateOrder);

            return AfterMap;
        }

        public static POSMessagesResponseModel CreateMap(POSMessages Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<POSMessages, POSMessagesResponseModel>()
                .ForMember(dest => dest.ZoneDesc, mo => mo.MapFrom(src => src.Zone.Name))
                .ForMember(dest => dest.ZoneCode, mo => mo.MapFrom(src => src.Zone.Code))
                .ForMember(dest => dest.ReferenceOverrideType, mo => mo.MapFrom(src => src.ReferenceOverrideType));

            });

            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<POSMessages, POSMessagesResponseModel>(Mapper);
            AfterMap.DisplayTypeId = (int)Enum.Parse(typeof(MessageDisplayType), Mapper?.DisplayType);
            AfterMap.ReferenceTypeId = (int)Enum.Parse(typeof(MessageRefType), Mapper?.ReferenceType);
            if (AfterMap.DisplayTypeId == (int)MessageDisplayType.CFD_DEFAULT)
            {
                AfterMap.DisplayType = EnumHelper.GetDescription<MessageDisplayType>(MessageDisplayType.CFD_DEFAULT);
            }

            if (!string.IsNullOrEmpty(Mapper?.ReferenceOverrideType))
                AfterMap.ReferenceOverrideTypeId = (int)Enum.Parse(typeof(MessageRefType), Mapper?.ReferenceOverrideType);

            return AfterMap;
        }

        public static POSMessages CreateMap(POSMessageRequestModel Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<POSMessageRequestModel, POSMessages>();
            });
            iMapper = config.CreateMapper();

            var AfterMap = iMapper.Map<POSMessageRequestModel, POSMessages>(Mapper);

            AfterMap.ReferenceType = Enum.GetName(typeof(MessageRefType), Mapper?.ReferenceTypeId);
            AfterMap.ReferenceOverrideType = Enum.GetName(typeof(MessageRefType), Mapper?.ReferenceOverrideTypeId);
            //var enumType = Enum.GetName(typeof(MessageDisplayType), Mapper?.DisplayTypeId);
            AfterMap.DisplayType = Enum.GetName(typeof(MessageDisplayType), Mapper?.DisplayTypeId);
            return AfterMap;
        }

        public static CorporateGroupTillResponseModel CreateCorporateTillMap(Till Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Till, CorporateGroupTillResponseModel>()
                .ForMember(dest => dest.Code, mo => mo.MapFrom(src => src.Code));
            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<Till, CorporateGroupTillResponseModel>(Mapper);
            return AfterMap;
        }

        public static CorporateGroupStoreResponseModel CreateCorporateStoreMap(Store Mapper)
        {
            var afterMap = Mapping<Store, CorporateGroupStoreResponseModel>(Mapper);
            if (Mapper?.TillOutlet != null && Mapper.TillOutlet.Count > 0)
            {
                var tillList = Mapper?.TillOutlet.Where(x => !x.IsDeleted).Select(CreateCorporateTillMap);
                afterMap.Till.AddRange(tillList);
            }
            return afterMap;
        }

        public static CorporateTreeResponseModel CreateCorporateTreeMap(StoreGroup Mapper)
        {
            var afterMap = Mapping<StoreGroup, CorporateTreeResponseModel>(Mapper);
            if (Mapper?.Stores != null && Mapper.Stores.Count > 0)
            {
                var tillList = Mapper?.Stores.Where(x => !x.IsDeleted).Select(CreateCorporateStoreMap);
                afterMap.StoresList.AddRange(Mapper?.Stores.Select(CreateCorporateStoreMap).ToList());
            }
            return afterMap;
        }

        public static MassPriceUpdateResponseModel CreateMassPriceMap(OutletProduct Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OutletProduct, MassPriceUpdateResponseModel>()
                .ForMember(dest => dest.ProductNumber, mo => mo.MapFrom(src => src.Product.Number))
                .ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.Product.CartonQty))
                .ForMember(dest => dest.UnitQty, mo => mo.MapFrom(src => src.Product.UnitQty));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<OutletProduct, MassPriceUpdateResponseModel>(Mapper);

            return AfterMap;
        }


        public static UserLogResponseModel<T> CreateUserLog<T>(UserLog Mapper) where T : class
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserLog, UserLogResponseModel<T>>()
          .ForMember(dest => dest.UserNumber, mo => mo.MapFrom(src => src.ActionById.UserName))
          .ForMember(dest => dest.UserName, mo => mo.MapFrom(src => src.ActionById.FirstName));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<UserLog, UserLogResponseModel<T>>(Mapper);

            if (Mapper?.DataLog != null)
            {
                //AfterMap.DataLogs = AfterMap.DataLog.Replace(System.Environment.NewLine, string.Empty).Replace("\"", string.Empty);

                AfterMap.DataLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<DataLog<T>>(Mapper?.DataLog);
            }
            if (Mapper?.ActionById != null)
            {
                AfterMap.UserName = $"{Mapper?.ActionById?.FirstName} {Mapper?.ActionById?.LastName }";
            }
            return AfterMap;
        }

        public static ManualSaleResponseModel CreateManualSaleMap(ManualSale Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ManualSale, ManualSaleResponseModel>()
                .ForMember(dest => dest.TotalSalesAmt, mo => mo.MapFrom(src => src.ManualSaleItem.Where(x => !x.IsDeleted).Sum(x => x.Amount)));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<ManualSale, ManualSaleResponseModel>(Mapper);
            return afterMap;
        }

        public static ManualSaleItemResponseModel CreateManualSaleItemMap(ManualSaleItem Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ManualSaleItem, ManualSaleItemResponseModel>()
                .ForMember(dest => dest.ProductNumber, mo => mo.MapFrom(src => src.Product.Number))
                .ForMember(dest => dest.ProductDesc, mo => mo.MapFrom(src => src.Product.Desc))
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.OutletDesc, mo => mo.MapFrom(src => src.Store.Desc));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<ManualSaleItem, ManualSaleItemResponseModel>(Mapper);
            return afterMap;
        }

        public static RebateResponseModel CreateRebateMap(RebateHeader Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RebateHeader, RebateResponseModel>()
                // .ForMember(dest => dest.RebateOutlets, mo => mo.MapFrom(src => src.RebateOutlets.Where(x => !x.IsDeleted).Select(x => x.StoreId)))
                .ForMember(dest => dest.ManufacturerCode, mo => mo.MapFrom(src => src.Manufacturer.Code))
                .ForMember(dest => dest.ManufacturerDesc, mo => mo.MapFrom(src => src.Manufacturer.Name))
                .ForMember(dest => dest.ManufacturerIsDeleted, mo => mo.MapFrom(src => src.Manufacturer.IsDeleted))
                .ForMember(dest => dest.ZoneCode, mo => mo.MapFrom(src => src.Zone.Code))
                .ForMember(dest => dest.ZoneDesc, mo => mo.MapFrom(src => src.Zone.Name))
                .ForMember(dest => dest.ZoneIsDeleted, mo => mo.MapFrom(src => src.Zone.IsDeleted));

            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<RebateHeader, RebateResponseModel>(Mapper);
            if (afterMap != null && Mapper?.RebateDetails.Count > 0)
            {
                var details = Mapper.RebateDetails.Where(x => !x.IsDeleted).Select(CreateRebateDetailMap).ToList();
                afterMap.RebateDetailsList.AddRange(details);
            }
            if (afterMap != null && Mapper?.RebateOutlets.Count > 0)
            {
                var outlets = Mapper.RebateOutlets.Where(x => !x.IsDeleted).Select(x => x.StoreId).ToList();
                afterMap.RebateOutletsList.AddRange(outlets);
            }
            return afterMap;
        }
        public static RebateDetailResponseModel CreateRebateDetailMap(RebateDetails Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RebateDetails, RebateDetailResponseModel>()
                .ForMember(dest => dest.Number, mo => mo.MapFrom(src => src.Product.Number))
                .ForMember(dest => dest.ProductDesc, mo => mo.MapFrom(src => src.Product.Desc))
                .ForMember(dest => dest.CartonQty, mo => mo.MapFrom(src => src.Product.CartonQty));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<RebateDetails, RebateDetailResponseModel>(Mapper);

            return afterMap;
        }

        public static TDest Mapping<TSource, TDest>(TSource Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDest>();
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<TSource, TDest>(Mapper);
            return afterMap;
        }


        public static T ConvertToObject<T>(this SqlDataReader rd) where T : class, new()
        {
            Type type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var t = new T();

            for (int i = 0; i < rd.FieldCount; i++)
            {
                if (!rd.IsDBNull(i))
                {
                    string fieldName = rd.GetName(i);

                    if (members.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                    {
                        accessor[t, fieldName] = rd.GetValue(i);
                    }
                }
            }

            return t;
        }
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt?.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr?.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    try
                    {

                        //if (pro.Name == column.ColumnName)
                        if (string.Equals(pro.Name, column.ColumnName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (pro.PropertyType.Namespace != "System.Collections.Generic")
                            {
                                if (!object.Equals(dr[column.ColumnName], DBNull.Value))
                                {
                                    if (pro.PropertyType.Name.Contains("Nullable"))
                                    {
                                        pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], Nullable.GetUnderlyingType(pro.PropertyType)), null);
                                    }
                                    else
                                    {
                                        pro.SetValue(obj, (dr[column.ColumnName] == DBNull.Value) ? null : dr[column.ColumnName], null);
                                    }
                                }
                                else
                                {
                                    pro.SetValue(obj, (dr[column.ColumnName] == DBNull.Value) ? null : dr[column.ColumnName], null);
                                }
                            }
                            else
                                continue;
                        }

                        else
                            continue;
                    }
                    catch (Exception ex)
                    {
                        string exm = ex.Message;
                        throw;
                    }

                }
            }
            return obj;
        }
        public static DataTable ToDataTable<T>(List<T> items, bool isIdentityRowRequired = false)
        {
            using (DataTable dataTable = new DataTable(typeof(T).Name))
            {
                if (isIdentityRowRequired)
                {
                    dataTable.Columns.Add("RowIndex", typeof(System.Int64));
                }
                //Get all the properties by using reflection   
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names  
                    dataTable.Columns.Add(prop.Name);
                }
                int itemCounter = 0;
#pragma warning disable CA1062 // Validate arguments of public methods
                foreach (T item in items)
#pragma warning restore CA1062 // Validate arguments of public methods
                {
                    itemCounter++;
                    var values = new object[isIdentityRowRequired == true ? (Props.Length + 1) : Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        if (i == 0)
                        {
                            values[i] = itemCounter;
                        }
                        if (isIdentityRowRequired)
                            values[i + 1] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                return dataTable;
            }
        }
        public static DataTable ToDataTable<T>(T items)
        {
            using (DataTable dataTable = new DataTable(typeof(T).Name))
            {
                //Get all the properties by using reflection   
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names  
                    if (prop.PropertyType.Namespace == "System.Collections.Generic")
                    {
                        dataTable.Columns.Add(prop.Name, typeof(string));
                    }
                    else
                    {
                        dataTable.Columns.Add(prop.Name);
                    }
                }
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    if (Props[i].PropertyType.Namespace != "System.Collections.Generic")
                    {
                        values[i] = Props[i].GetValue(items, null);
                    }
                }
                dataTable.Rows.Add(values);

                return dataTable;
            }
        }

        /// <summary>
        /// Mapper for Till Sync
        /// </summary>
        /// <param name="Mapper"></param>
        /// <returns></returns>
        public static TillSyncResponseModel CreateMap(TillSync Mapper)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TillSync, TillSyncResponseModel>()
                .ForMember(dest => dest.ProductSync, mo => mo.MapFrom(src => Enum.GetName(typeof(TillSyncType), src.Product)))
                .ForMember(dest => dest.CashierSync, mo => mo.MapFrom(src => Enum.GetName(typeof(TillSyncType), src.Cashier)))
                .ForMember(dest => dest.KeypadSync, mo => mo.MapFrom(src => Enum.GetName(typeof(TillSyncType), src.Keypad)))
                .ForMember(dest => dest.AccountSync, mo => mo.MapFrom(src => Enum.GetName(typeof(TillSyncType), src.Account)));

            });
            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<TillSync, TillSyncResponseModel>(Mapper);
            if (AfterMap?.TillActivity != null && (AfterMap?.TillActivity == DateTime.MinValue))
            { AfterMap.TillActivity = null; }
            return AfterMap;
        }

        public static PathsResponseModel CreatePatheMap(Paths paths)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Paths, PathsResponseModel>()
                .ForMember(dest => dest.OutletCode, mo => mo.MapFrom(src => src.Store.Code))
                .ForMember(dest => dest.OutletDesc, mo => mo.MapFrom(src => src.Store.Desc))
                .ForMember(dest => dest.PathTypeName, mo => mo.MapFrom(src => Enum.GetName(typeof(PathType), src.PathType)));
            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<Paths, PathsResponseModel>(paths);
            return afterMap;
        }

        public static HostSettingsResponseModel CreateHostSettingsMap(HostSettings hostSettings)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HostSettings, HostSettingsResponseModel>()
                .ForMember(dest => dest.FilePath, mo => mo.MapFrom(src => src.Path.Path))
                .ForMember(dest => dest.Supplier, mo => mo.MapFrom(src => src.Supplier.Desc))
                .ForMember(dest => dest.WareHouse, mo => mo.MapFrom(src => src.Warehouse.Desc))
                .ForMember(dest => dest.HostFormatName, mo => mo.MapFrom(src => src.HostFormatWareHouse.Name))
                .ForMember(dest => dest.HostFormatCode, mo => mo.MapFrom(src => src.HostFormatWareHouse.Code));

            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<HostSettings, HostSettingsResponseModel>(hostSettings);
            return afterMap;
        }

        public static CostPriceZonesResponseModel CreateCostPriceZonesMap(CostPriceZones costPriceZones)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CostPriceZones, CostPriceZonesResponseModel>()
                .ForMember(dest => dest.HostSetting, mo => mo.MapFrom(src => src.HostSettings.Description))
                .ForMember(dest => dest.Type, mo => mo.MapFrom(src => Enum.GetName(typeof(CostPriceZoneType), src.Type)));

            });
            iMapper = config.CreateMapper();
            var afterMap = iMapper.Map<CostPriceZones, CostPriceZonesResponseModel>(costPriceZones);
            return afterMap;
        }
        public static string DataTableToJsonObj(DataTable dt, string tabelName)
        {
            DataSet ds = new DataSet();
            ds.Merge(dt);
            StringBuilder JsonString = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (tabelName != "")
                    JsonString.Append("{'" + tabelName + "':");
                if (tabelName != "")
                    JsonString.Append("[");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        if (j < ds.Tables[0].Columns.Count - 1)
                        {
                            if (ds.Tables[0].Columns[j].ColumnName.ToString() == "YTD")
                                JsonString.Append(ds.Tables[0].Columns[j].ColumnName.ToString() + ":" + ds.Tables[0].Rows[i]["YTD"].ToString());
                            else
                                JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\",");
                        }
                        else if (j == ds.Tables[0].Columns.Count - 1)
                        {
                            if (ds.Tables[0].Columns[j].ColumnName.ToString() == "YTD")
                                JsonString.Append(ds.Tables[0].Columns[j].ColumnName.ToString() + ":" + ds.Tables[0].Rows[i]["YTD"].ToString());
                            else
                                JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                if (tabelName != "")
                    JsonString.Append("]");
                if (tabelName != "")
                    JsonString.Append("}");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}