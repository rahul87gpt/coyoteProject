using System;
using System.Globalization;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private IUnitOfWork _unitOfWork = null;
        private ILoggerManager _iLoggerManager = null;
        public EmailTemplateService(IUnitOfWork context, ILoggerManager iLoggerManager)
        {
            _unitOfWork = context;
            _iLoggerManager = iLoggerManager;
        }

        public async Task<EmailTemplate> GetEmailTemplateByName(string templateName)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateName))
                {
                    var repository = _unitOfWork?.GetRepository<EmailTemplate>();
                    var template = (await repository.GetAll(x => x.Name == templateName).FirstOrDefaultAsync().ConfigureAwait(false));
                    if (template == null)
                    {
                        throw new NotFoundException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    return template;
                }
                throw new NullReferenceException(ErrorMessages.MasterListNotFound.ToString(CultureInfo.CurrentCulture));
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
