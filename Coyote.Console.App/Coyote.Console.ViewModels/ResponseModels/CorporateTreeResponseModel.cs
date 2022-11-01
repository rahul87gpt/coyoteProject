using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class CorporateTreeResponseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public List<CorporateGroupStoreResponseModel> StoresList { get; } = new List<CorporateGroupStoreResponseModel>();
    }


    public class CorporateGroupStoreResponseModel
    { 
        public int Id { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }
        public bool Status { get; set; }
        public List<CorporateGroupTillResponseModel> Till{ get; } = new List<CorporateGroupTillResponseModel>();
    }

    public class CorporateGroupTillResponseModel
    {
        public int Id { get; set; }
        public String Code { get; set; }
        public string Desc { get; set; }
        public bool Status { get; set; }
    }

}
