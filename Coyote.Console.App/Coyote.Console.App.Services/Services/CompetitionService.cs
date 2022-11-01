using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class CompetitionService : ICompetitionService
    {
        IUnitOfWork _unitOfWork = null;
        IAutoMappingServices _iAutoMap = null;
        public CompetitionService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping)
        {
            _unitOfWork = unitOfWork;
            _iAutoMap = autoMapping;
        }

        /// <summary>
        /// Get List of competition 
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<CompetitionResponseViewModel>>> GetCompetitions(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<CompetitionDetail>();
                var list = repository.GetAll(x => !x.IsDeleted, null, includes: new Expression<Func<CompetitionDetail, object>>[] { c => c.CompetitionZone,
                    c => c.CompetitionType, c => c.CompetitionResetCycle, r=>r.RewardType, t=>t.TriggerType,p=>p.Promotion });
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower())
                        || x.Desc.Contains(inputModel.GlobalFilter.ToLower()) || x.Message.Contains(inputModel.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        list = list.Where(x => x.Status);

                    count = list.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    list = list.OrderByDescending(x => x.UpdatedAt);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderByDescending(x => x.UpdatedAt);
                                else
                                    list = list.OrderBy(x => x.UpdatedAt);
                                break;
                        }
                    }
                }
                List<CompetitionResponseViewModel> listVM;
                listVM = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                return new PagedOutputModel<List<CompetitionResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get List of competition 
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<CompetitionResponseViewModel>>> GetActiveCompetitions(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                var repository = _unitOfWork?.GetRepository<CompetitionDetail>();
                int count = 0;
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@Status", inputModel?.Status),
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@IsLogged", inputModel?.IsLogged),
                        new SqlParameter("@RoleId",RoleId)

                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveCompetitionDetail, dbParams.ToArray()).ConfigureAwait(false);
                List<CompetitionResponseViewModel> listVM = MappingHelpers.ConvertDataTable<CompetitionResponseViewModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["RecordCount"]);
                return new PagedOutputModel<List<CompetitionResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get Details for a Competition
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CompetitionResponseViewModel> GetCompetitionById(long id)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<CompetitionDetail>();
                    var header = await repository.GetAll(x => x.Id == id && !x.IsDeleted, include: x => x.Include(p => p.Promotion)
                    .ThenInclude(x => x.PromotionFrequency).Include(p => p.Promotion).ThenInclude(x => x.PromotionSource)
                    .Include(z => z.CompetitionZone).Include(t => t.CompetitionType).Include(r => r.CompetitionResetCycle).Include(tr => tr.TriggerType)
                    .Include(rt => rt.RewardType).Include(pc => pc.PromotionCompetitions).ThenInclude(x => x.Product)
                    .Include(x => x.PromotionCompetitions).ThenInclude(x => x.Rewards)
                    .Include(x => x.PromotionCompetitions).ThenInclude(x => x.Triggers).ThenInclude(x => x.TriggerProductGroup))
                        .FirstOrDefaultAsync().ConfigureAwait(false);

                    if (header == null)
                    {
                        throw new NotFoundException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    var compVM = MappingHelpers.CreateMap(header);
                    compVM.CompetitionTriggerResponse = (header.PromotionCompetitions.Where(x => x.Triggers != null && !x.IsDeleted).ToList()).Select(MappingHelpers.CreateMap).ToList();
                    compVM.CompetitionRewardResponse = (header.PromotionCompetitions.Where(x => x.Rewards != null && !x.IsDeleted).ToList()).Select(MappingHelpers.CreateMapRewards).ToList();



                    if (!string.IsNullOrEmpty(header.ImagePath))
                    {
                        Byte[] imageBytes;
                        string imageFolderPath = Directory.GetCurrentDirectory() + header.ImagePath;
                        if (File.Exists(imageFolderPath))
                        {
                            imageBytes = System.IO.File.ReadAllBytes(imageFolderPath);
                            compVM.Image = imageBytes;
                        }
                    }

                    return compVM;
                }
                throw new NullReferenceException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Insert a competition
        /// also insert a record in promotion 
        /// all products from trigger and reward are added to Promotion Competition
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CompetitionResponseViewModel> Insert(CompetitionRequestViewModel viewModel, int userId)
        {
            // insert into promo then into comp-detail then into promo-comp and then into trigger and rewards
            try
            {
                long resultId = 0;
                if (viewModel != null)
                {
                    //Check If Image is also added
                    if (viewModel.Image != null)
                    {
                        if (string.IsNullOrEmpty(viewModel.ImageName))
                        {
                            throw new BadRequestException(ErrorMessages.ImageNameError.ToString(CultureInfo.CurrentCulture));
                        }

                        var postedFileExtension = Path.GetExtension(viewModel.ImageName);
                        if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    #region Competition Data

                    if (viewModel.TriggerProds.Count == 0 || (viewModel.RewardProds.Count == 0 && (viewModel.Discount == null || viewModel.Discount == 0)))
                    {
                        throw new NullReferenceException(ErrorMessages.CompetitionDeatilsIncomplete.ToString(CultureInfo.CurrentCulture));
                    }
                    var promoRepo = _unitOfWork?.GetRepository<Promotion>();
                    var repository = _unitOfWork?.GetRepository<CompetitionDetail>();
                    var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                    var masterRepo = _unitOfWork?.GetRepository<MasterList>();
                    var prodRepo = _unitOfWork?.GetRepository<Product>();

                    #region Validate 
                    // is COMPETITIONTYPE
                    int? competitionType = (await masterRepo.GetAll(x => x.Code == "COMPETITIONTYPE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                    if (!competitionType.HasValue || !(await listItemRepo.GetAll(x => x.ListId == competitionType && x.Id == viewModel.TypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.CompetitionDetailCompetitionTypeNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    //Set promotion type to Competition

                    int? promoType = (await masterRepo.GetAll(x => x.Code == "PROMOTYPE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;

                    viewModel.PromotionTypeId = await listItemRepo.GetAll(x => x.ListId == promoType && x.Code.ToUpper().Contains("COMPITITION")).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                    // is Code unique
                    if ((await repository.GetAll(x => x.Code == viewModel.Code).AnyAsync().ConfigureAwait(false)) || (await promoRepo.GetAll(x => x.Code == viewModel.Code).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.CompetitionCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    //is Zone
                    int? zone = (await masterRepo.GetAll(x => x.Code == "Zone" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                    if (!zone.HasValue || !(await listItemRepo.GetAll(x => x.ListId == zone && x.Id == viewModel.ZoneId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.ZoneNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    //If zone is Inactive
                    if (!(await listItemRepo.GetAll(x => x.ListId == zone && x.Id == viewModel.ZoneId && x.Status).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.ZoneInactive.ToString(CultureInfo.CurrentCulture));
                    }
                    //is POINTSRESETCYCLE
                    int? resetCycle = (await masterRepo.GetAll(x => x.Code == "POINTSRESETCYCLE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                    if (!resetCycle.HasValue || !(await listItemRepo.GetAll(x => x.ListId == resetCycle && x.Id == viewModel.ResetCycleId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.ResetCycleNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    //is TRIGGERTYPE
                    int? triggerType = (await masterRepo.GetAll(x => x.Code == "TRIGGERTYPE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                    if (!triggerType.HasValue || !(await listItemRepo.GetAll(x => x.ListId == triggerType && x.Id == viewModel.TriggerTypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.TriggerTypeNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    //is REWARDTYPE
                    int? rewardType = (await masterRepo.GetAll(x => x.Code == "REWARDTYPE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                    if (!rewardType.HasValue || !(await listItemRepo.GetAll(x => x.ListId == rewardType && x.Id == viewModel.RewardTypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.RewardTypeNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    #endregion

                    //Add to promotion
                    var header = _iAutoMap.Mapping<CompetitionRequestViewModel, Promotion>(viewModel);
                    header.ImagePath = null;
                    header.IsDeleted = false;
                    header.CreatedById = userId;
                    header.UpdatedById = userId;
                    header.CreatedAt = DateTime.UtcNow;
                    header.UpdatedAt = DateTime.UtcNow;
                    var freq = await listItemRepo.GetAll(x => x.Code.ToLower() == "daily" && !x.IsDeleted).Include(c => c.MasterList).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                    if (freq.MasterList.Code == "PromotionFrequency")
                    {
                        header.FrequencyId = freq.Id;
                    }

                    int? promoSource = (await masterRepo.GetAll(x => x.Code == "PROMOSOURCE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                    var source = await listItemRepo.GetAll(x => x.Code.ToLower() == "manual" && x.ListId== promoSource && !x.IsDeleted).Include(c => c.MasterList).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    
                    if (source.MasterList.Code == "PROMOSOURCE")
                    {
                        header.SourceId = source.Id;
                    }


                    var type = await listItemRepo.GetAll(x => x.Code.ToLower() == "competition").Include(c => c.MasterList).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                    if (type.MasterList.Code == "PROMOTYPE")
                    {
                        header.PromotionTypeId = type.Id;
                    }
                    

                    //promo child is competition detail, add comp-details
                    var compDetails = _iAutoMap.Mapping<CompetitionRequestViewModel, CompetitionDetail>(viewModel);
                    compDetails.ImagePath = null;
                    compDetails.IsDeleted = false;
                    compDetails.CreatedById = userId;
                    compDetails.UpdatedById = userId;
                    compDetails.CreatedAt = DateTime.UtcNow;
                    compDetails.UpdatedAt = DateTime.UtcNow;

                    //Insert products into promo-comp  //all prods of triggers and  rewards(if any)
                    List<PromotionCompetition> promoCompList = new List<PromotionCompetition>();
                    #region Triggers
                    foreach (var prod in viewModel.TriggerProds)
                    {
                        //is TRIGGERPRODUCTGROUP
                        int? triggerProdGrpType = (await masterRepo.GetAll(x => x.Code == "TRIGGERPRODUCTGROUP" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                        if (!triggerProdGrpType.HasValue || !(await listItemRepo.GetAll(x => x.Id == prod.TriggerProductGroupID && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.TriggerProductGroupNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (!(await prodRepo.GetAll(x => x.Id == prod.ProductId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        var promoComp = _iAutoMap.Mapping<CompTriggerRequestViewModel, PromotionCompetition>(prod);
                        promoComp.Status = true;
                        promoComp.IsDeleted = false;
                        promoComp.CreatedById = userId;
                        promoComp.UpdatedById = userId;
                        promoComp.CreatedAt = DateTime.UtcNow;
                        promoComp.UpdatedAt = DateTime.UtcNow;
                        // add to trigger
                        var trigg = _iAutoMap.Mapping<CompTriggerRequestViewModel, CompetitionTrigger>(prod);
                        trigg.IsDeleted = false;
                        trigg.CreatedById = userId;
                        trigg.UpdatedById = userId;
                        trigg.CreatedAt = DateTime.UtcNow;
                        trigg.UpdatedAt = DateTime.UtcNow;

                        promoComp.Triggers = trigg;
                        //Add promo-comp to list to be added to comp-details
                        promoCompList.Add(promoComp);
                    }
                    #endregion

                    #region Rewards
                    foreach (var prod in viewModel.RewardProds)
                    {
                        //if complementory is product
                        if (prod.ProductId != null && prod.Count != null && prod.Count != 0)
                        {
                            if (!(await prodRepo.GetAll(x => x.Id == prod.ProductId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                            {
                                throw new BadRequestException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                        var promoComp = _iAutoMap.Mapping<CompRewardRequestViewModel, PromotionCompetition>(prod);
                        promoComp.Status = true;
                        promoComp.IsDeleted = false;
                        promoComp.CreatedById = userId;
                        promoComp.UpdatedById = userId;
                        promoComp.CreatedAt = DateTime.UtcNow;
                        promoComp.UpdatedAt = DateTime.UtcNow;
                        // add to Reward
                        var reward = _iAutoMap.Mapping<CompRewardRequestViewModel, CompetitionReward>(prod);
                        reward.IsDeleted = false;
                        reward.CreatedById = userId;
                        reward.UpdatedById = userId;
                        reward.CreatedAt = DateTime.UtcNow;
                        reward.UpdatedAt = DateTime.UtcNow;

                        promoComp.Rewards = reward;
                        //Add promo-comp to list to be added to comp-details
                        promoCompList.Add(promoComp);
                    }
                    #endregion

                    //Add promo-comp to comp-details
                    compDetails.PromotionCompetitions = promoCompList;

                    //add comp details to promo
                    header.CompetitionDetails = compDetails;

                    var result = await promoRepo.InsertAsync(header).ConfigureAwait(false);
                    #endregion

                    //save competition and promotion competition
                    if (await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false))
                    {
                        if (result != null)
                        {
                            //Fetch competition details Id
                            resultId = result.CompetitionDetails.Id;
                            return await GetCompetitionById(resultId).ConfigureAwait(false);
                        }
                    }
                }
                throw new NullReferenceException(ErrorMessages.NotAcceptedOrCreated.ToString(CultureInfo.CurrentCulture));

            }
            catch (Exception ex)
            {
                //Rollback 
                _unitOfWork.Dispose();
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Delete Competition, trigger, rewards and promotion and promotion competition
        /// </summary>
        /// <param name = "id" ></ param >
        /// < param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCompetition(long id, int userId)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<CompetitionDetail>();
                    var exists = await repository.GetAll(x => x.Id == id).Include(d => d.Promotion).Include(x => x.PromotionCompetitions).ThenInclude(x => x.Triggers)
                        .Include(x => x.PromotionCompetitions).ThenInclude(x => x.Rewards).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        //delete from comp-detail                       
                        exists.IsDeleted = true;
                        exists.UpdatedById = userId;                        
                        //delete from promo
                        exists.Promotion.IsDeleted = true;
                        exists.Promotion.UpdatedById = userId;
                        //delete from promo-comp and trigger and reward

                        foreach (var existingChild in exists.PromotionCompetitions.ToList())
                        {
                            //item is deleted
                            existingChild.UpdatedAt = DateTime.UtcNow;
                            existingChild.UpdatedById = userId;
                            existingChild.IsDeleted = true;
                            if (existingChild.Triggers != null)
                            {
                                //delete from trigger
                                existingChild.Triggers.UpdatedById = userId;
                                existingChild.Triggers.UpdatedAt = DateTime.UtcNow;
                                existingChild.Triggers.IsDeleted = true;
                            }
                            else if (existingChild.Rewards != null)
                            {
                                //delete from Reward
                                existingChild.Rewards.UpdatedById = userId;
                                existingChild.Rewards.UpdatedAt = DateTime.UtcNow;
                                existingChild.Rewards.IsDeleted = true;
                            }
                        }

                        repository?.Update(exists);
                        return await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                    }
                    throw new NullReferenceException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Delete an item from trigger, also delete from promotion competition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="triggerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCompetitionTriggerItem(long id, long triggerId, int userId)
        {
            try
            {
                if (id > 0 && triggerId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<CompetitionDetail>();
                    var exists = await repository.GetAll(x => x.Id == id).Include(d => d.Promotion).Include(x => x.PromotionCompetitions).ThenInclude(x => x.Triggers)
                         .Include(x => x.PromotionCompetitions).ThenInclude(x => x.Rewards).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        //Delete Trigger
                        var existingChild = exists.PromotionCompetitions.Where(x => x.Triggers != null && x.Triggers.Id == triggerId).FirstOrDefault();
                        if (existingChild != null)
                        {
                            //Delete from promo-comp
                            existingChild.UpdatedAt = DateTime.UtcNow;
                            existingChild.UpdatedById = userId;
                            existingChild.IsDeleted = true;
                            //delete from trigger
                            existingChild.Triggers.UpdatedById = userId;
                            existingChild.Triggers.UpdatedAt = DateTime.UtcNow;
                            existingChild.Triggers.IsDeleted = true;
                        }

                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Delete an item from Reward, also delete from promotion competition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rewardId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCompetitionRewardItem(long id, long rewardId, int userId)
        {
            try
            {
                if (id > 0 && rewardId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<CompetitionDetail>();
                    var exists = await repository.GetAll(x => x.Id == id).Include(d => d.Promotion).Include(x => x.PromotionCompetitions).ThenInclude(x => x.Triggers)
                         .Include(x => x.PromotionCompetitions).ThenInclude(x => x.Rewards).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        //Delete Reward
                        var existingChild = exists.PromotionCompetitions.Where(x => x.Rewards != null && x.Rewards.Id == rewardId).FirstOrDefault();
                        if (existingChild != null)
                        {
                            //Delete from promo-comp
                            existingChild.UpdatedAt = DateTime.UtcNow;
                            existingChild.UpdatedById = userId;
                            existingChild.IsDeleted = true;
                            //delete from Rewards
                            existingChild.Rewards.UpdatedById = userId;
                            existingChild.Rewards.UpdatedAt = DateTime.UtcNow;
                            existingChild.Rewards.IsDeleted = true;
                        }

                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        /// <summary>
        /// Update Competition , trigger and reward
        /// also update promotion and promotion competition
        /// deletes items that are not in request but in DB
        /// updates items that are in request and DB
        /// inserts items that are in request but not in DB
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CompetitionResponseViewModel> Update(CompetitionRequestViewModel viewModel, long id, int userId, string imagePath = null)
        {
            try
            {
                if (viewModel != null)
                {
                    if (id == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.TriggerProds.Count == 0 || (viewModel.RewardProds.Count == 0 && (viewModel.Discount == null || viewModel.Discount == 0)))
                    {
                        throw new NullReferenceException(ErrorMessages.CompetitionDeatilsIncomplete.ToString(CultureInfo.CurrentCulture));
                    }

                    var promoRepo = _unitOfWork?.GetRepository<Promotion>();
                    var repository = _unitOfWork?.GetRepository<CompetitionDetail>();

                    //change
                    var exists = await repository.GetAll(x => x.Id == id).Include(d => d.Promotion).Include(x => x.PromotionCompetitions).ThenInclude(x => x.Triggers)
                        .Include(x => x.PromotionCompetitions).ThenInclude(x => x.Rewards).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists != null)
                    {
                        var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                        var masterRepo = _unitOfWork?.GetRepository<MasterList>();
                        #region Validate
                        // is COMPETITIONTYPE
                        int? competitionType = (await masterRepo.GetAll(x => x.Code == "COMPETITIONTYPE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                        if (!competitionType.HasValue || !(await listItemRepo.GetAll(x => x.ListId == competitionType && x.Id == viewModel.TypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.CompetitionDetailCompetitionTypeNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        // is Code unique
                        if ((await repository.GetAll(x => x.Code == viewModel.Code && x.Id != id).AnyAsync().ConfigureAwait(false)) ||
                            (await promoRepo.GetAll(x => x.Code == viewModel.Code && x.Id != exists.PromotionId).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.CompetitionCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                        //is Zone
                        int? zone = (await masterRepo.GetAll(x => x.Code == "Zone" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                        if (!zone.HasValue || !(await listItemRepo.GetAll(x => x.ListId == zone && x.Id == viewModel.ZoneId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.ZoneNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        //is POINTSRESETCYCLE
                        int? resetCycle = (await masterRepo.GetAll(x => x.Code == "POINTSRESETCYCLE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                        if (!resetCycle.HasValue || !(await listItemRepo.GetAll(x => x.ListId == resetCycle && x.Id == viewModel.ResetCycleId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.ResetCycleNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        //is TRIGGERTYPE
                        int? triggerType = (await masterRepo.GetAll(x => x.Code == "TRIGGERTYPE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                        if (!triggerType.HasValue || !(await listItemRepo.GetAll(x => x.ListId == triggerType && x.Id == viewModel.TriggerTypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.TriggerTypeNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        //is REWARDTYPE
                        int? rewardType = (await masterRepo.GetAll(x => x.Code == "REWARDTYPE" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                        if (!rewardType.HasValue || !(await listItemRepo.GetAll(x => x.ListId == rewardType && x.Id == viewModel.RewardTypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.RewardTypeNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        #endregion 

                        //update promotion
                        var header = _iAutoMap.Mapping<CompetitionRequestViewModel, Promotion>(viewModel);
                        header.ImagePath = imagePath;
                        header.IsDeleted = false;
                        header.CreatedById = exists.Promotion.CreatedById;
                        header.UpdatedById = userId;
                        header.CreatedAt = exists.Promotion.CreatedAt;
                        header.UpdatedAt = DateTime.UtcNow;
                        header.Id = exists.Promotion.Id;
                        header.FrequencyId = exists.Promotion.FrequencyId;
                        header.SourceId = exists.Promotion.SourceId;
                        header.PromotionTypeId = exists.Promotion.PromotionTypeId;

                        //promo child is competition detail, update comp-details
                        var compDetails = _iAutoMap.Mapping<CompetitionRequestViewModel, CompetitionDetail>(viewModel);
                        compDetails.ImagePath = imagePath;
                        compDetails.IsDeleted = false;
                        compDetails.CreatedById = exists.CreatedById;
                        compDetails.UpdatedById = userId;
                        compDetails.CreatedAt = exists.CreatedAt;
                        compDetails.UpdatedAt = DateTime.UtcNow;
                        compDetails.Code = header.Code;
                        compDetails.Id = exists.Id;

                        //delete items from promo-comp that are in Db and not in request //repeat same for trigger and reward
                        var promoCompRepo = _unitOfWork?.GetRepository<PromotionCompetition>();
                        var triggRepo = _unitOfWork?.GetRepository<CompetitionTrigger>();
                        var rewRepo = _unitOfWork?.GetRepository<CompetitionReward>();

                        #region Delete 
                        foreach (var prod in exists.PromotionCompetitions.ToList())
                        {
                            if (prod.Triggers != null)
                            {
                                //delete items to be deleted //trigger
                                var child = viewModel.TriggerProds.FirstOrDefault(c => c.ProductId == prod.ProductId);
                                if (child == null)
                                {
                                    //item is deleted  
                                    prod.UpdatedAt = DateTime.UtcNow;
                                    prod.UpdatedById = userId;
                                    prod.IsDeleted = true;
                                    prod.Triggers.UpdatedAt = DateTime.UtcNow;
                                    prod.Triggers.UpdatedById = userId;
                                    prod.Triggers.IsDeleted = true;
                                }
                            }
                            else if (prod.Rewards != null)
                            {
                                //delete items to be deleted //rewards
                                var child = viewModel.RewardProds.FirstOrDefault(c => c.ProductId == prod.ProductId);
                                if (child == null)
                                {
                                    //item is deleted  
                                    prod.UpdatedAt = DateTime.UtcNow;
                                    prod.UpdatedById = userId;
                                    prod.IsDeleted = true;
                                    prod.Rewards.UpdatedAt = DateTime.UtcNow;
                                    prod.Rewards.UpdatedById = userId;
                                    prod.Rewards.IsDeleted = true;
                                }
                            }
                            //delete 
                            promoCompRepo.Update(prod);
                        }
                        #endregion

                        #region Insert/update
                        var prodRepo = _unitOfWork?.GetRepository<Product>();
                        foreach (var prod in viewModel.TriggerProds)
                        {
                            //is TRIGGERPRODUCTGROUP
                            int? triggerProdGrpType = (await masterRepo.GetAll(x => x.Code == "TRIGGERPRODUCTGROUP" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                            if (!triggerProdGrpType.HasValue || !(await listItemRepo.GetAll(x => x.Id == prod.TriggerProductGroupID && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                            {
                                throw new NotFoundException(ErrorMessages.TriggerProductGroupNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            if (!(await prodRepo.GetAll(x => x.Id == prod.ProductId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                            {
                                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                            }

                            var existingTrigger = exists.PromotionCompetitions?.Where(c => c.ProductId == prod.ProductId && c.Triggers != null && !c.IsDeleted).SingleOrDefault();
                            if (existingTrigger != null)
                            {
                                // Update child promo comp
                                existingTrigger.UpdatedAt = DateTime.UtcNow;
                                existingTrigger.UpdatedById = userId;
                                existingTrigger.IsDeleted = false;
                                existingTrigger.Status = true;

                                promoCompRepo.DetachLocal(_ => _.Id == existingTrigger.Id);
                                promoCompRepo.Update(existingTrigger);

                                // Update child trigger
                                var updTriggerItem = _iAutoMap.Mapping<CompTriggerRequestViewModel, CompetitionTrigger>(prod);
                                updTriggerItem.CreatedAt = existingTrigger.Triggers.CreatedAt;
                                updTriggerItem.CreatedById = existingTrigger.Triggers.CreatedById;
                                updTriggerItem.UpdatedAt = DateTime.UtcNow;
                                updTriggerItem.UpdatedById = userId;
                                updTriggerItem.IsDeleted = false;
                                updTriggerItem.CompPromoId = existingTrigger.Triggers.CompPromoId;
                                updTriggerItem.Id = existingTrigger.Triggers.Id;

                                triggRepo.DetachLocal(_ => _.Id == existingTrigger.Triggers.Id);
                                triggRepo.Update(updTriggerItem);
                            }
                            else
                            {
                                // Insert child
                                //handling first item 
                                if (exists.PromotionCompetitions == null)
                                    exists.PromotionCompetitions = new List<PromotionCompetition>();
                                var promoComp = _iAutoMap.Mapping<CompTriggerRequestViewModel, PromotionCompetition>(prod);
                                promoComp.Status = true;
                                promoComp.IsDeleted = false;
                                promoComp.CreatedById = userId;
                                promoComp.UpdatedById = userId;
                                promoComp.CreatedAt = DateTime.UtcNow;
                                promoComp.UpdatedAt = DateTime.UtcNow;
                                // add to trigger
                                var trigg = _iAutoMap.Mapping<CompTriggerRequestViewModel, CompetitionTrigger>(prod);
                                trigg.IsDeleted = false;
                                trigg.CreatedById = userId;
                                trigg.UpdatedById = userId;
                                trigg.CreatedAt = DateTime.UtcNow;
                                trigg.UpdatedAt = DateTime.UtcNow;

                                promoComp.Triggers = trigg;
                                //add to compDetails nd not in exists
                                if (compDetails.PromotionCompetitions == null)
                                    compDetails.PromotionCompetitions = new List<PromotionCompetition>();

                                compDetails.PromotionCompetitions.Add(promoComp);
                            }

                        }
                        foreach (var prod in viewModel.RewardProds)
                        {
                            //if complementory is product
                            if (prod.ProductId != null && prod.Count != null && prod.Count != 0)
                            {
                                if (!(await prodRepo.GetAll(x => x.Id == prod.ProductId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                                {
                                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                                }
                            }


                            var existingReward = exists.PromotionCompetitions?.Where(c => c.ProductId == prod.ProductId && c.Rewards != null && !c.IsDeleted).SingleOrDefault();
                            if (existingReward != null)
                            {
                                // Update child promo comp
                                existingReward.UpdatedAt = DateTime.UtcNow;
                                existingReward.UpdatedById = userId;
                                existingReward.IsDeleted = false;
                                existingReward.Status = true;
                                promoCompRepo.DetachLocal(_ => _.Id == existingReward.Id);
                                promoCompRepo.Update(existingReward);

                                // Update child Reward
                                var updRewardItem = _iAutoMap.Mapping<CompRewardRequestViewModel, CompetitionReward>(prod);
                                updRewardItem.CreatedAt = existingReward.Rewards.CreatedAt;
                                updRewardItem.CreatedById = existingReward.Rewards.CreatedById;
                                updRewardItem.UpdatedAt = DateTime.UtcNow;
                                updRewardItem.UpdatedById = userId;
                                updRewardItem.IsDeleted = false;
                                updRewardItem.CompPromoId = existingReward.Rewards.CompPromoId;
                                updRewardItem.Id = existingReward.Rewards.Id;

                                rewRepo.DetachLocal(_ => _.Id == existingReward.Rewards.Id);
                                rewRepo.Update(updRewardItem);
                            }
                            else
                            {
                                // Insert child
                                //handling first item 
                                if (exists.PromotionCompetitions == null)
                                    exists.PromotionCompetitions = new List<PromotionCompetition>();

                                var promoComp = _iAutoMap.Mapping<CompRewardRequestViewModel, PromotionCompetition>(prod);
                                promoComp.Status = true;
                                promoComp.IsDeleted = false;
                                promoComp.CreatedById = userId;
                                promoComp.UpdatedById = userId;
                                promoComp.CreatedAt = DateTime.UtcNow;
                                promoComp.UpdatedAt = DateTime.UtcNow;
                                // add to Reward
                                var reward = _iAutoMap.Mapping<CompRewardRequestViewModel, CompetitionReward>(prod);
                                reward.IsDeleted = false;
                                reward.CreatedById = userId;
                                reward.UpdatedById = userId;
                                reward.CreatedAt = DateTime.UtcNow;
                                reward.UpdatedAt = DateTime.UtcNow;

                                promoComp.Rewards = reward;
                                //add to compDetails nd not in exists
                                if (compDetails.PromotionCompetitions == null)
                                    compDetails.PromotionCompetitions = new List<PromotionCompetition>();

                                compDetails.PromotionCompetitions.Add(promoComp);
                            }

                        }

                        #endregion


                        //add comp details to promo
                        header.CompetitionDetails = compDetails;
                        repository.DetachLocal(_ => _.Id == compDetails.Id);
                        repository.Update(compDetails);
                        promoRepo.DetachLocal(_ => _.Id == header.Id);
                        promoRepo.Update(header);
                        if (await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false))
                        {
                            return await GetCompetitionById(id).ConfigureAwait(false);
                        }
                        throw new NullReferenceException(ErrorMessages.NotAcceptedOrCreated.ToString(CultureInfo.CurrentCulture));
                    }
                    throw new NotFoundException(ErrorMessages.CompetitionNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


    }
}
