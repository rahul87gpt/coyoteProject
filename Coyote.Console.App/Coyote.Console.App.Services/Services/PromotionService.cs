using System;
using System.Collections.Generic;
using System.Data;
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
using Coyote.Console.ViewModels.ImportModels;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class PromotionService : IPromotionService
    {
        private IUnitOfWork _unitOfWork = null;
        private IAutoMappingServices _iAutoMap;

        public PromotionService(IUnitOfWork unitOfWork, IAutoMappingServices autoMappingServices)
        {
            _unitOfWork = unitOfWork;
            _iAutoMap = autoMappingServices;
        }
        public async Task<PagedOutputModel<List<PromotionResponseViewModel>>> GetAllActivePromotionHeaders(SecurityViewModel securityViewModel, PromotionFilter inputModel = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Promotion>();
                var list = repository.GetAll()
                    .Include(c => c.PromotionFrequency)
                    .Include(c => c.PromotionSource)
                    .Include(c => c.PromotionType)
                    .Include(c => c.PromotionZone).Where(x => !x.IsDeleted && !x.PromotionType.IsDeleted);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()));
                    if (inputModel.PromotionStartDate.HasValue)
                        list = list.Where(x => x.Start >= inputModel.PromotionStartDate.Value || x.End >= inputModel.PromotionStartDate.Value);
                    if (inputModel.PromotionEndDate.HasValue)
                        list = list.Where(x => x.End >= inputModel.PromotionEndDate.Value || x.End <= inputModel.PromotionEndDate.Value);


                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        list = list.Where(x => x.Status);

                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.ZoneIds != null && securityViewModel.ZoneIds.Count > 0)
                        list = list.Where(x => securityViewModel.ZoneIds.Contains(x.ZoneId));

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    count = list.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Code);
                                else
                                    list = list.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Desc);
                                else
                                    list = list.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.UpdatedAt);
                                else
                                    list = list.OrderByDescending(x => x.UpdatedAt);
                                break;
                        }
                    }
                }
                List<PromotionResponseViewModel> listVM = new List<PromotionResponseViewModel>();
                var promos = (await list.ToListAsyncSafe().ConfigureAwait(false));

                listVM = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                count = list.Count();
                return new PagedOutputModel<List<PromotionResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        public async Task<PagedOutputModel<List<PromotionResponseViewModel>>> GetActivePromotionHeaders(SecurityViewModel securityViewModel, PromotionFilter inputModel = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Promotion>();
                int count = 0;
                bool zoneIds = false;
                var AccessZones = String.Empty;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.ZoneIds != null && securityViewModel.ZoneIds.Count > 0)
                {
                    foreach (var zoneId in securityViewModel.ZoneIds)
                        AccessZones += zoneId + ",";
                    zoneIds = true;
                }
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@PromotionStartDate", inputModel?.PromotionStartDate),
                        new SqlParameter("@PromotionEndDate", inputModel?.PromotionEndDate),
                        new SqlParameter("@PromoType", inputModel?.Code),
                        new SqlParameter("@Status", inputModel?.Status),
                        new SqlParameter("@ZoneIds", (zoneIds == true)?AccessZones:null),
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@ExcludeBuying", inputModel?.ExcludePromoBuy),
                        new SqlParameter("@IsLogged", inputModel?.IsLogged),
                        new SqlParameter("@Module","PromotionR"),
                        new SqlParameter("@RoleId",RoleId)

                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActivePromotionBatches, dbParams.ToArray()).ConfigureAwait(false);
                List<PromotionResponseViewModel> listVM = MappingHelpers.ConvertDataTable<PromotionResponseViewModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["RecordCount"]);
                return new PagedOutputModel<List<PromotionResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        public async Task<PromotionResponseViewModel> GetPromotionById(int id)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Promotion>();
                    //var promotion = await repository.GetById(id).ConfigureAwait(false
                    var promotion = await repository.GetAll(x => x.Id == id && !x.IsDeleted, includes: new Expression<Func<Promotion, object>>[] { c => c.PromotionType, c => c.PromotionZone }).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (promotion == null)
                    {
                        throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var promoVM = _iAutoMap.Mapping<Promotion, PromotionResponseViewModel>(promotion);
                    // promoVM.Frequency = promotion.PromotionFrequency.Code;
                    promoVM.Source = Enum.GetName(typeof(PromotionSource), promotion.SourceId);
                    promoVM.PromotionType = promotion.PromotionType.Code;
                    promoVM.Zone = promotion.PromotionZone.Name;
                    return promoVM;
                }
                throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<bool> DeletePromotion(int id, int userId)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Promotion>();
                    var exists = await repository.GetById(id).ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        exists.IsDeleted = true;
                        exists.UpdatedById = userId;

                        // exists.Code = (exists.Code + "~" + exists.Id);

                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<PromotionResponseViewModel> Update(PromotionRequestModel viewModel, int id, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    if (id == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var repository = _unitOfWork?.GetRepository<Promotion>();
                    var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                    var exists = await repository.GetById(id).ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (!(await listItemRepo.GetAll(x => x.Id == viewModel.PromotionTypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.PromotionTypeNotFound.ToString(CultureInfo.CurrentCulture));
                        }

                        if (!(await listItemRepo.GetAll(x => x.Id == viewModel.ZoneId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.PromotionZoneNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        var promo = _iAutoMap.Mapping<PromotionRequestModel, Promotion>(viewModel);


                        if (exists.PromotionTypeId != viewModel.PromotionTypeId)
                        {
                            //Delete Previous Products and Promotion Details
                            await DeletePreviousPromotionsAsync(viewModel.PromotionTypeId, id, userId).ConfigureAwait(false);
                        }

                        if (!String.IsNullOrEmpty(viewModel.Source))
                        {
                            if (viewModel.Source.ToUpper() == Enum.GetName(typeof(PromotionSource), PromotionSource.HOST) || viewModel.Source.ToUpper() == Enum.GetName(typeof(PromotionSource), PromotionSource.MANUAL))
                            {
                                promo.SourceId = (int)Enum.Parse(typeof(PromotionSource), viewModel?.Source);
                            }
                            else
                            {
                                promo.SourceId = (int)PromotionSource.HOST;
                            }
                        }
                        else
                        {
                            promo.SourceId = exists.SourceId;
                        }
                        promo.Id = id;
                        promo.FrequencyId = exists.FrequencyId;
                        promo.SourceId = exists.SourceId;
                        promo.CreatedAt = exists.CreatedAt;
                        promo.CreatedById = exists.CreatedById;
                        promo.UpdatedAt = DateTime.UtcNow;
                        promo.UpdatedById = userId;
                        promo.IsDeleted = false;
                        repository.DetachLocal(_ => _.Id == promo.Id);
                        repository.Update(promo);
                        if (await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false))
                        {
                            return await GetPromotionById(id).ConfigureAwait(false);
                        }
                        throw new NullReferenceException();
                    }
                    throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<PromotionResponseViewModel> Insert(PromotionRequestModel viewModel, int userId)
        {
            int resultId = 0;
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<Promotion>();
                    var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();

                    if (!(await listItemRepo.GetAll(x => x.Id == viewModel.PromotionTypeId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new NotFoundException(ErrorMessages.PromotionTypeNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (!(await listItemRepo.GetAll(x => x.Id == viewModel.ZoneId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new NotFoundException(ErrorMessages.PromotionZoneNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if ((await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.PromotionCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var promo = _iAutoMap.Mapping<PromotionRequestModel, Promotion>(viewModel);

                    promo.IsDeleted = false;
                    promo.CreatedById = userId;
                    promo.UpdatedById = userId;
                    promo.CreatedAt = DateTime.UtcNow;
                    promo.UpdatedAt = DateTime.UtcNow;

                    var freq = await listItemRepo.GetAll(x => x.Code.ToLower() == "daily" && !x.IsDeleted).Include(c => c.MasterList).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                    if (freq.MasterList.Code == "PromotionFrequency")
                    {
                        promo.FrequencyId = freq.Id;
                    }


                    var source = await listItemRepo.GetAll().Include(c => c.MasterList).Where(x => x.Code.ToUpper() == "MANUAL" && !x.IsDeleted && x.MasterList.Code == "PROMOSOURCE").FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                    if (source.MasterList.Code == "PROMOSOURCE")
                    {
                        promo.SourceId = source.Id;
                    }
                    var result = await repository.InsertAsync(promo).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        resultId = result.Id;
                    }
                }
                return await GetPromotionById(resultId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
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
        /// Get promotion Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="promoType"></param>
        /// <returns></returns>
        public async Task<PromotionDetailResponseViewModel> GetPromotionDetailsById(int id)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Promotion>();
                    var promotion = await repository.GetAll(x => x.Id == id,
                        include: x => x
                        .Include(c => c.PromotionFrequency)
                        .Include(c => c.PromotionSource)
                        .Include(c => c.PromotionType)
                        .Include(c => c.PromotionZone)
                        .Include(d => d.CompetitionDetails).ThenInclude(c => c.PromotionCompetitions).ThenInclude(p => p.Product).ThenInclude(c => c.Department)
                     .Include(d => d.PromotionBuying).ThenInclude(c => c.Product).ThenInclude(c => c.Department)
                     .Include(d => d.PromotionBuying).ThenInclude(pb => pb.Supplier)
                     .Include(d => d.PromotionMemberOffer).ThenInclude(c => c.Product).ThenInclude(c => c.Department)
                     .Include(d => d.PromotionOffer).ThenInclude(po => po.PromotionOfferProduct).ThenInclude(p => p.Product).ThenInclude(c => c.Department)
                     .Include(d => d.PromotionMixmatch).ThenInclude(pm => pm.PromotionMixmatchProduct).ThenInclude(p => p.Product).ThenInclude(c => c.Department)
                     .Include(d => d.PromotionSelling).ThenInclude(p => p.Product).ThenInclude(c => c.Department)).FirstOrDefaultAsync().ConfigureAwait(false);
                    
                    
                    if (promotion == null)
                    {
                        throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var promoVM = _iAutoMap.Mapping<Promotion, PromotionResponseViewModel>(promotion);
                    promoVM.Frequency = promotion.PromotionFrequency.Code;
                    promoVM.Source = promotion.PromotionSource.Code;
                    promoVM.PromotionType = promotion.PromotionType.Code;
                    promoVM.Zone = promotion.PromotionZone.Code;
                    PromotionDetailResponseViewModel promoTypeVM = new PromotionDetailResponseViewModel
                    {
                        Promotion = promoVM,
                    };

                    promoTypeVM.Promotion.PromotionProduct = new List<PromotionProductRequestModel>();

                    switch (promoVM.PromotionType.ToUpper())
                    {
                        case "BUYING":
                            #region Promotion Buying Mapping
                            promoTypeVM.Promotion.PromotionProduct.AddRange(promotion.PromotionBuying.Where(x => !x.IsDeleted).Select(MappingHelpers.CreateMap).ToList());
                            #endregion
                            break;
                        case "OFFER":
                            #region Promotion Offer Mapping
                            promoTypeVM.PromotionOffer = (promotion.PromotionOffer.Where(x => !x.IsDeleted).Select(x => new PromotionOfferViewModel
                            {
                                CreatedAt = x.CreatedAt,
                                CreatedById = x.CreatedById,
                                Group = x.Group,
                                Group1Price = x.Group1Price,
                                Group1Qty = x.Group1Qty,
                                Group2Price = x.Group2Price,
                                Group2Qty = x.Group2Qty,
                                Group3Price = x.Group3Price,
                                Group3Qty = x.Group3Qty,
                                Id = x.Id,
                                TotalPrice = x.TotalPrice,
                                TotalQty = x.TotalQty,
                                UpdatedAt = x.UpdatedAt,
                                UpdatedById = x.UpdatedById,
                            }).FirstOrDefault());

                            var offerProd = promotion.PromotionOffer.Where(x => !x.IsDeleted).Select(x => x.PromotionOfferProduct?.Where(x => !x.IsDeleted)).FirstOrDefault()?.Select(MappingHelpers.CreateMap).ToList();

                            if (offerProd != null)
                                promoTypeVM.Promotion.PromotionProduct.AddRange(offerProd);

                            #endregion
                            break;
                        case "SELLING":
                            #region Promotion Selling Mapping
                            promoTypeVM.Promotion.PromotionProduct.AddRange(promotion.PromotionSelling.Where(x => !x.IsDeleted).Select(MappingHelpers.CreateMap).ToList());//_iAutoMap.Mapping<PromotionSelling, PromotionProductRequestModel>
                            #endregion
                            break;
                        case "MEMBEROFFER":
                            #region Promotion Member Offer Mapping

                            promoTypeVM.Promotion.PromotionProduct.AddRange(promotion.PromotionMemberOffer.Where(x => !x.IsDeleted).Select(MappingHelpers.CreateMap).ToList());

                            //promoTypeVM.PromotionMember.AddRange(promotion.PromotionMemberOffer.Select(_iAutoMap.Mapping<PromotionMemberOffer, PromotionMemberOfferViewModel>).ToList());
                            #endregion
                            break;
                        case "MIXMATCH":
                            #region Promotion MixMatch Mapping
                            promoTypeVM.PromotionMixmatch = (promotion.PromotionMixmatch.Where(x => !x.IsDeleted).Select(x => new PromotionMixmatchViewModel
                            {
                                Amt1 = x.Amt1,
                                Amt2 = x.Amt2,
                                CreatedAt = x.CreatedAt,
                                CreatedById = x.CreatedById,
                                CumulativeOffer = x.CumulativeOffer,
                                DiscPcnt1 = x.DiscPcnt1,
                                DiscPcnt2 = x.DiscPcnt2,
                                Id = x.Id,
                                PriceLevel1 = x.PriceLevel1,
                                PriceLevel2 = x.PriceLevel2,
                                PromotionId = x.PromotionId,
                                Qty1 = x.Qty1,
                                Qty2 = x.Qty2,
                                UpdatedAt = x.UpdatedAt,
                                UpdatedById = x.UpdatedById,
                                Group = x.Group
                            }).FirstOrDefault());

                            var prodList = (promotion?.PromotionMixmatch?.Where(x => !x.IsDeleted).Select(x => x.PromotionMixmatchProduct.Where(x => !x.IsDeleted)).FirstOrDefault())?.Select(MappingHelpers.CreateMap).ToList();

                            if (prodList != null)
                                promoTypeVM.Promotion.PromotionProduct.AddRange(prodList);

                            #endregion
                            break;
                        case "COMPETITION":
                            var prodListComp = (promotion.CompetitionDetails.PromotionCompetitions.Where(x => !x.IsDeleted).Select(MappingHelpers.CreateMap1).ToList());
                            if (prodListComp != null)
                                promoTypeVM.Promotion.PromotionProduct.AddRange(prodListComp.OrderBy(x=>x.ProductId).ToList());
                            break;
                        default:
                            break;
                    }

                    return promoTypeVM;
                }
                throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Add promotion Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> AddPromotionDetail(PromotionDetailRequestModel viewModel, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.Promotion == null)
                    {
                        throw new BadRequestException(ErrorMessages.PromotionRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel?.Promotion.PromotionProduct.Select(x => x.ProductId).Distinct().Count() != viewModel?.Promotion.PromotionProduct.Count)
                    {
                        //can't add same product twice
                        throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
                    }

                    var promotion = await Insert(viewModel?.Promotion, userId).ConfigureAwait(false);
                    if (promotion == null)
                    {
                        throw new BadRequestException(ErrorMessages.PromotionNotSaved.ToString(CultureInfo.CurrentCulture));
                    }

                    switch (promotion.PromotionType.ToUpper())
                    {
                        case "BUYING":
                            #region Promotion Buying Mapping Add New 
                            await AddNewBuyingPromotion(promotion.Id, viewModel.Promotion.PromotionProduct, userId).ConfigureAwait(false);
                            #endregion
                            break;
                        case "OFFER":

                            #region Save offer settings and Offer Products

                            //If Promotion Offer is null
                            if (viewModel?.PromotionOffer == null)
                            {
                                throw new NullReferenceException(ErrorMessages.OfferRequired.ToString(CultureInfo.CurrentCulture));
                            }
                            #region Promotion Offer Mapping

                            var productRepo = _unitOfWork?.GetRepository<Product>();
                            var offRepo = _unitOfWork?.GetRepository<PromotionOffer>();
                            var offProdRepo = _unitOfWork?.GetRepository<PromotionOfferProduct>();
                            var offer = _iAutoMap.Mapping<PromotionOfferRequestModel, PromotionOffer>(viewModel.PromotionOffer);
                            offer.PromotionId = promotion.Id;
                            offer.IsDeleted = false;
                            offer.CreatedById = userId;
                            offer.UpdatedById = userId;
                            var resultOff = await offRepo.InsertAsync(offer).ConfigureAwait(false);
                            // TODO: Need to remove save changes from here as this is against unit of work
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            if (resultOff.Id > 0)
                            {
                                await AddNewPromotionOfferProducts(viewModel.Promotion?.PromotionProduct, offer.Id, userId).ConfigureAwait(false);
                            }
                            else
                            {
                                throw new Exception(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            #endregion
                            #endregion

                            break;
                        case "SELLING":
                            #region Promotion Selling Mapping 
                            await AddNewSellingPromotion(promotion.Id, viewModel.Promotion?.PromotionProduct, userId).ConfigureAwait(false);
                            #endregion
                            break;
                        case "MEMBEROFFER":
                            #region Promotion Member Offer Mapping
                            await AddNewMemberOffer(promotion.Id, viewModel.Promotion?.PromotionProduct, userId).ConfigureAwait(false);
                            #endregion
                            break;
                        case "MIXMATCH":

                            //If Promotion Mixmatch is null
                            if (viewModel?.PromotionMixmatch == null)
                            {
                                throw new NullReferenceException(ErrorMessages.MixMatchRequired.ToString(CultureInfo.CurrentCulture));
                            }

                            #region Promotion MixMatch Mapping
                            var mixRepo = _unitOfWork?.GetRepository<PromotionMixmatch>();
                            var mix = _iAutoMap.Mapping<PromotionMixmatchRequestModel, PromotionMixmatch>(viewModel.PromotionMixmatch);
                            mix.PromotionId = promotion.Id;
                            mix.IsDeleted = false;
                            mix.CreatedById = userId;
                            mix.UpdatedById = userId;
                            mix.Status = true;
                            var resultMix = await mixRepo.InsertAsync(mix).ConfigureAwait(false);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                            if (resultMix.Id > 0)
                            {
                                await AddNewPromotionMixMatchProduct(viewModel.Promotion?.PromotionProduct, mix.Id, userId).ConfigureAwait(false);
                            }
                            else
                            {
                                throw new Exception(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            #endregion
                            break;
                        default:
                            throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                            // break;
                    }
                    //Commit changes of list
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    return promotion.Id;
                }
                throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                //Rollback 
              //  _unitOfWork.Dispose();
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
        /// Update Promotion Detail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePromotionDetails(int promotionId, PromotionDetailRequestModel viewModel, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    if (promotionId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel?.Promotion != null)
                    {
                        if (viewModel?.Promotion.PromotionProduct.Select(x => x.ProductId).Distinct().Count() != viewModel?.Promotion.PromotionProduct.Count)
                        {
                            //can't add same product twice
                            throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
                        }

                        //Update Promotion Header and If type changed remove other promotions
                        await Update(viewModel?.Promotion, promotionId, userId).ConfigureAwait(false);
                    }
                    var repository = _unitOfWork?.GetRepository<Promotion>();
                    var promotion = await repository.GetAll(x => x.Id == promotionId, includes: new Expression<Func<Promotion, object>>[] { c => c.PromotionType }).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (promotion == null)
                    {
                        throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                    }


                    //Update Type of Promotion
                    var productRepo = _unitOfWork?.GetRepository<Product>();
                    switch (promotion.PromotionType.Code.ToUpper())
                    {
                        case "BUYING":
                            #region Promotion Buying Mapping Add New 
                            var buyRepo = _unitOfWork?.GetRepository<PromotionBuying>();
                            var supplierRepo = _unitOfWork?.GetRepository<Supplier>();
                            #region Delete Products
                            // Delete children
                            var buyExistingProds = await buyRepo.GetAll(x => x.PromotionId == promotionId
                            && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                            if (viewModel.Promotion?.PromotionProduct?.Count >= 0)
                            {
                                foreach (var existingProd in buyExistingProds.ToList())
                                {
                                    var prod = viewModel.Promotion?.PromotionProduct.FirstOrDefault(c => c.ProductId == existingProd.ProductId && c.PromotionId == existingProd.PromotionId);
                                    if (prod == null)
                                    {
                                        //item is deleted
                                        existingProd.UpdatedAt = DateTime.UtcNow;
                                        existingProd.UpdatedById = userId;
                                        existingProd.IsDeleted = true;
                                        buyRepo.Update(existingProd);
                                    }
                                }
                                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            }
                            #endregion

                            foreach (var item in viewModel.Promotion?.PromotionProduct)
                            {
                                //update ////if exists then update else add new product for promotion
                                var buyItemExists = await buyRepo.GetAll(x => x.Id == item.Id && x.PromotionId == promotionId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                                if (!productRepo.GetAll(x => x.Id == item.ProductId && !x.IsDeleted).Any())
                                {
                                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                                }
                                if (item.SupplierId == 0)
                                {
                                    item.SupplierId = null;
                                }
                                if (item.SupplierId != null)
                                {
                                    if (!supplierRepo.GetAll(x => x.Id == item.SupplierId && !x.IsDeleted).Any())
                                    {
                                        throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                                    }
                                }
                                if (buyRepo.GetAll(x => x.PromotionId == promotionId && x.ProductId == item.ProductId && !x.IsDeleted && buyItemExists != null).Any())
                                {
                                    //product exist in promotion detail, update details 
                                    var comm = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionBuying>(item);
                                    comm.Id = buyItemExists.Id;
                                    comm.PromotionId = promotionId;
                                    comm.CreatedAt = buyItemExists.CreatedAt;
                                    comm.CreatedById = buyItemExists.CreatedById;
                                    comm.UpdatedAt = DateTime.UtcNow;
                                    comm.UpdatedById = userId;
                                    comm.IsDeleted = false;
                                    //Detaching tracked entry - exists
                                    buyRepo.DetachLocal(_ => _.Id == comm.Id);
                                    buyRepo.Update(comm);
                                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                }
                                else
                                {
                                    //add new product in promotion 
                                    //do check for duplicate entry
                                    await AddNewBuyingPromotion(promotionId, new List<PromotionProductRequestModel>() { item }, userId).ConfigureAwait(false);
                                }
                            }
                            #endregion
                            break;
                        case "OFFER":
                            #region Offer Promotion Update
                            //If Promotion Offer is null
                            if (viewModel?.PromotionOffer == null)
                            {

                                throw new NullReferenceException(ErrorMessages.OfferRequired.ToString(CultureInfo.CurrentCulture));
                            }
                            #region Promotion Offer Mapping
                            var offRepo = _unitOfWork?.GetRepository<PromotionOffer>();
                            var offProdRepo = _unitOfWork?.GetRepository<PromotionOfferProduct>();

                            int? offerId = 0;

                            offerId = viewModel.PromotionOffer?.Id;

                            var offerItemExists = await offRepo.GetAll(x => x.Id == offerId && x.PromotionId == promotionId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);

                            var offer = _iAutoMap.Mapping<PromotionOfferRequestModel, PromotionOffer>(viewModel.PromotionOffer);

                            offer.PromotionId = promotionId;
                            offer.UpdatedAt = DateTime.UtcNow;
                            offer.UpdatedById = userId;
                            offer.IsDeleted = false;

                            if (offerItemExists == null)
                            {
                                offer.Id = 0;
                                offer.CreatedById = userId;
                                var resultOff = await offRepo.InsertAsync(offer).ConfigureAwait(false);
                                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                if (resultOff != null)
                                {
                                    offerId = resultOff.Id;
                                }
                            }
                            else
                            {
                                offer.Id = offerItemExists.Id;
                                offer.CreatedAt = offerItemExists.CreatedAt;
                                offer.CreatedById = offerItemExists.CreatedById;
                                //Detaching tracked entry - exists
                                offRepo.DetachLocal(_ => _.Id == offer.Id);
                                offRepo.Update(offer);
                            }

                            #region Delete Products
                            // Delete children
                            var offerExistingProds = await offProdRepo.GetAll(x => x.PromotionOfferId == offerId
                            && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                            if (viewModel.Promotion?.PromotionProduct?.Count >= 0)
                            {
                                foreach (var existingProd in offerExistingProds.ToList())
                                {
                                    //Need Updating
                                    var prod = viewModel.Promotion?.PromotionProduct.FirstOrDefault(c => c.ProductId == existingProd.ProductId);

                                    if (prod == null && (viewModel.PromotionOffer?.Id == existingProd.PromotionOfferId))
                                    {
                                        //item is deleted
                                        existingProd.UpdatedAt = DateTime.UtcNow;
                                        existingProd.UpdatedById = userId;
                                        existingProd.IsDeleted = true;
                                        offProdRepo.Update(existingProd);
                                    }
                                }
                            }
                            #endregion


                            foreach (var itemOff in viewModel.Promotion?.PromotionProduct)
                            {
                                if (!productRepo.GetAll(x => x.Id == itemOff.ProductId && !x.IsDeleted).Any())
                                {
                                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                                }
                                var offerProdExists = offProdRepo.GetAll(x => x.PromotionOfferId == offerId && x.ProductId == itemOff.ProductId).FirstOrDefault();
                                if (offerProdExists != null)
                                {
                                    if (itemOff.OfferGroup == null || itemOff.OfferGroup == 0)
                                    {
                                        itemOff.OfferGroup = offerProdExists.OfferGroup;
                                    }
                                    //product exist in promotion detail, update details 
                                    var comm = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionOfferProduct>(itemOff);
                                    comm.PromotionOfferId = offerProdExists.PromotionOfferId;
                                    comm.Id = offerProdExists.Id;
                                    comm.CreatedAt = offerProdExists.CreatedAt;
                                    comm.CreatedById = offerProdExists.CreatedById;
                                    comm.UpdatedAt = DateTime.UtcNow;
                                    comm.UpdatedById = userId;
                                    comm.IsDeleted = false;
                                    //Detaching tracked entry - exists
                                    offProdRepo.DetachLocal(_ => _.Id == comm.Id);
                                    offProdRepo.Update(comm);
                                }
                                else
                                {
                                    await AddNewPromotionOfferProducts(new List<PromotionProductRequestModel>() { itemOff }, (int)offerId, userId).ConfigureAwait(false);
                                }
                            }
                            #endregion
                            #endregion
                            break;
                        case "SELLING":
                            #region Promotion Selling Mapping
                            var sellRepo = _unitOfWork?.GetRepository<PromotionSelling>();

                            #region Delete Products
                            // Delete children
                            var sellExistingProds = await sellRepo.GetAll(x => x.PromotionId == promotionId
                            && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                            if (viewModel.Promotion?.PromotionProduct?.Count >= 0)
                            {
                                foreach (var existingProd in sellExistingProds.ToList())
                                {
                                    var prod = viewModel.Promotion?.PromotionProduct.FirstOrDefault(c => c.ProductId == existingProd.ProductId && c.PromotionId == existingProd.PromotionId);
                                    if (prod == null)
                                    {
                                        //item is deleted
                                        existingProd.UpdatedAt = DateTime.UtcNow;
                                        existingProd.UpdatedById = userId;
                                        existingProd.IsDeleted = true;
                                        sellRepo.Update(existingProd);
                                    }
                                }
                            }
                            #endregion

                            foreach (var itemSell in viewModel.Promotion?.PromotionProduct)
                            {

                                //update 
                                var sellItemExists = await sellRepo.GetAll(x => (x.Id == itemSell.Id || x.ProductId == itemSell.ProductId) && x.PromotionId == promotionId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                                if (!productRepo.GetAll(x => x.Id == itemSell.ProductId && !x.IsDeleted).Any())
                                {
                                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                                }
                                if (sellRepo.GetAll(x => x.PromotionId == promotionId && x.ProductId == itemSell.ProductId && sellItemExists != null).Any())
                                {
                                    //If AmtOffNorm has value, prices sholud be null
                                    if (itemSell.AmtOffNorm1 != null && itemSell.AmtOffNorm1 > 0)
                                    {
                                        itemSell.Price = null;
                                        itemSell.Price1 = null;
                                        itemSell.Price2 = null;
                                        itemSell.Price3 = null;
                                        itemSell.Price4 = null;
                                    }

                                    //product exist in promotion detail, update details 
                                    var comm = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionSelling>(itemSell);


                                    comm.PromotionId = promotionId;
                                    comm.Id = sellItemExists.Id;
                                    comm.CreatedAt = sellItemExists.CreatedAt;
                                    comm.CreatedById = sellItemExists.CreatedById;
                                    comm.UpdatedAt = DateTime.UtcNow;
                                    comm.UpdatedById = userId;
                                    comm.IsDeleted = false;
                                    //Detaching tracked entry - exists
                                    sellRepo.DetachLocal(_ => _.Id == comm.Id);
                                    sellRepo.Update(comm);
                                }
                                else
                                {
                                    await AddNewSellingPromotion(promotionId, new List<PromotionProductRequestModel>() { itemSell }, userId).ConfigureAwait(false);
                                }
                            }
                            #endregion
                            break;
                        case "MEMBEROFFER":
                            #region Promotion Member Offer Mapping
                            var memRepo = _unitOfWork?.GetRepository<PromotionMemberOffer>();

                            #region Delete Products
                            // Delete children
                            var membExistingProds = await memRepo.GetAll(x => x.PromotionId == promotionId
                            && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                            if (viewModel.Promotion?.PromotionProduct?.Count >= 0)
                            {
                                foreach (var existingProd in membExistingProds.ToList())
                                {
                                    var prod = viewModel.Promotion?.PromotionProduct.FirstOrDefault(c => c.ProductId == existingProd.ProductId && c.PromotionId == existingProd.PromotionId);
                                    if (prod == null)
                                    {
                                        //item is deleted
                                        existingProd.UpdatedAt = DateTime.UtcNow;
                                        existingProd.UpdatedById = userId;
                                        existingProd.IsDeleted = true;
                                        memRepo.Update(existingProd);
                                    }
                                }
                            }
                            #endregion

                            foreach (var itemMem in viewModel.Promotion?.PromotionProduct)
                            {
                                //update 
                                var memItemExists = await memRepo.GetAll(x => x.Id == itemMem.Id && x.PromotionId == promotionId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                                if (!productRepo.GetAll(x => x.Id == itemMem.ProductId && !x.IsDeleted).Any())
                                {
                                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                                }
                                if (memRepo.GetAll(x => x.PromotionId == promotionId && x.ProductId == itemMem.ProductId && memItemExists != null).Any())
                                {
                                    //product exist in promotion detail, update details 
                                    var comm = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionMemberOffer>(itemMem);
                                    comm.Id = memItemExists.Id;
                                    comm.PromotionId = promotionId;
                                    comm.CreatedAt = memItemExists.CreatedAt;
                                    comm.CreatedById = memItemExists.CreatedById;
                                    comm.UpdatedAt = DateTime.UtcNow;
                                    comm.UpdatedById = userId;
                                    comm.IsDeleted = false;
                                    //Detaching tracked entry - exists
                                    memRepo.DetachLocal(_ => _.Id == comm.Id);
                                    memRepo.Update(comm);
                                }
                                else
                                {
                                    await AddNewMemberOffer(promotionId, new List<PromotionProductRequestModel>() { itemMem }, userId).ConfigureAwait(false);
                                }
                            }
                            #endregion
                            break;
                        case "MIXMATCH":
                            #region Mixmatch promotion update
                            //If Promotion Mixmatch is null
                            if (viewModel?.PromotionMixmatch == null)
                            {
                                throw new NullReferenceException(ErrorMessages.MixMatchRequired.ToString(CultureInfo.CurrentCulture));
                            }

                            #region Promotion MixMatch Mapping
                            var mixRepo = _unitOfWork?.GetRepository<PromotionMixmatch>();
                            var mixProdRepo = _unitOfWork?.GetRepository<PromotionMixmatchProduct>();

                            int? mixId = 0;
                            mixId = viewModel.PromotionMixmatch.Id;

                            var mixExists = await mixRepo.GetAll(x => x.Id == mixId && x.PromotionId == promotionId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);

                            var mix = _iAutoMap.Mapping<PromotionMixmatchRequestModel, PromotionMixmatch>(viewModel.PromotionMixmatch);

                            mix.Group = viewModel.PromotionMixmatch.Group;
                            mix.PromotionId = promotionId;
                            mix.UpdatedAt = DateTime.UtcNow;
                            mix.UpdatedById = userId;
                            mix.IsDeleted = false;


                            if (mixExists == null)
                            {
                                mix.Id = 0;
                                mix.CreatedById = userId;
                                var mixResult = await mixRepo.InsertAsync(mix).ConfigureAwait(false);
                                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                if (mixResult != null)
                                {
                                    mixId = mixResult.Id;
                                }
                            }
                            else
                            {
                                mix.Id = mixExists.Id;
                                mix.CreatedAt = mixExists.CreatedAt;
                                mix.CreatedById = mixExists.CreatedById;
                                //Detaching tracked entry - exists
                                mixRepo.DetachLocal(_ => _.Id == mix.Id);
                                mixRepo.Update(mix);
                            }


                            #region Delete Products
                            // Delete children
                            var mixExistingProds = await mixProdRepo.GetAll(x => x.PromotionMixmatchId == mixId
                            && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                            if (viewModel.Promotion?.PromotionProduct?.Count >= 0)
                            {
                                foreach (var existingProd in mixExistingProds.ToList())
                                {
                                    //Need Updating
                                    var prod = viewModel.Promotion?.PromotionProduct.FirstOrDefault(c => c.ProductId == existingProd.ProductId);

                                    if (prod == null && (viewModel.PromotionMixmatch?.Id == existingProd.PromotionMixmatchId))
                                    {
                                        //item is deleted
                                        existingProd.UpdatedAt = DateTime.UtcNow;
                                        existingProd.UpdatedById = userId;
                                        existingProd.IsDeleted = true;
                                        mixProdRepo.Update(existingProd);
                                    }
                                }
                            }
                            #endregion


                            foreach (var itemMix in viewModel.Promotion?.PromotionProduct)
                            {
                                if (!productRepo.GetAll(x => x.Id == itemMix.ProductId && !x.IsDeleted).Any())
                                {
                                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                                }

                                var mixProdExists = mixProdRepo.GetAll(x => x.PromotionMixmatchId == mixId && x.ProductId == itemMix.ProductId).FirstOrDefault();
                                if (mixProdExists != null)
                                {
                                    //product exist in promotion detail, update details 
                                    var comm = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionMixmatchProduct>(itemMix);
                                    comm.PromotionMixmatchId = mixProdExists.PromotionMixmatchId;
                                    comm.Id = mixProdExists.Id;
                                    comm.CreatedAt = mixProdExists.CreatedAt;
                                    comm.CreatedById = mixProdExists.CreatedById;
                                    comm.UpdatedById = userId;
                                    comm.IsDeleted = false;
                                    //Detaching tracked entry - exists
                                    mixProdRepo.DetachLocal(_ => _.Id == comm.Id);
                                    mixProdRepo.Update(comm);
                                }
                                else
                                {
                                    await AddNewPromotionMixMatchProduct(new List<PromotionProductRequestModel>() { itemMix }, (int)mixId, userId).ConfigureAwait(false);
                                }
                            }
                            #endregion
                            #endregion
                            break;
                        default:
                            throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                            // break;
                    }

                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                    // var response = await GetPromotionDetailsById(promotionId).ConfigureAwait(false);
                    //return response;
                    return true;
                }
                throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                //Rollback 
                //_unitOfWork.Dispose();
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Add product to mix match promotion
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="mixmatchId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task AddNewPromotionMixMatchProduct(List<PromotionProductRequestModel> viewModel, int mixmatchId, int userId)
        {
            try
            {
                var productRepo = _unitOfWork?.GetRepository<Product>();
                var mixProdRepo = _unitOfWork?.GetRepository<PromotionMixmatchProduct>();
                foreach (var itemMix in viewModel)
                {
                    if (!productRepo.GetAll(x => x.Id == itemMix.ProductId && !x.IsDeleted).Any())
                    {
                        throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    itemMix.Id = 0;
                    var mixProd = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionMixmatchProduct>(itemMix);
                    mixProd.PromotionMixmatchId = mixmatchId;
                    mixProd.IsDeleted = false;
                    mixProd.CreatedById = userId;
                    mixProd.UpdatedById = userId;
                    mixProd.Status = true;
                    var result = await mixProdRepo.InsertAsync(mixProd).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
            }

#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Add products to Promotion offer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <param name="offerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task AddNewPromotionOfferProducts(List<PromotionProductRequestModel> viewModel, int offerId, int userId)
        {
            var productRepo = _unitOfWork?.GetRepository<Product>();
            var offProdRepo = _unitOfWork?.GetRepository<PromotionOfferProduct>();
            foreach (var itemOff in viewModel)
            {
                if (!productRepo.GetAll(x => x.Id == itemOff.ProductId && !x.IsDeleted).Any())
                {
                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                }

                itemOff.Id = 0;

                if (itemOff.OfferGroup == null || itemOff.OfferGroup == 0)
                {
                    itemOff.OfferGroup = 1;
                }

                var offProd = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionOfferProduct>(itemOff);
                offProd.PromotionOfferId = offerId;
                offProd.IsDeleted = false;
                offProd.Status = true;
                offProd.CreatedById = userId;
                offProd.UpdatedById = userId;
                var result = await offProdRepo.InsertAsync(offProd).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Add new Member Offer Promotion
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task AddNewMemberOffer(int id, List<PromotionProductRequestModel> viewModel, int userId)
        {
            var productRepo = _unitOfWork?.GetRepository<Product>();
            var memRepo = _unitOfWork?.GetRepository<PromotionMemberOffer>();
            if (viewModel.Select(x => x.ProductId).Distinct().Count() != viewModel.Count)
            {
                //can't add same product twice
                throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
            }

            foreach (var itemMO in viewModel)
            {
                if (!productRepo.GetAll(x => x.Id == itemMO.ProductId && !x.IsDeleted).Any())
                {
                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                }
                if (memRepo.GetAll(x => x.PromotionId == id && x.ProductId == itemMO.ProductId & !x.IsDeleted).Any())
                {
                    throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
                }
                itemMO.Id = 0;
                var ofer = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionMemberOffer>(itemMO);
                ofer.PromotionId = id;
                ofer.Status = true;
                ofer.IsDeleted = false;
                ofer.CreatedById = userId;
                ofer.UpdatedById = userId;
                var result = await memRepo.InsertAsync(ofer).ConfigureAwait(false);
            }
            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

        }

        /// <summary>
        /// Add new Selling Promotion
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task AddNewSellingPromotion(int id, List<PromotionProductRequestModel> viewModel, int userId)
        {
            var productRepo = _unitOfWork?.GetRepository<Product>();
            var sellRepo = _unitOfWork?.GetRepository<PromotionSelling>();
            if (viewModel.Select(x => x.ProductId).Distinct().Count() != viewModel.Count)
            {
                //can't add same product twice
                throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
            }


            foreach (var itemS in viewModel)
            {
                //break loop, throw error and Roll back

                if (await sellRepo.GetAll(x => x.PromotionId == id && x.ProductId == itemS.ProductId).AnyAsync().ConfigureAwait(false))
                {
                    //can't add same product twice
                    //skip product
                    return;
                }

                if (!productRepo.GetAll(x => x.Id == itemS.ProductId && !x.IsDeleted).Any())
                {
                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                }
                if (sellRepo.GetAll(x => x.PromotionId == id && x.ProductId == itemS.ProductId).Any())
                {
                    throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
                }
                //Add new in loop and don't commit here
                itemS.Id = 0;

                //If AmtOffNorm has value, prices sholud be null
                if (itemS.AmtOffNorm1 != null && itemS.AmtOffNorm1 > 0)
                {
                    itemS.Price = null;
                    itemS.Price1 = null;
                    itemS.Price2 = null;
                    itemS.Price3 = null;
                    itemS.Price4 = null;
                    itemS.Status = true;
                }

                var sell = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionSelling>(itemS);
                sell.PromotionId = id;
                sell.IsDeleted = false;
                sell.Status = true;
                sell.CreatedById = userId;
                sell.UpdatedById = userId;
                var result = await sellRepo.InsertAsync(sell).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Add buying promotion
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task AddNewBuyingPromotion(int id, List<PromotionProductRequestModel> viewModel, int userId)
        {
            var productRepo = _unitOfWork?.GetRepository<Product>();
            var supplierRepo = _unitOfWork?.GetRepository<Supplier>();
            var buyRepo = _unitOfWork?.GetRepository<PromotionBuying>();
            if (viewModel.Select(x => x.ProductId).Distinct().Count() != viewModel.Count)
            {
                //can't add same product twice
                throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
            }
            foreach (var item in viewModel)
            {
                if (!productRepo.GetAll(x => x.Id == item.ProductId && !x.IsDeleted).Any())
                {
                    throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                }
                if (item.SupplierId == 0)
                {
                    item.SupplierId = null;
                }
                if (item.SupplierId != null)
                {
                    if (!supplierRepo.GetAll(x => x.Id == item.SupplierId && !x.IsDeleted).Any())
                    {
                        throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }
                if (buyRepo.GetAll(x => x.PromotionId == id && x.ProductId == item.ProductId && !x.IsDeleted).Any())
                {
                    throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
                }
                //Add new in loop and don't commit here
                item.Id = 0;
                var comm = _iAutoMap.Mapping<PromotionProductRequestModel, PromotionBuying>(item);
                comm.PromotionId = id;
                comm.IsDeleted = false;
                comm.Status = true;
                comm.CreatedById = userId;
                comm.UpdatedById = userId;
                var result = await buyRepo.InsertAsync(comm).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Delete Promotion Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="detailId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeletePromotionDetails(int id, int detailId, int userId)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Promotion>();
                    var promotion = await repository.GetAll(x => x.Id == id, includes: new Expression<Func<Promotion, object>>[] { c => c.PromotionType }).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (promotion == null)
                    {
                        throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    switch (promotion.PromotionType.Code.ToUpper())
                    {
                        case "BUYING":
                            #region Promotion Buying Mapping Add New 
                            var buyRepo = _unitOfWork?.GetRepository<PromotionBuying>();
                            var exists = await buyRepo.GetById(detailId).ConfigureAwait(false);
                            if (exists != null)
                            {
                                exists.IsDeleted = true;
                                exists.UpdatedById = userId;
                                buyRepo?.Update(exists);
                            }
                            #endregion
                            break;
                        case "OFFER":
                            #region Promotion Offer Mapping
                            var offRepo = _unitOfWork?.GetRepository<PromotionOffer>();
                            var offExists = await offRepo.GetAll(x => x.Id == detailId).Include(x => x.PromotionOfferProduct).FirstOrDefaultAsync().ConfigureAwait(false);
                            if (offExists != null)
                            {
                                foreach (var offProd in offExists.PromotionOfferProduct)
                                {
                                    offProd.IsDeleted = true;
                                    offProd.UpdatedAt = DateTime.UtcNow;
                                    offProd.UpdatedById = userId;
                                    //offRepo?.Update(offProd);
                                }
                                offExists.IsDeleted = true;
                                offExists.UpdatedById = userId;
                                offRepo?.Update(offExists);
                            }
                            #endregion
                            break;
                        case "SELLING":
                            #region Promotion Selling Mapping
                            var sellRepo = _unitOfWork?.GetRepository<PromotionSelling>();
                            var existsSell = await sellRepo.GetById(detailId).ConfigureAwait(false);
                            if (existsSell != null)
                            {
                                existsSell.IsDeleted = true;
                                existsSell.UpdatedById = userId;
                                sellRepo?.Update(existsSell);
                            }
                            #endregion
                            break;
                        case "MEMBEROFFER":
                            #region Promotion Member Offer Mapping
                            var memRepo = _unitOfWork?.GetRepository<PromotionMemberOffer>();
                            var existsMem = await memRepo.GetById(detailId).ConfigureAwait(false);
                            if (existsMem != null)
                            {
                                existsMem.IsDeleted = true;
                                existsMem.UpdatedById = userId;
                                memRepo?.Update(existsMem);
                            }
                            #endregion
                            break;
                        case "MIXMATCH":
                            #region Promotion MixMatch Mapping
                            var mixRepo = _unitOfWork?.GetRepository<PromotionMixmatch>();
                            var existsmix = await mixRepo.GetAll(x => x.Id == detailId).Include(x => x.PromotionMixmatchProduct).FirstOrDefaultAsync().ConfigureAwait(false);
                            if (existsmix != null)
                            {
                                foreach (var prod in existsmix.PromotionMixmatchProduct)
                                {
                                    prod.IsDeleted = true;
                                    prod.UpdatedAt = DateTime.UtcNow;
                                    prod.UpdatedById = userId;
                                }
                                existsmix.IsDeleted = true;
                                existsmix.UpdatedById = userId;
                                mixRepo?.Update(existsmix);
                            }
                            #endregion
                            break;
                        default:
                            throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                            // break;
                    }
                    //Commit changes of list
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    return true;
                }
                throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                //Rollback
                //_unitOfWork.Dispose();
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        //public async Task<PromotionDetailResponseViewModel> CopyPromotion(int promotionId, int userId)
        //{
        //    try
        //    {
        //        var existingPromotion = await GetPromotionDetailsById(promotionId).ConfigureAwait(false);
        //        if (existingPromotion == null)
        //        {
        //            throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
        //        }
        //        var code = existingPromotion.Promotion?.Code.Trim() + "-COPY";
        //        int i = 1;
        //        var repository = _unitOfWork.GetRepository<Promotion>();
        //        while (await repository.GetAll(x => x.Code == code && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
        //        {
        //            code = existingPromotion.Promotion?.Code + "-COPY(" + i + ")";

        //            if (!(code.Length <= MaxLengthConstants.MaxPromotionCodeLength))
        //            {
        //                code = code.Substring(0, 10) + "-COPY(" + i + ")";
        //            }
        //            i++;
        //        }

        //        existingPromotion.Promotion.Code = code;

        //        var requestModel = MappingHelpers.Mapping<PromotionDetailResponseViewModel, PromotionDetailRequestViewModel>(existingPromotion);

        //        int resultId = await AddPromotionDetail(requestModel, userId).ConfigureAwait(false);

        //        repository.DetachLocal(_ => _.Id == resultId);
        //        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
        //        var response = await GetPromotionDetailsById(resultId).ConfigureAwait(false);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Rollback 
        //        _unitOfWork.Dispose();
        //        if (ex is NotFoundException)
        //        {
        //            throw new NotFoundException(ex.Message);
        //        }
        //        if (ex is AlreadyExistsException)
        //        {
        //            throw new AlreadyExistsException(ex.Message);
        //        }
        //        if (ex is BadRequestException)
        //        {
        //            throw new BadRequestException(ex.Message);
        //        }
        //        if (ex is NullReferenceException)
        //        {
        //            throw new NullReferenceException(ex.Message);
        //        }
        //        throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
        //    }
        //}

        public async Task DeletePreviousPromotionsAsync(int promotypeId, int promotionId, int userId)
        {
            var codeRepo = _unitOfWork.GetRepository<MasterListItems>();

            var promoType = await codeRepo.GetAll(x => x.Id == promotypeId && !x.IsDeleted).Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);

            if (promoType == null)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
            }

            if (promoType.ToUpper() != "BUYING")
            {
                //Delete Buying Promotion

                var buyRepo = _unitOfWork?.GetRepository<PromotionBuying>();

                var buyExistingProds = await buyRepo.GetAll(x => x.PromotionId == promotionId
                    && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);


                foreach (var existingProd in buyExistingProds.ToList())
                {
                    //item is deleted
                    existingProd.UpdatedAt = DateTime.UtcNow;
                    existingProd.UpdatedById = userId;
                    existingProd.IsDeleted = true;
                    buyRepo.Update(existingProd);
                }
            }

            if (promoType.ToUpper() != "SELLING")
            {
                //Delete SELLING Promotion

                var sellRepo = _unitOfWork?.GetRepository<PromotionSelling>();

                var sellExistingProds = await sellRepo.GetAll(x => x.PromotionId == promotionId
                && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                foreach (var existingProd in sellExistingProds.ToList())
                {
                    //item is deleted
                    existingProd.UpdatedAt = DateTime.UtcNow;
                    existingProd.UpdatedById = userId;
                    existingProd.IsDeleted = true;
                    sellRepo.Update(existingProd);
                }
            }

            if (promoType.ToUpper() != "MEMBEROFFER")
            {
                var memRepo = _unitOfWork?.GetRepository<PromotionMemberOffer>();

                var membExistingProds = await memRepo.GetAll(x => x.PromotionId == promotionId
                && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);
                foreach (var existingProd in membExistingProds.ToList())
                {
                    //item is deleted
                    existingProd.UpdatedAt = DateTime.UtcNow;
                    existingProd.UpdatedById = userId;
                    existingProd.IsDeleted = true;
                    memRepo.Update(existingProd);
                }
            }

            if (promoType.ToUpper() != "OFFER")
            {
                //Delete OFFER Promotion

                var offRepo = _unitOfWork?.GetRepository<PromotionOffer>();
                var offProdRepo = _unitOfWork?.GetRepository<PromotionOfferProduct>();

                var offerExists = await offRepo.GetAll(x => x.PromotionId == promotionId
                  && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                foreach (var existingoffer in offerExists.ToList())
                {
                    //item is deleted
                    existingoffer.UpdatedAt = DateTime.UtcNow;
                    existingoffer.UpdatedById = userId;
                    existingoffer.IsDeleted = true;
                    offRepo.Update(existingoffer);

                    //Delete Offer Products
                    var offerExistingProds = await offProdRepo.GetAll(x => x.PromotionOfferId == existingoffer.Id && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                    foreach (var existingProd in offerExistingProds.ToList())
                    {
                        //item is deleted
                        existingProd.UpdatedAt = DateTime.UtcNow;
                        existingProd.UpdatedById = userId;
                        existingProd.IsDeleted = true;
                        offProdRepo.Update(existingProd);

                    }

                }
            }

            if (promoType.ToUpper() != "MIXMATCH")
            {
                //Delete MIXMATCH Promotion
                var mixRepo = _unitOfWork?.GetRepository<PromotionMixmatch>();
                var mixProdRepo = _unitOfWork?.GetRepository<PromotionMixmatchProduct>();

                var mixExists = await mixRepo.GetAll(x => x.PromotionId == promotionId
                   && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);


                foreach (var existingMix in mixExists)
                {
                    //item is deleted
                    existingMix.UpdatedAt = DateTime.UtcNow;
                    existingMix.UpdatedById = userId;
                    existingMix.IsDeleted = true;
                    mixRepo.Update(existingMix);

                    var mixExistingProds = await mixProdRepo.GetAll(x => x.PromotionMixmatchId == existingMix.Id && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                    foreach (var existingProd in mixExistingProds.ToList())
                    {

                        //item is deleted
                        existingProd.UpdatedAt = DateTime.UtcNow;
                        existingProd.UpdatedById = userId;
                        existingProd.IsDeleted = true;
                        mixProdRepo.Update(existingProd);
                    }
                }

            }

            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        }

        public async Task<PromotionDetailResponseViewModel> CopyPromotion(PromotionCloneFilter inputModel, int userId)
        {
            try
            {
                if (inputModel != null)
                {
                    var existingPromotion = await GetPromotionDetailsById(inputModel.Id).ConfigureAwait(false);

                    if (existingPromotion == null)
                    {
                        throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                    //do not check for delete.Using to check competition only
                    var type = masterRepository.GetAll(x => x.Id == existingPromotion.Promotion.PromotionTypeId).FirstOrDefault();

                    //To change request for Competition.
                    if (type.Name.ToLower() == "competition")
                    {
                        throw new BadRequestException(ErrorMessages.PromotionCompetitionCopy.ToString(CultureInfo.CurrentCulture));
                    }


                    var code = "";
                    var repository = _unitOfWork.GetRepository<Promotion>();
                    if (!string.IsNullOrEmpty(inputModel.Code))
                    {
                        code = inputModel.Code.Trim();
                        if (await repository.GetAll(x => x.Code == code && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new AlreadyExistsException(ErrorMessages.PromotionCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        code = existingPromotion.Promotion?.Code.Trim() + "-COPY";
                        int i = 1;

                        while (await repository.GetAll(x => x.Code == code && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            code = existingPromotion.Promotion?.Code + "-COPY(" + i + ")";

                            if (!(code.Length <= MaxLengthConstants.MaxPromotionCodeLength))
                            {
                                code = code.Substring(0, 10) + "-COPY(" + i + ")";
                            }
                            i++;
                        }
                    }

                    existingPromotion.Promotion.Code = code;
                    existingPromotion.Promotion.Desc = inputModel.Desc != null ? inputModel.Desc : code;
                    var outRepo = _unitOfWork.GetRepository<OutletProduct>();
                    if (inputModel.OutletId != null)
                    {

                        List<long> outletProdList = new List<long>();
                        var prodList = existingPromotion.Promotion.PromotionProduct.ToList();
                        if (prodList != null)
                        {
                            foreach (var prod in prodList)
                            {
                                {
                                    var outlProd = await outRepo.GetAll(x => x.StoreId == inputModel.OutletId && !x.IsDeleted && x.Status && x.ProductId == prod.ProductId).FirstOrDefaultAsync().ConfigureAwait(false);
                                    if (outlProd != null) outletProdList.Add(outlProd.ProductId);
                                }
                            }
                        }

                        //Delete other Products from List

                        if (outletProdList?.Count >= 0)
                        {

                            foreach (var product in outletProdList)
                            {

                                var prodExist = existingPromotion.Promotion?.PromotionProduct.Where(x => x.Id == product).FirstOrDefault();
                                if (prodExist == null)
                                {
                                    existingPromotion.Promotion?.PromotionProduct.Remove(prodExist);
                                }
                            }
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        }
                    }

                    if (inputModel.ZoneId != null)
                    {

                        var MasterListRepo = _unitOfWork.GetRepository<MasterListItems>();

                        if (!await MasterListRepo.GetAll(x => x.Id == inputModel.ZoneId && !x.IsDeleted && x.Status).Include(x => x.MasterList).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidZone.ToString(CultureInfo.CurrentCulture));
                        }

                        var zoneRepo = _unitOfWork.GetRepository<ZoneOutlet>();
                        if (inputModel.OutletId != null)
                        {
                            var outletList = await zoneRepo.GetAll(x => x.ZoneId == inputModel.ZoneId && x.StoreId == inputModel.OutletId).ToListAsyncSafe().ConfigureAwait(false);
                            if (outletList == null || outletList.Count <= 0)
                            { existingPromotion.Promotion?.PromotionProduct.Clear(); }
                        }
                        else
                        {
                            var zoneOutletList = await zoneRepo.GetAll(x => x.ZoneId == inputModel.ZoneId).ToListAsyncSafe().ConfigureAwait(false);
                            foreach (var zone in zoneOutletList)
                            {
                                var outletProdList = await outRepo.GetAll(x => x.Status && !x.IsDeleted && x.StoreId == zone.StoreId).ToListAsyncSafe().ConfigureAwait(false);

                                if (outletProdList?.Count >= 0)
                                {

                                    foreach (var product in outletProdList)
                                    {
                                        var prodExist = existingPromotion.Promotion?.PromotionProduct.Where(x => x.Id == product.ProductId).FirstOrDefault();
                                        if (prodExist == null)
                                        {
                                            existingPromotion.Promotion?.PromotionProduct.Remove(prodExist);
                                        }
                                    }
                                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                }

                            }
                        }

                    }

                    var requestModel = MappingHelpers.Mapping<PromotionDetailResponseViewModel, PromotionDetailRequestModel>(existingPromotion);


                    if (requestModel.PromotionOffer != null) { requestModel.PromotionOffer.Id = 0; }
                    if (requestModel.PromotionMixmatch != null) { requestModel.PromotionMixmatch.Id = 0; }

                    int resultId = await AddPromotionDetail(requestModel, userId).ConfigureAwait(false);

                    repository.DetachLocal(_ => _.Id == resultId);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    var response = await GetPromotionDetailsById(resultId).ConfigureAwait(false);
                    return response;
                }
                else
                {
                    throw new NotFoundException(ErrorMessages.CashierIdRequired.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (Exception ex)
            {
                //Rollback 
                //_unitOfWork.Dispose();
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


        public async Task<int> ImportPromotionDetail(PromotionImportRequestModel importRequestModel, string path, int userId, int? promotionId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Promotion>();
                if (importRequestModel != null)
                {
                    if (importRequestModel.Promotion == null)
                    {
                        throw new BadRequestException(ErrorMessages.PromotionRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    if (string.IsNullOrEmpty(path))
                    {
                        throw new BadRequestException(ErrorMessages.CSVNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    #region CSV Read

                    var filePath = Directory.GetCurrentDirectory() + path;

                    var lineCount = File.ReadLines(filePath).Count();

                    var promoDset = CSVReader.ConvertPromoCSVtoDataTable(filePath);

                    #endregion

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                //new SqlParameter("@ProductId", 0),
                //new SqlParameter("@UserId", userId),
                new SqlParameter{
                  Direction = ParameterDirection.Input,
                  ParameterName = "@promoProductImportRequestType",
                  TypeName ="[dbo].[PromoProductImportRequestType]",
                  Value = promoDset,
                  SqlDbType = SqlDbType.Structured
                } };

                 var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetPromoProductToInsert, dbParams.ToArray()).ConfigureAwait(false);


                    List<PromotionProductRequestModel> promoProducts = MappingHelpers.ConvertDataTable<PromotionProductRequestModel>(dset.Tables[0]);

                    //call SP here to convert CSV to PromoProducts

                //    var promoProducts = new List<PromotionProductRequestModel>();

                    //promoProducts = promoImportProducts.Select(MappingHelpers.Mapping<PromoProductImportModel, PromotionProductRequestModel>).ToList();

                    #region keep commented
                    //Check promotion product duplicate in CSV
                    //{
                    //    //can't add same product twice
                    //    throw new BadRequestException(ErrorMessages.PromotionBuyingProductDuplicate.ToString(CultureInfo.CurrentCulture));
                    //}
                    #endregion
                    //Insert only after successfully CSV is read

                    var promotion = new PromotionResponseViewModel();
                    if (promotionId == null)
                    {
                        promotion = await Insert(importRequestModel?.Promotion, userId).ConfigureAwait(false);
                    }
                    else if (promotionId > 0)
                    {
                        promotion =  await Update(importRequestModel.Promotion, (int)promotionId, userId).ConfigureAwait(false);
                    }
                    if (promotion == null)
                    {
                        throw new BadRequestException(ErrorMessages.PromotionNotSaved.ToString(CultureInfo.CurrentCulture));
                    }

                    switch (promotion.PromotionType.ToUpper())
                    {
                        case "BUYING":
                            #region Promotion Buying Mapping Add New 
                            await AddNewBuyingPromotion(promotion.Id, promoProducts, userId).ConfigureAwait(false);
                            #endregion
                            break;
                        case "OFFER":

                            #region Save offer settings and Offer Products

                            //If Promotion Offer is null
                            if (importRequestModel?.PromotionOffer == null)
                            {
                                throw new NullReferenceException(ErrorMessages.OfferRequired.ToString(CultureInfo.CurrentCulture));
                            }
                            #region Promotion Offer Mapping

                            var productRepo = _unitOfWork?.GetRepository<Product>();
                            var offRepo = _unitOfWork?.GetRepository<PromotionOffer>();
                            var offProdRepo = _unitOfWork?.GetRepository<PromotionOfferProduct>();
                            var offer = _iAutoMap.Mapping<PromotionOfferRequestModel, PromotionOffer>(importRequestModel.PromotionOffer);
                            offer.PromotionId = promotion.Id;
                            offer.IsDeleted = false;
                            offer.CreatedById = userId;
                            offer.UpdatedById = userId;
                            var resultOff = await offRepo.InsertAsync(offer).ConfigureAwait(false);
                            // TODO: Need to remove save changes from here as this is against unit of work
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            if (resultOff.Id > 0)
                            {
                                await AddNewPromotionOfferProducts(promoProducts, offer.Id, userId).ConfigureAwait(false);
                            }
                            else
                            {
                                throw new Exception(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            #endregion
                            #endregion

                            break;
                        case "SELLING":
                            #region Promotion Selling Mapping 
                            await AddNewSellingPromotion(promotion.Id, promoProducts, userId).ConfigureAwait(false);
                            #endregion
                            break;
                        case "MEMBEROFFER":
                            #region Promotion Member Offer Mapping
                            await AddNewMemberOffer(promotion.Id, promoProducts, userId).ConfigureAwait(false);
                            #endregion
                            break;
                        case "MIXMATCH":

                            //If Promotion Mixmatch is null
                            if (importRequestModel.PromotionMixmatch == null)
                            {
                                throw new NullReferenceException(ErrorMessages.MixMatchRequired.ToString(CultureInfo.CurrentCulture));
                            }

                            #region Promotion MixMatch Mapping
                            var mixRepo = _unitOfWork?.GetRepository<PromotionMixmatch>();
                            var mix = _iAutoMap.Mapping<PromotionMixmatchRequestModel, PromotionMixmatch>(importRequestModel.PromotionMixmatch);
                            mix.PromotionId = promotion.Id;
                            mix.IsDeleted = false;
                            mix.CreatedById = userId;
                            mix.UpdatedById = userId;
                            mix.Status = true;
                            var resultMix = await mixRepo.InsertAsync(mix).ConfigureAwait(false);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                            if (resultMix.Id > 0)
                            {
                                await AddNewPromotionMixMatchProduct(promoProducts, mix.Id, userId).ConfigureAwait(false);
                            }
                            else
                            {
                                throw new Exception(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            #endregion
                            break;
                        default:
                            throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
                            // break;
                    }
                    //Commit changes of list
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    return promotion.Id;
                }
                throw new NullReferenceException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                //Rollback 
                //_unitOfWork.Dispose();
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



    }
}
