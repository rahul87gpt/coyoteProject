using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
  public class ReportFilterationResponseModel
  {
    public List<MasterListItemResponseViewModel> Categories { get; } = new List<MasterListItemResponseViewModel>();
    public List<MasterListItemResponseViewModel> Groups { get; } = new List<MasterListItemResponseViewModel>();
    public List<MasterListItemResponseViewModel> Zones { get; } = new List<MasterListItemResponseViewModel>();
    public List<MasterListItemResponseViewModel> Manufacturers { get; } = new List<MasterListItemResponseViewModel>();

    public List<StoreResponseModel> Stores { get; } = new List<StoreResponseModel>();
    public List<CashierResponseModel> Cashiers { get; } = new List<CashierResponseModel>();
    public List<CommodityResponseModel> Commodities { get; } = new List<CommodityResponseModel>();
    public List<DepartmentResponseModel> Departments { get; } = new List<DepartmentResponseModel>();
    public List<SupplierResponseViewModel> Suppliers { get; } = new List<SupplierResponseViewModel>();

    public List<TillResponseModel> Tills { get; } = new List<TillResponseModel>();
  }
}
