using System;
using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class SystemControlsResponseModel
    {
        public int Id { get; set; }
        public DateTime ExpiryDate { get; set; }
   
        public string Name { get; set; }
    
        public string SerialNo { get; set; }
   
        public string LicenceKey { get; set; }
        public int? MaxStores { get; set; }
       
        public TillJournal TillJournal { get; set; }
        public bool? AllocateGroups { get; set; } = false;
       
        public string MassPriceUpdate { get; set; }
        public bool? AllowALM { get; set; } = false;
        public string DatabaseUsage { get; set; }
        public float GrowthFactor { get; set; }
        public bool AllowFIFO { get; set; } = false;
    
        public string Color { get; set; }
        public string TransactionRef { get; set; }
        public string WastageRef { get; set; }
        public string TransferRef { get; set; }
      
        public long NumberFactor { get; set; }
        public bool HostUpdatePricing { get; set; } = false;
        public bool InvoicePostPricing { get; set; } = false;
        public PriceRounding PriceRounding { get; set; } //('None''5Cents')
        public DefaultItemPricing DefaultItemPricing { get; set; }//( 'Price''HoldGP%''HostPrice''BestPrice')
    }
}
