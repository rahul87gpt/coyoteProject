using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IExportServices
    {
        Task<byte[]> GetExportedFiles(ExportFilterRequestModel inputfilter);
    }
}
