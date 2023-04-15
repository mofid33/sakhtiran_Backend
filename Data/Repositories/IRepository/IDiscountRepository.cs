using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IDiscountRepository
    {
        Task<List<SpecialSellPlanDto>> GetSpecialSellPlanByIds(List<int> ids);
        Task<TDiscountPlan> GetGoodsAndCatIdsByPlanId(int planId);
        Task<TDiscountPlan> DiscountPlanAdd(TDiscountPlan discount);
        Task<bool> SetDiscount(long planId);
        Task<bool> UnSetDiscount(long planId);
        Task<List<string>> GetRandomString(int count);
        Task<List<TDiscountPlan>> GetDiscountByDateAndTime(DateTime date); 
        Task<List<DiscountPlanGetDto>> DiscountPlanGet(DiscountFilterDto filterDto);
        Task<int> DiscountPlanCountGet(DiscountFilterDto filterDto);
        Task<List<DiscountCodeExelDto>> GetCoponCodeForExel(int planId);
        Task<List<DiscountCodeDetailDto>> GetDiscountCodeDetail(DiscountCodePaginationDto pagination);
        Task<int> GetDiscountCodeDetailCount(DiscountCodePaginationDto pagination);
        Task<TDiscountPlan> DiscountPlanGetById(long planId);
        Task<TDiscountPlan> DiscountPlanEdit(TDiscountPlan discountDto);
        Task<bool> ChangeAccept(AcceptDto accept);
        Task<DiscountPlanGetOneDto> GetOne(int planId);
        Task<bool> ShopHasDiscount(int shopId,long planId);

        Task<RepRes<bool>> deleteDiscount(long planId);
    }
}