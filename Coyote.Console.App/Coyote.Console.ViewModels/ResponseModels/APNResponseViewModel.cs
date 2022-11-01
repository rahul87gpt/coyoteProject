using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class APNResponseViewModel : APNRequestModel
    {

        public long Id { get; set; }

        public long ProductNumber { get; set; }
        public string ProductDesc { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int CreatedById { get; set; }

        [Required]
        public int UpdatedById { get; set; }
    }
}
