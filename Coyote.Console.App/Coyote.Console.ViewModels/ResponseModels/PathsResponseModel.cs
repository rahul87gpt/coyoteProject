using Coyote.Console.Common;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class PathsResponseModel
    {
        public int ID { get; set; }
        public int? OutletID { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public PathType PathType { get; set; }
        public string PathTypeName { get; set; }
    }
}
