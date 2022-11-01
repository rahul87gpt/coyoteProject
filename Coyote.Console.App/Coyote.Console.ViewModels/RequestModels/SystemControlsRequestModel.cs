using System;
using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class SystemControlsRequestModel
    {
        public DateTime ExpiryDate { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]       
        public string SerialNo { get; set; }
        [Required]
        public string LicenceKey { get; set; }
        public int? MaxStores { get; set; }
        [Required]
        public TillJournal TillJournal { get; set; }
        public bool? AllocateGroups { get; set; } = false;
        [Required]
        public string MassPriceUpdate { get; set; }
        public bool? AllowALM { get; set; } = false;
        public string DatabaseUsage { get; set; }
        public float GrowthFactor { get; set; }
        public bool AllowFIFO { get; set; } = false;
        [Required]
        public string Color { get; set; }
        public string TransactionRef { get; set; }
        public string WastageRef { get; set; }
        public string TransferRef { get; set; }
        [Required]
        public long NumberFactor { get; set; }
        public bool HostUpdatePricing { get; set; } = false;
        public bool InvoicePostPricing { get; set; } = false;
        public PriceRounding PriceRounding { get; set; } //('None''5Cents')
        public DefaultItemPricing DefaultItemPricing { get; set; }//( 'Price''HoldGP%''HostPrice''BestPrice')

    }
}
