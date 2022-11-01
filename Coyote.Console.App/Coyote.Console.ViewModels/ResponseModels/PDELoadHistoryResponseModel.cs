using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class PDELoadHistoryResponseModel<T> where T : class
    {

        public int Id { get; set; }
        public string Module { get; set; }
        public string Table { get; set; }
        public long TableId { get; set; }
        public string Action { get; set; }
        public PDEDataLogs<T> PDEDataLogList { get; set; }
        public int ActionBy { get; set; }
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public DateTime ActionAt { get; set; }
    }
    public class PDELoadDataLogResponseModel
    {
        public string Description { get; set; }
        public int OutletId { get; set; }
        public string OutletCode { get; set; }
        public string OutletDescription { get; set; }
        public string PDEData { get; set; }
        public string PDEQty { get; set; }
        public string Product { get; set; }
        public long ProductNumber { get; set; }
        public string Status { get; set; }
        public string Sequence { get; set; }
        public string CartonQty { get; set; }
        public string Cost { get; set; }
}


    public class PDEImportModel
    {
        public string RecordType { get; set; }
        public long ProductNumber { get; set; }
        public float Price { get; set; }
    }

    public class PDEDataLogs<T> where T : class
    {
#pragma warning disable CA2227 // Collection properties should be read only
        public List<T> Data { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
