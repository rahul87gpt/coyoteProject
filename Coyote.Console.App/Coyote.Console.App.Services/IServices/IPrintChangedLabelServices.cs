using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IPrintChangedLabelServices
    {
        Task<List<PrintChangeLabelResponseModel>> GetPrintLabelChangedAsync();
        Task<List<RePrintChangeLabelResponseModel>> GetRePrintLabelChanged();

        Task<List<SpecPrintChangeLabelResponseModel>> GetSpecPrintLabelChanged();
        Task<PagedOutputModel<List<SpecPrintChangeLabelResponseModel>>> GetSpecPrintLabelChanged(PagedInputModel inputModel);
        Task<PagedOutputModel<List<PromotionResponseViewModel>>> GetPromotionLabelChanged(PromotionFilter inputModel, SecurityViewModel securityViewModel);
        Task<PagedOutputModel<List<PromotionResponseViewModel>>> GetPromotionPrintLabel(PromotionFilter inputModel, SecurityViewModel securityViewModel);
        Task<List<PrintLabelFromTabletPDEMode>> GetPrintLabelFromPDE(int StoreId);
        Task<List<PrintLabelFromTableResponseModel>> GetPrintLabelFromTablet(SecurityViewModel securityViewModel);
        Task<byte[]> GetPrintLabelPDEImport(PrintLabelRequestModel requestModel, string mime);
        Task<PagedOutputModel<List<RePrintChangeLabelResponseModel>>> GetRePrintChangedLabel(PagedInputModel inputModel);
        Task<PagedOutputModel<List<PrintChangeLabelResponseModel>>> GetPrintChangedLabel(PagedInputModel inputModel);
    }
}
