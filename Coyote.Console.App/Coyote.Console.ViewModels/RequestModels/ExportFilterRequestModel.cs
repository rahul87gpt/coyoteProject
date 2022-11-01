using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class ExportFilterRequestModel
    {
        public bool Product { get; set; }
        public bool OutletProduct { get; set; }
        public bool APN { get; set; }
        public bool SupplierProduct { get; set; }
        public bool StoreGroups { get; set; }
        public bool Stores { get; set; }
        public bool Tills { get; set; }
        public bool Groups { get; set; }
        public bool Commodieties { get; set; }
        public bool Category { get; set; }
        public bool Department { get; set; }
        public bool SubRanges { get; set; }
        public bool Manufacturers { get; set; }
        public bool Suppliers { get; set; }
        public bool TaxCodes { get; set; }
        public bool Users { get; set; }
        public bool Cashiers { get; set; }
        public bool PriceZones { get; set; }
        public bool CostZones { get; set; }
        public bool GeneralZones { get; set; }
        public bool SystemPathSettings { get; set; }
        public bool UserTypes { get; set; }
        public bool CashierTypes { get; set; }
        public bool HostSettings { get; set; }
        public bool SystemSettings { get; set; }
        public bool OutletSupplier { get; set; }
        public bool EpayProd { get; set; }
        public bool TouchProd { get; set; }
        public bool EpayV2Prod { get; set; }
    }


    public class ImportFilterRequestModel
    {
        public IFormFile ImportCSV { get; set; }
        public bool AddNewOnly { get; set; } = false;
        public bool AddNewAsInactive { get; set; } =false;
        public string OnlyOutlet { get; set; }
        [Required]
        public string ImportPassword { get; set; }
    }
}
