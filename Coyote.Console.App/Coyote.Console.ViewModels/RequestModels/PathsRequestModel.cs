using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;
using Microsoft.AspNetCore.Http;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class PathsRequestModel
    {
        [Required]
        public PathType PathType { get; set; }
        public int? OutletID { get; set; }
        [Required]
        [MaxLength(MaxLengthConstants.DescriptionLength, ErrorMessage = ErrorMessages.DescriptionLength)]
        public string Description { get; set; }
        //[Required]
        //[MaxLength(MaxLengthConstants.PathLength, ErrorMessage = ErrorMessages.PathLength)]
        public string Path { get; set; }
        public IFormFile File { get; set; }

    }
}
