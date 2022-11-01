using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class KeypadServices : IKeypadServices
    {
        private IUnitOfWork _unitOfWork = null;
        private IAutoMappingServices _iAutoMapper;
        private IGenericRepository<Keypad> _repository;

        public KeypadServices(IUnitOfWork unitOfWork, IAutoMappingServices autoMappingServices)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMappingServices;
            _repository = _unitOfWork?.GetRepository<Keypad>();
        }
        /// <summary>
        /// Add new keypad
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<KeypadDesignResponseModel> Insert(KeypadRequestModel viewModel, int userId)
        {
            KeypadDesignResponseModel responseModel = new KeypadDesignResponseModel();
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.OutletId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.OutletIdReq.ToString(CultureInfo.CurrentCulture));
                    }

                    var storeRepository = _unitOfWork?.GetRepository<Store>();

                    if (!(await storeRepository.GetAll(x => x.Id == viewModel.OutletId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.InvalidOutletId.ToString(CultureInfo.CurrentCulture));
                    }
                    if ((await _repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.KeyPadDuplicate.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.KeypadCopyId > 0)
                    {
                        var existing = await _repository.GetAll().Where(x => x.Id == viewModel.KeypadCopyId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                        if (existing == null)
                        {
                            throw new BadRequestException(ErrorMessages.InvalidCopyKeypad.ToString(CultureInfo.CurrentCulture));
                        }
                        existing.KeyPadButtonJSONData = LoadJson(existing.Code);
                        viewModel.KeyPadButtonJSONData = existing.KeyPadButtonJSONData;
                        var response = await CopyKeypadAsync(viewModel, userId).ConfigureAwait(false);
                        
                        return response;

                    }


                    var keypad = _iAutoMapper.Mapping<KeypadRequestModel, Keypad>(viewModel);
                    keypad.IsDeleted = false;
                    keypad.CreatedById = userId;
                    keypad.UpdatedById = userId;
                    var result = await _repository.InsertAsync(keypad).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        responseModel = await GetKeypadDesign(result.Id).ConfigureAwait(false);
                    }
                }
                return responseModel;
            }
            catch (Exception ex)
            {
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
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
        /// Update keypad
        /// </summary>
        /// <param name="keypadId"></param>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<KeypadResponseModel> Update(int keypadId, KeypadRequestModel viewModel, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    KeypadResponseModel responseModel = new KeypadResponseModel();
                    if (viewModel.OutletId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.OutletIdReq.ToString(CultureInfo.CurrentCulture));
                    }

                    var storeRepository = _unitOfWork?.GetRepository<Store>();

                    if (!(await storeRepository.GetAll(x => x.Id == viewModel.OutletId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.InvalidOutletId.ToString(CultureInfo.CurrentCulture));
                    }
                    if ((await _repository.GetAll(x => x.Code == viewModel.Code && x.Id != keypadId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.KeyPadDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var keypadExist = await _repository.GetAll(x => x.Id == keypadId).FirstAsync().ConfigureAwait(false);
                    if (keypadExist != null)
                    {
                        if (keypadExist.IsDeleted != true)
                        {
                            var keypad = _iAutoMapper.Mapping<KeypadRequestModel, Keypad>(viewModel);
                            keypad.Id = keypadId;
                            keypad.IsDeleted = false;
                            keypad.UpdatedById = userId;
                            keypad.CreatedById = keypadExist.CreatedById;
                            keypad.CreatedAt = keypadExist.CreatedAt;
                          //  keypad.KeyPadButtonJSONData = SaveJson(viewModel.KeyPadButtonJSONData, viewModel.Code);

                            //Detaching tracked entry - exists
                            _repository.DetachLocal(_ => _.Id == keypad.Id);
                            _repository.Update(keypad);

                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            responseModel = await GetKeypadById(keypad.Id).ConfigureAwait(false);
                        }
                    }
                    return responseModel;
                }
                throw new NotFoundException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
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
        /// Get keypad by Id
        /// </summary>
        /// <param name="keypadId"></param>
        /// <returns></returns>
        public async Task<KeypadResponseModel> GetKeypadById(long keypadId)
        {
            try
            {
                if (keypadId > 0)
                {
                    var keypad = await _repository.GetAll(x => x.Id == keypadId && !x.IsDeleted).Include(c => c.Store).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (keypad == null)
                    {
                        throw new NotFoundException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    KeypadResponseModel keypadModel = MappingHelpers.CreateMap(keypad);

                    return keypadModel;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get all active Keypads
        /// along with seraching,sorting and pagination
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<KeypadResponseModel>>> GetAllActiveKeypads(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var keypads = _repository.GetAll(x => !x.IsDeleted, null, includes:
                    new Expression<Func<Keypad, object>>[] { c => c.Store });

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        keypads = keypads.Where(x => x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.OutletId.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        keypads = keypads.Where(x => x.Status);


                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                        keypads = keypads.Where(x => securityViewModel.StoreIds.Contains(x.OutletId));

                    keypads = keypads.OrderByDescending(x => x.UpdatedAt);
                    count = keypads.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        keypads = keypads.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    keypads = keypads.OrderBy(x => x.Code);
                                else
                                    keypads = keypads.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    keypads = keypads.OrderBy(x => x.Desc);
                                else
                                    keypads = keypads.OrderByDescending(x => x.Code);
                                break;
                            case "outlet":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    keypads = keypads.OrderBy(x => x.OutletId);
                                else
                                    keypads = keypads.OrderByDescending(x => x.OutletId);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    keypads = keypads.OrderBy(x => x.Id);
                                else
                                    keypads = keypads.OrderByDescending(x => x.Id);
                                break;
                        }
                    }

                }

                List<KeypadResponseModel> keypadModels;
                keypadModels = (await keypads.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                return new PagedOutputModel<List<KeypadResponseModel>>(keypadModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Delete a keypad using Id.
        /// </summary>
        /// <param name="keypadId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(long keypadId, int userId)
        {
            try
            {
                if (keypadId > 0)
                {
                    var keypadExist = await _repository.GetAll(x => x.Id == keypadId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (keypadExist != null)
                    {
                        if (!keypadExist.IsDeleted)
                        {
                            // keypadExist.Code = (keypadExist.Code + "~" + keypadExist.Id);
                            keypadExist.UpdatedById = userId;
                            keypadExist.IsDeleted = true;
                            _repository?.Update(keypadExist);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceCustomException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }
                throw new NullReferenceCustomException(ErrorMessages.CashierIdRequired.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Delete multiple keypads at once.
        /// </summary>
        /// <param name="keypadIds">String as Array of multiple Ids.</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMultiple(string keypadIds, int userId)

        {
            try
            {
                if (!string.IsNullOrEmpty(keypadIds))
                {
                    var IdsList = keypadIds.Split(',');
                    foreach (var Id in IdsList)
                    {
                        var keypadId = Convert.ToInt64(Id);
                        if (keypadId > 0)
                        {
                            var keypadExist = await _repository.GetAll(x => x.Id == keypadId).FirstOrDefaultAsync().ConfigureAwait(false);
                            if (keypadExist != null)
                            {
                                if (!keypadExist.IsDeleted)
                                {
                                    keypadExist.Code = (keypadExist.Code + "~" + keypadExist.Id);
                                    keypadExist.UpdatedAt = DateTime.UtcNow;
                                    keypadExist.UpdatedById = userId;
                                    keypadExist.IsDeleted = true;
                                    _repository?.Update(keypadExist);
                                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                                }
                                else
                                {
                                    throw new NullReferenceCustomException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture));
                                }
                            }
                        }
                    }
                    return true;
                }
                throw new NullReferenceCustomException(ErrorMessages.InvalidKeypadId.ToString(CultureInfo.CurrentCulture));
            }
            catch (FormatException ex)
            {
                throw new FormatException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get keypad designs using keypad Id.
        /// </summary>
        /// <param name="keypadId"></param>
        /// <returns></returns>
        public async Task<KeypadDesignResponseModel> GetKeypadDesign(int keypadId)
        {
            try
            {
                KeypadDesignResponseModel keypadDesign = new KeypadDesignResponseModel();

                var keypadRepository = _unitOfWork.GetRepository<Keypad>();
                var keypadExist = await keypadRepository.GetAll(x => !x.IsDeleted && x.Id == keypadId).Include(c => c.Store).FirstOrDefaultAsync().ConfigureAwait(false);

                if (keypadExist != null)
                {
                    var lvlRepository = _unitOfWork.GetRepository<KeypadLevel>();
                    //var keypadLevel = await lvlRepository.GetAll(x => !x.IsDeleted && x.KeypadId == keypadId, includes:
                    //        new Expression<Func<KeypadLevel, object>>[]
                    //        { c => c.Keypad, c => c.Keypad.Store}
                    //        ).Include(c => c.KeypadButton).ThenInclude(c => c.BtnKeypadLevel)
                    //        //.ThenInclude(s => s.ButtonType)
                    //        .ToListAsyncSafe().ConfigureAwait(false);

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                    new SqlParameter("@KeyPadId", keypadId)
                    };
                    var keypadLevel = await lvlRepository.ExecuteStoredProcedure(StoredProcedures.GetKeypadDetailsById, dbParams.ToArray()).ConfigureAwait(false);

                    if (keypadLevel != null && keypadLevel.Tables.Count > 0)
                    {
                        keypadDesign = MappingHelpers.ConvertDataTable<KeypadDesignResponseModel>(keypadLevel.Tables[0]).FirstOrDefault();
                        keypadDesign.KeypadLevels.AddRange(MappingHelpers.ConvertDataTable<KeypadLevelResponseModel>(keypadLevel.Tables[1]));
                        DataTable dtKeypadButton = new DataTable();
                        dtKeypadButton = keypadLevel.Tables[2].Copy();
                        dtKeypadButton.Clear();
                        foreach (KeypadLevelResponseModel lvlItems in keypadDesign.KeypadLevels)
                        {
                            string expression = "LevelId = " + lvlItems.LevelId;
                            DataRow[] selectedRows = keypadLevel?.Tables[2]?.Select(expression);
                            dtKeypadButton.Clear();
                            foreach (DataRow item in selectedRows)
                            {
                                dtKeypadButton.Rows.Add(item.ItemArray);
                            }
                            lvlItems.KeypadButtons.AddRange(MappingHelpers.ConvertDataTable<KeypadButtonResponseModel>(dtKeypadButton));
                        }
                    }
                    else
                    {
                        keypadDesign = MappingHelpers.KeypadCreateMap(keypadExist);
                    }

                    keypadDesign.KeyPadButtonJSONData = LoadJson(keypadDesign.Code);
                    return keypadDesign;
                }
                throw new NotFoundException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture));

            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        /// <summary>
        /// Get keypad designs using keypad Id.
        /// </summary>
        /// <param name="keypadId"></param>
        /// <returns></returns>
        public async Task<DesignKeypadResponseModel> GetAllKeypadDesign(int keypadId)
        {

            try
            {
                DesignKeypadResponseModel keypadDesign = new DesignKeypadResponseModel();

                var keypadRepository = _unitOfWork.GetRepository<Keypad>();
                var keypadExist = await keypadRepository.GetAll(x => !x.IsDeleted && x.Id == keypadId).Include(c => c.Store).FirstOrDefaultAsync().ConfigureAwait(false);

                if (keypadExist != null)
                {
                    var lvlRepository = _unitOfWork.GetRepository<KeypadLevel>();
                    var keypadLevel = await lvlRepository.GetAll(x => !x.IsDeleted && x.KeypadId == keypadId, includes:
                            new Expression<Func<KeypadLevel, object>>[]
                            { c => c.Keypad, c => c.Keypad.Store}
                            ).Include(c => c.KeypadButton).ThenInclude(s => s.ButtonType).ToListAsyncSafe().ConfigureAwait(false);
                    if (keypadLevel != null && keypadLevel.Count > 0)
                    {
                        keypadDesign = MappingHelpers.KeypadDesignCreateMap(keypadLevel.FirstOrDefault());
                    }

                    return keypadDesign;
                }
                throw new NotFoundException(ErrorMessages.KeypadNotFound.ToString(CultureInfo.CurrentCulture));

            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        public async Task<KeypadDesignResponseModel> CopyKeypadAsync(KeypadRequestModel copyModel, int userId)
        {
            try
            {
                if (copyModel?.KeypadCopyId > 0)
                {
                    //  var design = await GetKeypadDesign((int)copyModel?.KeypadCopyId).ConfigureAwait(false);
                    var keyRepository = _unitOfWork.GetRepository<Keypad>();
                    var design = await keyRepository.GetAll().Include(c => c.KeypadLevel).ThenInclude(x => x.KeypadButton).Where(x => !x.IsDeleted && x.Id == copyModel.KeypadCopyId).FirstOrDefaultAsyncSafe().ConfigureAwait(false);


                    if (design == null)
                    { throw new BadRequestException(ErrorMessages.InvalidKeypad.ToString(CultureInfo.CurrentCulture)); }


                    var newKeypad = design;
                    newKeypad.Id = 0;
                    newKeypad.Code = copyModel.Code;
                    newKeypad.Desc = copyModel.Desc;
                    newKeypad.OutletId = copyModel.OutletId;
                    newKeypad.UpdatedAt = DateTime.UtcNow;
                    newKeypad.UpdatedById = userId;
                    newKeypad.CreatedAt = DateTime.UtcNow;
                    newKeypad.CreatedById = userId;
                    newKeypad.KeypadLevel = new List<KeypadLevel>();
                    newKeypad.KeyPadButtonJSONData = null;
                    foreach (var oldlevel in design.KeypadLevel)
                    {
                        var level = new KeypadLevel();

                        // level.Id = 0;
                        level.CreatedAt = DateTime.UtcNow;
                        level.UpdatedAt = DateTime.UtcNow;
                        level.CreatedById = userId;
                        level.UpdatedById = userId;
                        level.Desc = oldlevel.Desc;
                        level.IsDeleted = false;
                        level.KeypadId = newKeypad.Id;
                        level.KeypadButton = new List<KeypadButton>();

                        foreach (var button in oldlevel.KeypadButton)
                        {
                            var newButton = new KeypadButton();

                            button.Id = 0;
                            button.LevelId = 0;

                            newButton = button;

                            level.KeypadButton.Add(newButton);
                        }
                        newKeypad.KeypadLevel.Add(level);
                    }
                    var result = await _repository.InsertAsync(newKeypad).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        var response = await GetKeypadDesign(result.Id).ConfigureAwait(false);
                        return response;
                    }

                    throw new BadRequestException(ErrorMessages.InvalidKeypad.ToString(CultureInfo.CurrentCulture));

                }
                else
                {
                    throw new BadRequestException(ErrorMessages.InvalidKeypad.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
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
        /// Add new keypad with its design
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<KeypadDesignResponseModel> AddKeypadDesignWithIndex(KeypadDesignRequestModel requestModel, int userId)
        {

            try
            {
                int keypadId = 0;
                var keypadModel = MappingHelpers.Mapping<KeypadDesignRequestModel, KeypadRequestModel>(requestModel);
                if (keypadModel != null)
                {
                    var keypad = await Insert(keypadModel, userId).ConfigureAwait(false);
                    if (keypad != null && keypad.Id > 0)
                    {
                        keypadId = keypad.Id;
                    }
                }
                KeypadDesignResponseModel responseModel = new KeypadDesignResponseModel();

                if (requestModel?.KeypadLevel != null)
                {

                    //Inserting new levels and buttons and Update Existing Levels and Buttons.
                    var levelList = requestModel.KeypadLevel.ToList();
                    foreach (var item in levelList)
                    {
                        int levelId = 0;

                        await AddUpdateKeypadLevelAsync(keypadId, item.LevelIndex, item.LevelDesc, userId).ConfigureAwait(false);

                        if (item.KeypadButtons != null)
                        {
                            var masterRepo = _unitOfWork.GetRepository<MasterListItems>();

                            //Adding and Updating Buttons.
                            foreach (var buttons in item.KeypadButtons)
                            {
                                //to be used for button type level only
                                var btnlevelId = 0;

                                var buttonName = await masterRepo.GetAll().Where(x => x.Id == buttons.Type && !x.IsDeleted).Select(x => x.Code).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                                if (string.IsNullOrEmpty(buttonName))
                                {
                                    throw new BadRequestException(ErrorMessages.KeypadButtonTypeRequired.ToString(CultureInfo.CurrentCulture));
                                }

                                //Check button type
                                switch (buttonName.ToLower())
                                {
                                    case "product":
                                        {
                                            if (buttons.ProductId == null)
                                                throw new BadRequestException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));

                                            else
                                            {
                                                var prodRepo = _unitOfWork.GetRepository<Product>();
                                                if (!await prodRepo.GetAll(x => x.Id == buttons.ProductId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                                                {
                                                    throw new BadRequestException(ErrorMessages.InvalidProductId.ToString(CultureInfo.CurrentCulture));
                                                }

                                            }
                                        }
                                        break;
                                    case "category":
                                        {
                                            if (buttons.CategoryId == null)
                                                throw new BadRequestException(ErrorMessages.CategoryIdReq.ToString(CultureInfo.CurrentCulture));

                                            else
                                            {
                                                var catRepo = _unitOfWork.GetRepository<MasterListItems>();
                                                if (!await catRepo.GetAll().Include(x => x.MasterList).Where(x => x.Id == buttons.CategoryId && !x.IsDeleted && x.MasterList.Code.ToLower() == "category").AnyAsyncSafe().ConfigureAwait(false))
                                                {
                                                    throw new BadRequestException(ErrorMessages.InvalidProductId.ToString(CultureInfo.CurrentCulture));
                                                }

                                            }
                                        }
                                        break;
                                    case "level":
                                        {
                                            if (buttons.ButtonLevelIndex == null)
                                                throw new BadRequestException(ErrorMessages.KeypadButtonLevelIdRequired.ToString(CultureInfo.CurrentCulture));

                                            else
                                            {
                                                var levRepo = _unitOfWork.GetRepository<KeypadLevel>();

                                                btnlevelId = await levRepo.GetAll().Where(x => x.LevelIndex == buttons.ButtonLevelIndex && x.KeypadId == keypadId && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                                                if (btnlevelId == 0)
                                                {
                                                    //create new level
                                                    await AddUpdateKeypadLevelAsync(keypadId, (int)buttons.ButtonLevelIndex, null, userId).ConfigureAwait(false);
                                                }
                                            }
                                        }
                                        break;

                                    case "pricelevel":
                                        {
                                            if (buttons.PriceLevel == null)
                                                throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));
                                        }
                                        break;
                                    case "pricelevelchange":
                                        {
                                            if (buttons.PriceLevel == null)
                                                throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));
                                        }
                                        break;
                                    case "pricelevelnext":
                                        {
                                            if (buttons.PriceLevel == null)
                                                throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));
                                        }
                                        break;
                                    case "saledisc":
                                        {
                                            if (buttons.SalesDiscountPerc == null)
                                            {
                                                throw new BadRequestException(ErrorMessages.SalesDiscountPercReq.ToString(CultureInfo.CurrentCulture));
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                //Add-Update Button
                                await AddUpdateKeypadButtonAsync(keypadId, levelId, userId, btnlevelId, buttons).ConfigureAwait(false);


                            }
                        }
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    }
                }

                responseModel = await GetKeypadDesign(keypadId).ConfigureAwait(false);

                return responseModel;
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<KeypadDesignResponseModel> UpdateKeypadDesignWithIndex(KeypadDesignRequestModel requestModel, int keypadId, int userId)
        {

            try
            {
                var keypadModel = MappingHelpers.Mapping<KeypadDesignRequestModel, KeypadRequestModel>(requestModel);
                if (keypadModel != null)
                {
                    await Update(keypadId, keypadModel, userId).ConfigureAwait(false);
                }
                KeypadDesignResponseModel responseModel = new KeypadDesignResponseModel();

                if (requestModel?.KeypadLevel != null)
                {

                    //Inserting new levels and buttons and Update Existing Levels and Buttons.
                    var levelList = requestModel.KeypadLevel.ToList();
                    foreach (var item in levelList)
                    {
                        if (item != null)
                        {
                            int levelId = 0;

                            levelId = await AddUpdateKeypadLevelAsync(keypadId, item.LevelIndex, item.LevelDesc, userId).ConfigureAwait(false);

                            if (item.KeypadButtons != null)
                            {
                                var masterRepo = _unitOfWork.GetRepository<MasterListItems>();

                                //Adding and Updating Buttons.
                                foreach (var buttons in item.KeypadButtons)
                                {
                                    //to be used for button type level only
                                    var btnlevelId = 0;

                                    var buttonName = await masterRepo.GetAll().Where(x => x.Id == buttons.Type && !x.IsDeleted).Select(x => x.Code).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                                    if (string.IsNullOrEmpty(buttonName))
                                    {
                                        throw new BadRequestException(ErrorMessages.KeypadButtonTypeRequired.ToString(CultureInfo.CurrentCulture));
                                    }

                                    //Check button type
                                    switch (buttonName.ToLower())
                                    {
                                        case "product":
                                            {
                                                if (buttons.ProductId == null)
                                                    throw new BadRequestException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));

                                                else
                                                {
                                                    var prodRepo = _unitOfWork.GetRepository<Product>();
                                                    if (!await prodRepo.GetAll(x => x.Id == buttons.ProductId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                                                    {
                                                        throw new BadRequestException(ErrorMessages.InvalidProductId.ToString(CultureInfo.CurrentCulture));
                                                    }

                                                }
                                            }
                                            break;
                                        case "category":
                                            {
                                                if (buttons.CategoryId == null)
                                                    throw new BadRequestException(ErrorMessages.CategoryIdReq.ToString(CultureInfo.CurrentCulture));

                                                else
                                                {
                                                    var catRepo = _unitOfWork.GetRepository<MasterListItems>();
                                                    if (!await catRepo.GetAll().Include(x => x.MasterList).Where(x => x.Id == buttons.CategoryId && !x.IsDeleted && x.MasterList.Code.ToLower() == "category").AnyAsyncSafe().ConfigureAwait(false))
                                                    {
                                                        throw new BadRequestException(ErrorMessages.InvalidProductId.ToString(CultureInfo.CurrentCulture));
                                                    }

                                                }
                                            }
                                            break;
                                        case "level":
                                            {
                                                if (buttons.ButtonLevelIndex == null)
                                                    throw new BadRequestException(ErrorMessages.KeypadButtonLevelIdRequired.ToString(CultureInfo.CurrentCulture));

                                                else
                                                {
                                                    var levRepo = _unitOfWork.GetRepository<KeypadLevel>();

                                                    btnlevelId = await levRepo.GetAll().Where(x => x.LevelIndex == buttons.ButtonLevelIndex && x.KeypadId == keypadId && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                                                    if (btnlevelId == 0)
                                                    {
                                                        //create new level
                                                        btnlevelId = await AddUpdateKeypadLevelAsync(keypadId, (int)buttons.ButtonLevelIndex, null, userId).ConfigureAwait(false);
                                                    }
                                                }
                                            }
                                            break;

                                        case "pricelevel":
                                            {
                                                if (buttons.PriceLevel == null)
                                                    throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));
                                            }
                                            break;
                                        case "pricelevelchange":
                                            {
                                                if (buttons.PriceLevel == null)
                                                    throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));
                                            }
                                            break;
                                        case "pricelevelnext":
                                            {
                                                if (buttons.PriceLevel == null)
                                                    throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));
                                            }
                                            break;
                                        case "saledisc":
                                            {
                                                if (buttons.SalesDiscountPerc == null)
                                                {
                                                    throw new BadRequestException(ErrorMessages.SalesDiscountPercReq.ToString(CultureInfo.CurrentCulture));
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    //Add-Update Button
                                    await AddUpdateKeypadButtonAsync(keypadId, levelId, userId, btnlevelId, buttons).ConfigureAwait(false);


                                }
                            }
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        }
                    }
                }

                responseModel = await GetKeypadDesign(keypadId).ConfigureAwait(false);

                return responseModel;
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<KeypadDesignResponseModel> UpdateKeypadDesignUsingSP(KeypadDesignRequestModel requestModel, int keypadId, int userId)
        {

            try
            {
                var keypadModel = MappingHelpers.Mapping<KeypadDesignRequestModel, KeypadRequestModel>(requestModel);
                if (keypadModel != null)
                {
                    await Update(keypadId, keypadModel, userId).ConfigureAwait(false);
                }
                KeypadDesignResponseModel responseModel = new KeypadDesignResponseModel();

                var masterRepo = _unitOfWork.GetRepository<MasterList>();

                var buttonRepo = await masterRepo.GetAll().Include(c => c.Items).Where(x => x.Code == "KEYPADBUTTON_TYPE" && !x.IsDeleted).Select(x => x.Items).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                var keypadButtonsList = new List<KeypadButtonSPRequestModel>();

                if (requestModel?.KeypadLevel != null)
                {
                    //Inserting new levels and buttons and Update Existing Levels and Buttons.
                    var levelList = requestModel.KeypadLevel.ToList();
                    foreach (var item in levelList)
                    {
                        if (item != null)
                        {
                            var levelId = await AddUpdateKeypadLevelAsync(keypadId, item.LevelIndex, item.LevelDesc, userId).ConfigureAwait(false);

                            if (item.KeypadButtons != null)
                            {
                                if (item.KeypadButtons.Where(x => x.Type == buttonRepo.Where(x => x.Code.ToLower() == "product").Select(x => x.Id).FirstOrDefault() && x.ProductId == null).Any())
                                {
                                    throw new BadRequestException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));
                                }

                                if (item.KeypadButtons.Where(x => x.Type == buttonRepo.Where(x => x.Code.ToLower() == "category").Select(x => x.Id).FirstOrDefault() && x.CategoryId == null).Any())
                                {
                                    throw new BadRequestException(ErrorMessages.CategoryIdReq.ToString(CultureInfo.CurrentCulture));
                                }

                                if (item.KeypadButtons.Where(x => x.Type == buttonRepo.Where(x => x.Code.ToLower() == "pricelevelnext" || x.Code.ToLower() == "pricelevelchange" || x.Code.ToLower() == "pricelevel").Select(x => x.Id).FirstOrDefault() && x.PriceLevel == null).Any())
                                {
                                    throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));
                                }

                                if (item.KeypadButtons.Where(x => x.Type == buttonRepo.Where(x => x.Code.ToLower() == "saledisc").Select(x => x.Id).FirstOrDefault() && x.SalesDiscountPerc == null).Any())
                                {
                                    throw new BadRequestException(ErrorMessages.SalesDiscountPercReq.ToString(CultureInfo.CurrentCulture));
                                }

                                if (item.KeypadButtons.Where(x => x.Type == buttonRepo.Where(x => x.Code.ToLower() == "level").Select(x => x.Id).FirstOrDefault() && x.ButtonLevelIndex == null).Any())
                                {
                                    throw new BadRequestException(ErrorMessages.KeypadButtonLevelIdRequired.ToString(CultureInfo.CurrentCulture));
                                }

                                var btnToAdd = item.KeypadButtons.Select(MappingHelpers.Mapping<KeypadButtonRequestModel, KeypadButtonSPRequestModel>).ToList();

                                foreach (var btn in btnToAdd)
                                {
                                    btn.LevelIndex = item.LevelIndex;
                                    btn.Color = ChangeColorCode(btn.Color);
                                }


                                keypadButtonsList.AddRange(btnToAdd);
                            }
                        }
                    }
                    var buttonTable = MappingHelpers.ToDataTable(keypadButtonsList, true);
                    //Add-Update Button
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                                                    new SqlParameter{
                                                      Direction = ParameterDirection.Input,
                                                      ParameterName = "@KeypadBtnReq",
                                                      TypeName ="KeypadButtonRequestType",
                                                      Value = buttonTable,
                                                      SqlDbType = SqlDbType.Structured
                                                    },
                                                    new SqlParameter("@keypadId", keypadId),
                                                    new SqlParameter("@userId", userId)
                                    };

                    var dset = await masterRepo.ExecuteStoredProcedure(StoredProcedures.AddKeypadButtons, dbParams.ToArray()).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    responseModel = await GetKeypadDesign(keypadId).ConfigureAwait(false);

                    return responseModel;
                }

                return responseModel;
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<int> AddUpdateKeypadLevelAsync(int keypadId, int levelInd, string LevelDesc, int userId)
        {
            try
            {
                var lvlRepository = _unitOfWork.GetRepository<KeypadLevel>();
                var levelExists = await lvlRepository.GetAll(x => !x.IsDeleted && x.LevelIndex == levelInd && x.KeypadId == keypadId).FirstOrDefaultAsync().ConfigureAwait(false);

                var levelId = 0;

                if (levelExists != null)
                {
                    levelExists.UpdatedById = userId;
                    levelExists.Desc = LevelDesc == null ? $"Level {levelInd}" : LevelDesc;

                    lvlRepository.DetachLocal(_ => _.Id == levelExists.Id);
                    lvlRepository.Update(levelExists);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    levelId = levelExists.Id;
                }
                else
                {
                    var level = new KeypadLevel();
                    level.KeypadId = keypadId;
                    level.Desc = LevelDesc == null ? $"Level {levelInd}" : LevelDesc;
                    level.LevelIndex = levelInd;
                    level.CreatedById = userId;
                    level.UpdatedById = userId;
                    level.IsDeleted = false;
                    var levelResponse = await lvlRepository.InsertAsync(level).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (levelResponse != null)
                    {
                        levelId = levelResponse.Id;
                    }
                }

                return levelId;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

        }

        public async Task AddUpdateKeypadButtonAsync(int keypadId, int levelId, int userId, int? btnlevelId, KeypadButtonRequestModel buttons)
        {
            try
            {
                if (buttons != null)
                {
                    var btnRepository = _unitOfWork.GetRepository<KeypadButton>();


                    var button = _iAutoMapper.Mapping<KeypadButtonRequestModel, KeypadButton>(buttons);

                    if (btnlevelId != null && btnlevelId > 0)
                    {
                        button.BtnKeypadLevelId = btnlevelId;
                    }

                    var buttonExist = await btnRepository.GetAll(x => x.ButtonIndex == buttons.ButtonIndex && x.KeypadId == keypadId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (buttonExist != null)
                    {
                        button.Color = ChangeColorCode(buttons.Color);
                        button.LevelId = levelId;
                        button.CreatedById = buttonExist.CreatedById;
                        button.CreatedAt = buttonExist.CreatedAt;
                        button.UpdatedById = userId;
                        button.KeypadId = keypadId;
                        button.IsDeleted = false;
                        button.Id = buttonExist.Id;

                        btnRepository.DetachLocal(_ => _.Id == buttonExist.Id);
                        btnRepository.Update(button);
                    }
                    else
                    {
                        var newButton = new KeypadButton();

                        newButton = _iAutoMapper.Mapping<KeypadButtonRequestModel, KeypadButton>(buttons);

                        if (btnlevelId != null && btnlevelId > 0)
                        {
                            button.BtnKeypadLevelId = btnlevelId;
                        }
                        newButton.LevelId = levelId;
                        newButton.CreatedById = userId;
                        newButton.UpdatedById = userId;
                        newButton.KeypadId = keypadId;
                        newButton.IsDeleted = false;
                        newButton.ButtonIndex = buttons?.ButtonIndex;
                        newButton.Color = ChangeColorCode(buttons.Color);
                        await btnRepository.InsertAsync(newButton).ConfigureAwait(false);
                    }

                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

        public static string ChangeColorCode(string hexCode)
        {
            try
            {
                var Tcolor = "";

                if (!string.IsNullOrEmpty(hexCode) && hexCode.Contains('#'))
                {
                    Tcolor = hexCode.Replace("#", "$00").ToUpper();
                }
                if (!string.IsNullOrEmpty(hexCode) && hexCode.ElementAt(0) == '$')
                {
                    Tcolor = hexCode.ToUpper();
                }

                if (Tcolor.Length > 10)
                {
                    Tcolor = hexCode.Replace("$00", "$").ToUpper();
                }

                return Tcolor;

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }


        public static string LoadJson(string keypadCode)
        {
            string json = "";
            keypadCode = keypadCode + ".json";

            try
            {
                string TempFolderName = Path.Combine("Resources", "KeypadJson");
                TempFolderName = Path.Combine(TempFolderName, string.Join("\\", keypadCode));

                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName)))
                {
                    return json;
                }
                string path = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName);
                using (StreamReader str = new StreamReader(path))
                {
                    json = str.ReadToEnd();
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return json;
            }
            return json;
        }

        //public static string SaveJson(string keypadJson, string keypadCode)
        //{
        //    keypadCode = keypadCode + ".json";

        //    string TempFolderName = Path.Combine("Resources", "KeypadJson");

        //    //if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName)))
        //    //{
        //    //    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName));
        //    //}

        //    TempFolderName = Path.Combine(TempFolderName, string.Join("\\", keypadCode));

        //    string path = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName);
        //    //  dynamic json = JsonConvert.DeserializeObject<string>(keypadJson);


            
        //    File.WriteAllText(path, keypadJson);
        //    return TempFolderName;
        //}
    }

}

