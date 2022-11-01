using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IKeypadServices
    {
        Task<KeypadDesignResponseModel> Insert(KeypadRequestModel viewModel, int userId);
        Task<KeypadResponseModel> Update(int keypadId, KeypadRequestModel viewModel, int userId);

        Task<KeypadResponseModel> GetKeypadById(long keypadId);

        Task<PagedOutputModel<List<KeypadResponseModel>>> GetAllActiveKeypads(PagedInputModel inputModel, SecurityViewModel securityViewModel);

        Task<bool> Delete(long keypadId, int userId);

        Task<bool> DeleteMultiple(string keypadIds, int userId);
        Task<KeypadDesignResponseModel> GetKeypadDesign(int keypadId);
        Task<DesignKeypadResponseModel> GetAllKeypadDesign(int keypadId);
      //  Task<KeypadDesignResponseModel> AddKeypadDesignWithIndex(KeypadDesignRequestModel requestModel, int userId);
        Task<KeypadDesignResponseModel> UpdateKeypadDesignWithIndex(KeypadDesignRequestModel requestModel, int keypadId, int userId);
        Task<KeypadDesignResponseModel> UpdateKeypadDesignUsingSP(KeypadDesignRequestModel requestModel, int keypadId, int userId);


    }
}
