using System;
using System.Globalization;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.ViewModels;

namespace Coyote.Console.App.Services
{
    public class ModuleActionsService : IModuleActionsService
    {
        private IUnitOfWork _unitOfWork = null;
        private IAutoMappingServices _iAutoMap = null;
        public ModuleActionsService(IUnitOfWork unitOfWork, IAutoMappingServices autoMappingServices)
        {
            _unitOfWork = unitOfWork;
            _iAutoMap = autoMappingServices;
        }

        public async Task<ModuleActionsViewModel> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<ModuleActions>();
                    var module = await repository.GetById(id).ConfigureAwait(false);
                    if (module == null || module.IsDeleted)
                    {
                        throw new NotFoundException(ErrorMessages.ModuleActionNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    return _iAutoMap.Mapping<ModuleActions, ModuleActionsViewModel>(module);
                }
                throw new NullReferenceException(ErrorMessages.ModuleActionNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
