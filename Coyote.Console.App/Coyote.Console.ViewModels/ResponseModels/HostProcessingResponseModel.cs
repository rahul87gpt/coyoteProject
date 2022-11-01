using System;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class HostProcessingResponseModel
    {
        public int ID { get; set; }    
        public long Number { get; set; }       
        public string Code { get; set; }     
        public string Description { get; set; }     
        public string TimeStamp { get; set; }
        public bool? Posted { get; set; }
    }
}
