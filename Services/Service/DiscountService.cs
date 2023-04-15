using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class DiscountService : IDiscountService
    {
        public IMapper _mapper { get; }
        public IDiscountRepository _discountRepository { get; }
        public IGoodsRepository _goodsRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public DiscountService(IMapper mapper, IDiscountRepository discountRepository,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
         IGoodsRepository goodsRepository, ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
            this._mapper = mapper;
            this._goodsRepository = goodsRepository;
            this._discountRepository = discountRepository;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            _ms = ms;
        }
        public async Task<ApiResponse<DiscountPlanAddDto>> DiscountPlanAdd(DiscountPlanAddDto discountDto)
        {
            if (discountDto.FkPlanTypeId == (int)PlanTypeEnum.DiscountCode)
            {
                discountDto.PermittedUseNumberAll = discountDto.CouponCodeCount * discountDto.PermittedUseNumberPerCode;
                if (discountDto.PermittedUseNumberAll < discountDto.CouponCodeCount)
                {
                    return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                }
                if (discountDto.UseLimitationType == true)
                {
                    if (discountDto.PermittedUseNumberPerCode != 1)
                    {
                        return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                    }
                }
                // if (discountDto.PermittedUseNumberPerCode == 1)
                // {
                //     if (discountDto.UseLimitationType == false)
                //     {
                //         return null;
                //     }
                // }
                if (discountDto.ActiveForFirstBuy == true)
                {
                    if (discountDto.UseLimitationType != true)
                    {
                        return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                    }
                    if (discountDto.PermittedUseNumberPerCustomer != 1)
                    {
                        return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                    }
                }
                if (discountDto.FkCouponCodeTypeId == (short)CouponCodeTypeEnum.Unique)
                {
                    if (discountDto.CouponCodeCount != 1)
                    {
                        return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                    }
                }
                if (discountDto.FkCouponCodeTypeId == (short)CouponCodeTypeEnum.Unique || discountDto.FkCouponCodeTypeId == (short)CouponCodeTypeEnum.Prefixed)
                {
                    if (string.IsNullOrWhiteSpace(discountDto.CouponCodePrefix))
                    {
                        return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                    }
                }
            }
            else
            {
                // discountDto.FreeShippingCost = false;
                // discountDto.FreeProduct = false;
            }

            if(token.Rule == UserGroupEnum.Seller)
            {
                discountDto.FkShopId = token.Id;
                // discountDto.TDiscountShops = new List<DiscountShopsDto>();
                // var discountShop = new DiscountShopsDto();
                // discountShop.Allowed = true;
                // discountShop.AssignedShopId = 0;
                // discountShop.FkDiscountPlanId = 0;
                // discountShop.FkShopId = token.Id;
                // discountDto.TDiscountShops.Add(discountShop);
            }
            

            if (discountDto.TDiscountCategory.Count > 0)
            {
                var firstValue = discountDto.TDiscountCategory[0].Allowed;
                if (discountDto.TDiscountCategory.Any(x => x.Allowed != firstValue))
                {
                    return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                }
            }

            if (discountDto.TDiscountCustomers.Count > 0)
            {
                var firstValue = discountDto.TDiscountCustomers[0].Allowed;
                if (discountDto.TDiscountCustomers.Any(x => x.Allowed != firstValue))
                {
                    return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                }
            }

            if (discountDto.TDiscountGoods.Count > 0)
            {
                var firstValue = discountDto.TDiscountGoods[0].Allowed;
                if (discountDto.TDiscountGoods.Any(x => x.Allowed != firstValue))
                {
                    return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                }
            }

            // if (discountDto.TDiscountShops.Count > 0)
            // {
            //     var firstValue = discountDto.TDiscountShops[0].Allowed;
            //     if (discountDto.TDiscountShops.Any(x => x.Allowed != firstValue))
            //     {
            //         return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
            //     }
            // }

            if (discountDto.TimingType == true)
            {
                if (discountDto.StartDateTime == (DateTime?)null||discountDto.EndDateTime == (DateTime?)null)
                {
                    return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                }
            }

            var mapDiscount = _mapper.Map<TDiscountPlan>(discountDto);
            if (discountDto.FkPlanTypeId == (int)PlanTypeEnum.DiscountCode)
            {
                mapDiscount.TDiscountCouponCode = await GenerateCode(mapDiscount.FkCouponCodeTypeId, mapDiscount.CouponCodePrefix, (int)mapDiscount.PermittedUseNumberPerCode, (int)mapDiscount.CouponCodeCount);
                if (mapDiscount.TDiscountCouponCode == null)
                {
                    return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                }
            }
            else if (discountDto.FkPlanTypeId == (int)PlanTypeEnum.SpecialSell)
            {
                discountDto.UseWithOtherDiscountPlan = true;
            }
            var CreateDiscount = await _discountRepository.DiscountPlanAdd(mapDiscount);
            if (CreateDiscount == null)
            {
                return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
            }

            if (discountDto.FkPlanTypeId == (int)PlanTypeEnum.SpecialSell)
            {
                if (CreateDiscount.TimingType == false && CreateDiscount.Status == true)
                {
                    var setDiscount = await _discountRepository.SetDiscount(CreateDiscount.PlanId);
                    if (setDiscount == false)
                    {
                        // what am i do
                    }
                }
            }

            var mapCreatedDiscount = _mapper.Map<DiscountPlanAddDto>(CreateDiscount);
            return new ApiResponse<DiscountPlanAddDto>(ResponseStatusEnum.Success, mapCreatedDiscount,_ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<DiscountPlanEditDto>> DiscountPlanEdit(DiscountPlanEditDto discountDto)
        {
            if(token.Rule == UserGroupEnum.Seller)
            {
                if(!await _discountRepository.ShopHasDiscount(token.Id,discountDto.PlanId))
                {
                    return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.NotFound,null,_ms.MessageService(Message.DiscountEditing));
                }
            }
            var oldDiscount = await _discountRepository.DiscountPlanGetById(discountDto.PlanId);
            if(oldDiscount == null)
            {
                return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.NotFound,null,_ms.MessageService(Message.DiscountEditing));
            }
            if (oldDiscount.FkPlanTypeId == (int)PlanTypeEnum.DiscountCode)
            {
                if (oldDiscount.ActiveForFirstBuy == true)
                {
                    if (discountDto.PermittedUseNumberPerCustomer != 1)
                    {
                        return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountEditing));
                    }
                }
                if (discountDto.MaximumDiscountAmount == null || discountDto.MaximumDiscountAmount == 0
                || discountDto.MinimumOrderAmount == null || discountDto.MinimumOrderAmount == 0)
                {
                    return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountEditing));
                }
            }
            else
            {
                // discountDto.FreeShippingCost = false;
                // discountDto.FreeProduct = false;
            }

            if(token.Rule == UserGroupEnum.Seller)
            {
                discountDto.FkShopId = token.Id;
                // discountDto.TDiscountShops = new List<DiscountShopsDto>();
                // var discountShop = new DiscountShopsDto();
                // discountShop.Allowed = true;
                // discountShop.AssignedShopId = 0;
                // discountShop.FkDiscountPlanId = 0;
                // discountShop.FkShopId = token.Id;
                // discountDto.TDiscountShops.Add(discountShop);
            }


            // if (discountDto.TDiscountCategory.Count > 0)
            // {
            //     var firstValue = discountDto.TDiscountCategory[0].Allowed;
            //     if (discountDto.TDiscountCategory.Any(x => x.Allowed != firstValue))
            //     {
            //         return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountEditing));
            //     }
            // }

            // if (discountDto.TDiscountCustomers.Count > 0)
            // {
            //     var firstValue = discountDto.TDiscountCustomers[0].Allowed;
            //     if (discountDto.TDiscountCustomers.Any(x => x.Allowed != firstValue))
            //     {
            //         return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountEditing));
            //     }
            // }

            if (discountDto.TDiscountGoods.Count > 0)
            {
                var firstValue = discountDto.TDiscountGoods[0].Allowed;
                if (discountDto.TDiscountGoods.Any(x => x.Allowed != firstValue))
                {
                    return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountEditing));
                }
            }

            // if (discountDto.TDiscountShops.Count > 0)
            // {
            //     var firstValue = discountDto.TDiscountShops[0].Allowed;
            //     if (discountDto.TDiscountShops.Any(x => x.Allowed != firstValue))
            //     {
            //         return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountEditing));
            //     }
            // }

            if (discountDto.TimingType == true)
            {
                if (discountDto.StartDateTime == (DateTime?)null||discountDto.EndDateTime == (DateTime?)null)
                {
                    return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountAdding));
                }
            }

            if (oldDiscount.Status == true && oldDiscount.FkPlanTypeId == (int)PlanTypeEnum.SpecialSell)
            {
                var unset = await _discountRepository.UnSetDiscount(discountDto.PlanId);
            }

            var mapDiscount = _mapper.Map<TDiscountPlan>(discountDto);
            var EditDiscount = await _discountRepository.DiscountPlanEdit(mapDiscount);
            if (EditDiscount == null)
            {
                return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountEditing));
            }

            if (oldDiscount.FkPlanTypeId == (int)PlanTypeEnum.SpecialSell)
            {
                if (discountDto.TimingType == false && discountDto.Status == true)
                {
                    var setDiscount = await _discountRepository.SetDiscount(discountDto.PlanId);
                    if (setDiscount == false)
                    {
                        // what am i do
                    }
                }

                else if (discountDto.TimingType == true && discountDto.Status == true)
                {
                    var date = DateTime.Now;
                    if (discountDto.StartDateTime <= date && discountDto.EndDateTime >= date )
                    {
                        var setDiscount = await _discountRepository.SetDiscount(discountDto.PlanId);
                        if (setDiscount == false)
                        {
                            // what am i do
                        }
                    }
                }
            }

            return new ApiResponse<DiscountPlanEditDto>(ResponseStatusEnum.Success, discountDto,_ms.MessageService(Message.Successfull));

        }

        private async Task<List<TDiscountCouponCode>> GenerateCode(int? couponCodeType, string discountCode, int maxUse, int? count)
        {
            var CodeList = new List<DiscountCodeDto>();
            if (couponCodeType == (short)CouponCodeTypeEnum.Unique)
            {
                var newCode = new DiscountCodeDto(0, 0, discountCode, maxUse, 0, true);
                CodeList.Add(newCode);
            }

            else if (couponCodeType == (short)CouponCodeTypeEnum.Prefixed)
            {
                var GetRandomString = await _discountRepository.GetRandomString((int)count);
                if (GetRandomString == null || GetRandomString.Count != (int)count)
                {
                    return null;
                }
                foreach (var item in GetRandomString)
                {
                    var newCode = new DiscountCodeDto(0, 0, discountCode + item, maxUse, 0, true);
                    CodeList.Add(newCode);
                }

            }
            else if (couponCodeType == (short)CouponCodeTypeEnum.ByAccident)
            {
                var GetRandomString = await _discountRepository.GetRandomString((int)count);
                if (GetRandomString == null || GetRandomString.Count != (int)count)
                {
                    return null;
                }
                foreach (var item in GetRandomString)
                {
                    var newCode = new DiscountCodeDto(0, 0, item, maxUse, 0, true);
                    CodeList.Add(newCode);
                }
            }

            return _mapper.Map<List<TDiscountCouponCode>>(CodeList);
        }

        public async Task<ApiResponse<Pagination<DiscountPlanGetDto>>> DiscountPlanGet(DiscountFilterDto filterDto)
        {
            if(token.Rule == UserGroupEnum.Seller)
            {
                filterDto.ShopId = token.Id;
            }
            var catIds = new List<int>(); 
            if (filterDto.CategoryId != 0)
            {
                //man ino yadam nemiad vali fek konam intori bashe ke parent haro begirim
                catIds.AddRange(await _categoryRepository.GetParentCatIds(filterDto.CategoryId));
            }
            if (filterDto.GoodsId != 0)
            {
                var catId = await _categoryRepository.GetGoodsCategoryId(filterDto.GoodsId);
                var parentIds = await _categoryRepository.GetParentCatIds(catId);
                catIds.AddRange(parentIds);
            }
            filterDto.CatIds = catIds;
            var data = await _discountRepository.DiscountPlanGet(filterDto);
            if (data == null)
            {
                return null;
            }
            var count = await _discountRepository.DiscountPlanCountGet(filterDto);

            return new ApiResponse<Pagination<DiscountPlanGetDto>>(ResponseStatusEnum.Success, new Pagination<DiscountPlanGetDto>(count, data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<DiscountCodeExelDto>>> GetCoponCodeForExel(int planId)
        {
            var data = await _discountRepository.GetCoponCodeForExel(planId);
            if (data == null)
            {
                return new ApiResponse<List<DiscountCodeExelDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountCodeGetting));
            }
            return new ApiResponse<List<DiscountCodeExelDto>>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<DiscountCodeDetailDto>>> GetDiscountCodeDetail(DiscountCodePaginationDto pagination)
        {
            var data = await _discountRepository.GetDiscountCodeDetail(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<DiscountCodeDetailDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountCodeGetting));
            }
            var count = await _discountRepository.GetDiscountCodeDetailCount(pagination);
            return new ApiResponse<Pagination<DiscountCodeDetailDto>>(ResponseStatusEnum.Success, new Pagination<DiscountCodeDetailDto>(count, data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept)
        {
            var result = await _discountRepository.ChangeAccept(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.DiscountChangeActive));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<DiscountPlanGetOneDto>> GetOne(int planId)
        {
            var data = await _discountRepository.GetOne(planId);
            if (data == null)
            {
                return new ApiResponse<DiscountPlanGetOneDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.DiscountGetting));
            }
            return new ApiResponse<DiscountPlanGetOneDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> DeleteDiscount(long planId)
        {
            var result = await _discountRepository.deleteDiscount(planId);
            if (result.Data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(result.Message));
            }        
        }
    }
}