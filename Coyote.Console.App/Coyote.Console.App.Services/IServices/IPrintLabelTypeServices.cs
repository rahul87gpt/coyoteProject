using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IPrintLabelTypeServices
    {
        Task<PagedOutputModel<List<PrintLabelTypeResponseModel>>> GetAllActivePrintLabelTypes(PagedInputModel inputModel = null);
        Task<PrintLabelTypeResponseModel> GetPrintLabelTypeById(long id);
        Task<PrintLabelTypeResponseModel> Insert(PrintLabelTypeRequestModel viewModel, int userId);
        Task<PrintLabelTypeResponseModel> Update(PrintLabelTypeRequestModel viewModel, int Id, int userId);
        Task<bool> Delete(int labelId, int userId);
    }
}
