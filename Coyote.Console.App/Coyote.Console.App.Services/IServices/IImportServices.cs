using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IImportServices
    {
        Task<bool> ImportCSVToTable(ImportFilterRequestModel reportRequest, ExportFilterRequestModel tableFilter, string path, int userId);
    }
}
