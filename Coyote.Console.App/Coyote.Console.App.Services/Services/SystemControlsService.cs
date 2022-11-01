using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class SystemControlsService: ISystemControlsService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;

        public SystemControlsService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMapping;
        }

        /// <summary>
        /// Get SystemControls 
        /// </summary>
        /// <returns><SystemControlsResponseModel></returns>
        public async Task<SystemControlsResponseModel> GetSystemControls()
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<SystemControls>();
                var systemControls = await repository.GetAll().Where(x => x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (systemControls == null)
                    throw new NotFoundException(ErrorMessages.SystemControlsNotFound.ToString(CultureInfo.CurrentCulture));
                SystemControlsResponseModel SettingResponses;
                SettingResponses = _iAutoMapper.Mapping<SystemControls, SystemControlsResponseModel>(systemControls);
                return SettingResponses;
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


        /// <summary>
        /// Insert HostSystemControls
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="hostSettingsiD"></param>
        /// <param name="userId"></param>
        /// <returns>List</returns>
        public async Task<SystemControlsResponseModel> AddUpdateSystemControls(SystemControlsRequestModel viewModel, int userId)
        {

            try
            {
                if (viewModel == null)
                    throw new NullReferenceException();
               
                var repository = _unitOfWork?.GetRepository<SystemControls>();

                var Exists = await repository.GetAll().Where(x => x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
             
                if (Exists == null)
                {
                    var comm = _iAutoMapper.Mapping<SystemControlsRequestModel, SystemControls>(viewModel);
                    comm.Name = viewModel.Name;
                    comm.SerialNo = viewModel.SerialNo;
                    comm.LicenceKey = viewModel.LicenceKey;
                    comm.MaxStores = viewModel.MaxStores;
                    comm.TillJournal = viewModel.TillJournal;
                    comm.AllocateGroups = viewModel.AllocateGroups;
                    comm.MassPriceUpdate = viewModel.MassPriceUpdate;
                    comm.AllowALM = viewModel.AllowALM;
                    comm.DatabaseUsage = viewModel.DatabaseUsage;
                    comm.GrowthFactor = viewModel.GrowthFactor;
                    comm.AllowFIFO = viewModel.AllowFIFO;

                    comm.Color = viewModel.Color;
                    comm.TransactionRef = viewModel.TransactionRef;
                    comm.WastageRef = viewModel.WastageRef;
                    comm.TransferRef = viewModel.TransferRef;
                    comm.NumberFactor = viewModel.NumberFactor;
                    comm.ExpiryDate = viewModel.ExpiryDate;
                    comm.HostUpdatePricing = viewModel.HostUpdatePricing;
                    comm.InvoicePostPricing = viewModel.InvoicePostPricing;
                    comm.PriceRounding = viewModel.PriceRounding;
                    comm.DefaultItemPricing = viewModel.DefaultItemPricing;
                  
                    comm.IsActive = Status.Active;
                    comm.CreatedBy = userId;
                    comm.CreatedDate = DateTime.UtcNow;
                    comm.ModifiedBy = userId;
                    comm.ModifiedDate = DateTime.UtcNow;
                    var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                }
                else
                {
                    Exists.Name = viewModel.Name;
                    Exists.SerialNo = viewModel.SerialNo;
                    Exists.LicenceKey = viewModel.LicenceKey;
                    Exists.MaxStores = viewModel.MaxStores;
                    Exists.TillJournal = viewModel.TillJournal;
                    Exists.AllocateGroups = viewModel.AllocateGroups;
                    Exists.MassPriceUpdate = viewModel.MassPriceUpdate;
                    Exists.AllowALM = viewModel.AllowALM;
                    Exists.DatabaseUsage = viewModel.DatabaseUsage;
                    Exists.GrowthFactor = viewModel.GrowthFactor;
                    Exists.AllowFIFO = viewModel.AllowFIFO;
                    Exists.ExpiryDate = viewModel.ExpiryDate;
                    Exists.Color = viewModel.Color;
                    Exists.TransactionRef = viewModel.TransactionRef;
                    Exists.WastageRef = viewModel.WastageRef;
                    Exists.TransferRef = viewModel.TransferRef;
                    Exists.NumberFactor = viewModel.NumberFactor;

                    Exists.HostUpdatePricing = viewModel.HostUpdatePricing;
                    Exists.InvoicePostPricing = viewModel.InvoicePostPricing;
                    Exists.PriceRounding = viewModel.PriceRounding;
                    Exists.DefaultItemPricing = viewModel.DefaultItemPricing;
                    Exists.ModifiedBy = userId;
                    Exists.ModifiedDate = DateTime.UtcNow;
                    repository.DetachLocal(_ => _.ID == Exists.ID);
                    repository.Update(Exists);
                }

                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetSystemControls().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
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
