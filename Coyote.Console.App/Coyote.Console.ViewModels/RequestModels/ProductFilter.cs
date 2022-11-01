using Coyote.Console.App.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class ProductFilter : PagedInputModel
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public string Dept { get; set; }  
        public string Replicate { get; set; }
        public string StoreId { get; set; }

        public string SupplierId { get; set; }

    }

    public class ProductSupplier {

        public List<Product> Product { get; } = new List<Product>();
        public int SupplierId {get;set;}
    }
}
