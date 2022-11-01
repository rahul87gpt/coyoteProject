using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
   public class GLAccountRequestModel
    {
        [MaxLength(MaxLengthConstants.MaxGLADescLength, ErrorMessage = ErrorMessages.MaxGLADescLength)]
        [Required]
        public string Desc { get; set; }

        [Required]
        public int AccountSystemId { get; set; }

        [MaxLength(MaxLengthConstants.MaxGLANumberLength, ErrorMessage = ErrorMessages.MaxGLANumberLength)]
        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public int StoreId { get; set; }
        [Required]
        public int SupplierId { get; set; }
        [Required]
        public int TypeId { get; set; }

        public int? Company { get; set; }
    }
}
