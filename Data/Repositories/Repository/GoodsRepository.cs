using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Header;
using AutoMapper;
using System;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Accept;
using System.Linq.Expressions;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class GoodsRepository : IGoodsRepository
    {

        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMapper _mapper { get; set; }
        public ICategoryRepository _categoryRepository { get; }
        public IWareHouseRepository _wareHouseRepository { get; }
        public IMessageRepository _messageRepository { get; }

        public GoodsRepository(MarketPlaceDbContext context, IWareHouseRepository wareHouseRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICategoryRepository categoryRepository,
        IMessageRepository messageRepository)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
            this._categoryRepository = categoryRepository;
            _mapper = mapper;
            _wareHouseRepository = wareHouseRepository;
            this._messageRepository = messageRepository;
        }

        public async Task<TGoods> GoodsAdd(TGoods goods)
        {
            try
            {
                goods.Title = JsonExtensions.JsonAdd(goods.Title, header);
                goods.MetaDescription = goods.Title;
                goods.MetaTitle = goods.Title;
                goods.MetaKeywords = goods.Title;
                // goods.SurveyCount = 1;
                Random rnd = new Random();
                // goods.SurveyScore = rnd.Next(3, 6);

                //goodsproviderAdding
                if (goods.TGoodsProvider != null)
                {
                    var rate = (decimal)1.00;
                    if ( header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                    }
                    foreach (var item in goods.TGoodsProvider)
                    {
                        item.Price = item.Price / rate;
                        item.IsAccepted = true;
                        if (item.InventoryCount > 0 && !goods.IsDownloadable)
                        {
                            item.HasInventory = true;
                            var opration = new TStockOperation();
                            opration.FkOperationTypeId = (int)StockOperationTypeEnum.Import;
                            opration.FkStockId = item.ProviderId;
                            opration.OperationDate = DateTime.Now;
                            opration.OperationStockCount = (double)item.InventoryCount;
                            opration.SaleUnitPrice = item.Price;
                            opration.OperationComment = "ورود " + item.InventoryCount + " عدد " + JsonExtensions.JsonGet(goods.Title, header) + "به انبار";
                            item.TStockOperation = new List<TStockOperation>();
                            item.TStockOperation.Add(opration);
                        }
                        if (goods.IsDownloadable)
                        {
                            item.HasInventory = true;
                        }
                    }
                }
                await _context.TGoods.AddAsync(goods);
                await _context.SaveChangesAsync();
                if (goods.TGoodsProvider != null)
                {
                    foreach (var item in goods.TGoodsProvider)
                    {
                        var price = await this.CalculateDiscountSpecialSell(item, goods);
                        item.DiscountAmount = price.DiscountAmount;
                        item.DiscountPercentage = price.DiscountPercentage;
                        var shop = await _context.TShop.Include(x => x.FkCountry).AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == item.FkShopId);
                        if (shop.GoodsIncludedVat)
                        {
                            if (item.Vatfree == true)
                            {
                                item.Vatamount = 0;
                            }
                            else
                            {
                                item.Vatamount = ((item.Price - item.DiscountAmount) * shop.FkCountry.Vat) / 100;
                            }
                        }
                        else
                        {
                            item.Vatfree = true;
                            item.Vatamount = 0;
                        }
                    }
                }
                goods.GoodsCode = "GN-" + goods.GoodsId;
                goods.LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                // goods.Title = JsonExtensions.JsonGet(goods.Title, header);
                if (token.Rule == UserGroupEnum.Seller && goods.FkOwnerId != null)
                {
                    goods.FkOwner = await _context.TShop.FirstOrDefaultAsync(x => x.ShopId == goods.FkOwnerId);
                }
                return goods;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TGoodsProvider> CalculateDiscountSpecialSell(TGoodsProvider goodsProvider, TGoods goods)
        {
            try
            {
                goodsProvider.DiscountPercentage = (decimal)0.00;
                goodsProvider.DiscountAmount = (decimal)0.00;
                var categoryIds = await _categoryRepository.GetParentCatIds(goods.FkCategoryId);
                var allCopons = await _context.TDiscountPlan
                .Include(t => t.TDiscountCategory)
                .Include(t => t.TDiscountGoods)
                .Include(t => t.TDiscountShops)
                .Where(r => r.Status == true &&
                r.FkPlanTypeId == (int)PlanTypeEnum.SpecialSell &&
                (r.FkShopId == null || r.FkShopId == goodsProvider.FkShopId) &&
                ((r.TDiscountCategory.Any(i => categoryIds.Contains(i.FkCategoryId)) && (r.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory) ||
                (r.TDiscountGoods.Any(i => (i.FkGoodsId == goodsProvider.FkGoodsId && (i.FkVarietyId == null || goodsProvider.ProviderId == i.FkVarietyId) && i.Allowed == true) || (i.FkGoodsId != goods.GoodsId && (i.FkVarietyId != null || goodsProvider.ProviderId != i.FkVarietyId) && i.Allowed == false)) && (r.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)) ||
                r.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)))
                .AsNoTracking().ToListAsync();

                var DiscountPercentage = (decimal)0.00;
                var DisountAmount = (decimal)0.00;

                foreach (var catId in categoryIds)
                {
                    foreach (var copon in allCopons)
                    {
                        if (copon.TDiscountCategory.Count > 0)
                        {
                            foreach (var coponCatId in copon.TDiscountCategory)
                            {
                                if (coponCatId.FkCategoryId == catId && coponCatId.Allowed == true)
                                {
                                    if (copon.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                                    {
                                        DiscountPercentage = DiscountPercentage + ((copon.DiscountAmount * 100) / (decimal)goodsProvider.Price);
                                        DisountAmount = DisountAmount + copon.DiscountAmount;
                                    }
                                    if (copon.FkDiscountTypeId == (int)DiscountTypeId.PercentDiscount)
                                    {
                                        DiscountPercentage = DiscountPercentage + copon.DiscountAmount;
                                        DisountAmount = DisountAmount + ((copon.DiscountAmount * (decimal)goodsProvider.Price) / 100);
                                    }
                                }
                            }
                        }
                    }
                }


                foreach (var copon in allCopons)
                {
                    if (copon.TDiscountGoods.Count > 0)
                    {
                        if (copon.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                        {
                            DiscountPercentage = DiscountPercentage + ((copon.DiscountAmount * 100) / (decimal)goodsProvider.Price);
                            DisountAmount = DisountAmount + copon.DiscountAmount;
                        }
                        if (copon.FkDiscountTypeId == (int)DiscountTypeId.PercentDiscount)
                        {
                            DiscountPercentage = DiscountPercentage + copon.DiscountAmount;
                            DisountAmount = DisountAmount + ((copon.DiscountAmount * (decimal)goodsProvider.Price) / 100);
                        }
                    }
                }

                goodsProvider.DiscountPercentage = DiscountPercentage;
                goodsProvider.DiscountAmount = DisountAmount;

                return goodsProvider;
            }
            catch (System.Exception)
            {
                return goodsProvider;
            }
        }

        public async Task<RepRes<bool>> GoodsEdit(TGoods goods, bool? goodsProviderIsAccepted, int goodsProviderId)
        {
            try
            {
                var data = await _context.TGoods.Include(x => x.TGoodsProvider).FirstOrDefaultAsync(x => x.GoodsId == goods.GoodsId);

                var canEdit = false;
                if (token.Rule == UserGroupEnum.Seller)
                {
                    if (data.FkOwnerId == token.Id)
                    {
                        canEdit = true;
                    }
                    else
                    {
                        if (data.IsCommonGoods == true)
                        {
                            canEdit = false;
                        }
                        else
                        {
                            return new RepRes<bool>(Message.ThisGoodsIsNotForYou, false, false);
                        }
                    }
                }
                else
                {
                    canEdit = true;
                }

                goods.LastUpdateDateTime = DateTime.Now;
                if (canEdit == true)
                {
                    if (data.HaveVariation != goods.HaveVariation)
                    {
                        if (data.TGoodsProvider.Count > 0)
                        {
                            return new RepRes<bool>(Message.FirstGoDeleteOldGoodsProvider, false, false);
                        }
                    }
                    goods.Title = JsonExtensions.JsonEdit(goods.Title, data.Title, header);
                    _context.Entry(data).CurrentValues.SetValues(goods);
                    _context.Entry(data).Property(x => x.LikedCount).IsModified = false;
                    _context.Entry(data).Property(x => x.ReturnedCount).IsModified = false;
                    _context.Entry(data).Property(x => x.SaleCount).IsModified = false;
                    _context.Entry(data).Property(x => x.SurveyScore).IsModified = false;
                    _context.Entry(data).Property(x => x.ViewCount).IsModified = false;
                    _context.Entry(data).Property(x => x.FkOwnerId).IsModified = false;
                    _context.Entry(data).Property(x => x.IsCommonGoods).IsModified = false;
                    _context.Entry(data).Property(x => x.HaveVariation).IsModified = false;
                    _context.Entry(data).Property(x => x.RegisterDate).IsModified = false;
                    _context.Entry(data).Property(x => x.Description).IsModified = false;
                    _context.Entry(data).Property(x => x.SurveyCount).IsModified = false;
                    _context.Entry(data).Property(x => x.MetaDescription).IsModified = false;
                    _context.Entry(data).Property(x => x.MetaTitle).IsModified = false;
                    _context.Entry(data).Property(x => x.MetaKeywords).IsModified = false;
                    _context.Entry(data).Property(x => x.PageTitle).IsModified = false;
                    _context.Entry(data).Property(x => x.GoodsCode).IsModified = false;
                    var ChangeCategory = false;
                    if (data.FkCategoryId != goods.FkCategoryId)
                    {
                        ChangeCategory = true;
                    }
                    if (ChangeCategory)
                    {
                        var categoryIdsOld = await _categoryRepository.GetParentCatIds(data.FkCategoryId);
                        var categoryIdsNew = await _categoryRepository.GetParentCatIds(goods.FkCategoryId);
                        categoryIdsOld.Where(x => !categoryIdsNew.Contains(x));
                        if (categoryIdsOld.Count() > 0)
                        {
                            var specIds = await _context.TCategorySpecification
                            .AsNoTracking()
                            .Where(x => categoryIdsOld.Contains(x.FkCategoryId))
                            .Select(x => x.FkSpecId)
                            .ToListAsync();

                            var goodsSpecs = await _context.TGoodsSpecification
                            .Where(x => specIds.Contains(x.FkSpecId) && x.FkGoodsId == goods.GoodsId).ToListAsync();
                            var specOpIds = goodsSpecs.Select(x => x.Gsid).ToList();
                            var GoodsspecOp = await _context.TGoodsSpecificationOptions.Where(x => specOpIds.Contains(x.FkGsid)).ToListAsync();

                            _context.TGoodsSpecification.RemoveRange(goodsSpecs);
                            _context.TGoodsSpecificationOptions.RemoveRange(GoodsspecOp);
                        }
                    }
                }

                if (goods.HaveVariation == false)
                {
                    if (goods.TGoodsProvider != null)
                    {
                        foreach (var item in goods.TGoodsProvider)
                        {
                            var rate = (decimal)1.00;
                            if (header.CurrencyNum != CurrencyEnum.TMN)
                            {
                                var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                                rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                            }
                            item.Price = item.Price / rate;
                            var existProvider = data.TGoodsProvider.FirstOrDefault(x => x.FkGoodsId == goods.GoodsId && x.FkShopId == item.FkShopId);
                            if (existProvider == null)
                            {
                                if (item.InventoryCount > 0 && !goods.IsDownloadable)
                                {
                                    item.HasInventory = true;
                                    var opration = new TStockOperation();
                                    opration.FkOperationTypeId = (int)StockOperationTypeEnum.Import;
                                    opration.FkStockId = item.ProviderId;
                                    opration.OperationDate = DateTime.Now;
                                    opration.OperationStockCount = (double)item.InventoryCount;
                                    opration.SaleUnitPrice = item.Price;
                                    opration.OperationComment = "ورود " + item.InventoryCount + " عدد " + goods.Title + " به انبار";
                                    item.TStockOperation = new List<TStockOperation>();
                                    item.TStockOperation.Add(opration);
                                }
                                if (goods.IsDownloadable)
                                {
                                    item.HasInventory = true;
                                }
                                await _context.TGoodsProvider.AddAsync(item);
                            }
                            else
                            {
                                _context.Entry(existProvider).CurrentValues.SetValues(item);
                                _context.Entry(existProvider).Property(x => x.HasInventory).IsModified = false;
                                _context.Entry(existProvider).Property(x => x.InventoryCount).IsModified = false;
                            }
                            var price = await this.CalculateDiscountSpecialSell(item, data);
                            item.DiscountAmount = price.DiscountAmount;
                            item.DiscountPercentage = price.DiscountPercentage;
                            var shop = await _context.TShop.Include(x => x.FkCountry).AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == item.FkShopId);
                            if (shop.GoodsIncludedVat)
                            {
                                if (item.Vatfree == true)
                                {
                                    item.Vatamount = 0;
                                }
                                else
                                {
                                    item.Vatamount = ((item.Price - item.DiscountAmount) * shop.FkCountry.Vat) / 100;
                                }
                            }
                            else
                            {
                                item.Vatfree = true;
                                item.Vatamount = 0;
                            }
                        }
                    }

                }

                await _context.SaveChangesAsync();

                if (token.Rule == UserGroupEnum.Admin)
                {

                    var goodsProvicerData = await _context.TGoodsProvider.Where(c => c.FkShopId == goodsProviderId && c.FkGoodsId == goods.GoodsId).ToListAsync();
                    var shodUserId = await _context.TUser.FirstOrDefaultAsync(c => c.FkShopId == goodsProviderId);
                    var sendMessage = false;
                    var activeOrDeactive = (bool)goodsProviderIsAccepted ? "فعال" : "غیرفعال";
                    var goodsTitle = JsonExtensions.JsonGet(goods.Title, header);
                    foreach (var item in goodsProvicerData)
                    {
                        if (!sendMessage && item.IsAccepted != (bool)goodsProviderIsAccepted && shodUserId != null)
                        {
                            sendMessage = true;
                            await _messageRepository.SendMessageToVendor("تامین کننده گرامی کالای " + goodsTitle + " " + " با کد " + goods.GoodsCode + " توسط ادمین " + activeOrDeactive + " شد "
                            , "تغییر وضعیت کالا", shodUserId.UserId);

                        }
                        item.IsAccepted = (bool)goodsProviderIsAccepted;

                    }
                    await _context.SaveChangesAsync();

                }

                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.GoodsEditing, false, false);
            }
        }

        public async Task<bool> ChangeAccept(List<AcceptNullDto> accept)
        {
            try
            {
                var goodsIds = new List<int>();
                foreach (var item in accept)
                {
                    goodsIds.Add(item.Id);
                }
                var data = await _context.TGoods.Include(t=>t.TGoodsProvider).Where(x => goodsIds.Contains(x.GoodsId)).ToListAsync();
                var acceptData = new List<bool>();
                for (int i = 0; i < data.Count; i++)
                {
                    acceptData.Add((bool) data[i].IsAccepted);
                    if (accept[0].Accept == 0)
                    {
                        data[i].IsAccepted = false;

                    }
                    else if (accept[0].Accept == 1)
                    {
                        data[i].IsAccepted = true;
                    }
                    else
                    {
                        data[i].IsAccepted = null;
                    }
                    data[i].LastUpdateDateTime = DateTime.Now;
                }
                if (accept[0].Accept != 1)
                {
                    var items = await _context.TOrderItem
                    .Include(x => x.FkOrder).ThenInclude(t => t.TOrderItem)
                    .Include(x => x.FkOrder).ThenInclude(b => b.TPaymentTransaction)
                    .Where(x => goodsIds.Contains(x.FkGoodsId) && x.FkOrder.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                     x.FkStatusId == (int)OrderStatusEnum.Cart).ToArrayAsync();
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].FkOrder.TOrderItem.Count > 1)
                        {
                            items[i].FkOrder = null;
                            _context.TOrderItem.Remove(items[i]);
                        }
                        else
                        {
                            _context.TOrderItem.RemoveRange(items[i].FkOrder.TOrderItem);
                            _context.TPaymentTransaction.RemoveRange(items[i].FkOrder.TPaymentTransaction);
                            _context.TOrder.Remove(items[i].FkOrder);
                        }

                    }
                }
                await _context.SaveChangesAsync();


                for (int i = 0; i < data.Count; i++)
                {
                    var accStatus = accept[0].Accept == 0 ? false : (accept[0].Accept == 1 ? true : false) ;
                    if (accStatus != acceptData[i])
                    {
                        var goodsTitle = JsonExtensions.JsonGet(data[i].Title, header);
                        var accStatusText = accStatus ? " فعال " : " غیرفعال ";
                        var goodsProvider = data[i].TGoodsProvider.ToList();
                        for (int j = 0; j < goodsProvider.Count; j++)
                        {
                            var shodUserId = await _context.TUser.FirstOrDefaultAsync(c => c.FkShopId == goodsProvider[j].FkShopId);
                            await _messageRepository.SendMessageToVendor("تامین کننده گرامی کالای " + goodsTitle + " " + " با کد " + data[i].GoodsCode + " توسط ادمین " + accStatusText + " شد "
                           ,  "تغییر وضعیت کالا" , shodUserId.UserId);

                        }
                    }
                }


                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangeProviderToBeDisplay(List<AcceptNullDto> accept)
        {
            try
            {
                var goodsIds = new List<int>();
                foreach (var item in accept)
                {
                    goodsIds.Add(item.Id);
                }
                var data = await _context.TGoodsProvider.Where(x => goodsIds.Contains(x.FkGoodsId) && x.FkShopId == token.Id).ToListAsync();
                for (int i = 0; i < data.Count; i++)
                {
                    if (accept[0].Accept == 0)
                    {
                        data[i].ToBeDisplayed = false;
                    }
                    else if (accept[0].Accept == 1)
                    {
                        data[i].ToBeDisplayed = true;
                    }

                }
                if (accept[0].Accept != 1)
                {
                    var items = await _context.TOrderItem
                    .Include(x => x.FkOrder).ThenInclude(t => t.TOrderItem)
                    .Include(x => x.FkOrder).ThenInclude(b => b.TPaymentTransaction)
                    .Include(x => x.FkVariety)
                    .Where(x => goodsIds.Contains(x.FkVariety.FkGoodsId) && x.FkVariety.FkShopId == token.Id && x.FkOrder.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                     x.FkStatusId == (int)OrderStatusEnum.Cart).ToArrayAsync();
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].FkOrder.TOrderItem.Count > 1)
                        {
                            items[i].FkOrder = null;
                            _context.TOrderItem.Remove(items[i]);
                        }
                        else
                        {
                            _context.TOrderItem.RemoveRange(items[i].FkOrder.TOrderItem);
                            _context.TPaymentTransaction.RemoveRange(items[i].FkOrder.TPaymentTransaction);
                            _context.TOrder.Remove(items[i].FkOrder);
                        }

                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<TGoods>> GoodsDelete(int goodsId)
        {
            try
            {
                var canDelete = await _context.TOrderItem.AsNoTracking().AnyAsync(x => x.FkGoodsId == goodsId);
                if (canDelete)
                {
                    return new RepRes<TGoods>(Message.GoodsCantDelete, false, null);
                }
                var images = await _context.TGoodsDocument.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsDiscount = await _context.TDiscountGoods.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsVarity = await _context.TGoodsVariety.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsSpec = await _context.TGoodsSpecification.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsSpecOp = await _context.TGoodsSpecificationOptions.Where(x => x.FkGsid == goodsId).ToListAsync();
                var goodsProvider = await _context.TGoodsProvider.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsSurvey = await _context.TGoodsSurveyAnswers.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsComment = await _context.TGoodsComment.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsQs = await _context.TGoodsQueAns.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsView = await _context.TGoodsView.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsLike = await _context.TGoodsLike.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goodsStock = await _context.TStockOperation.Include(t => t.FkStock).Where(x => x.FkStock.FkGoodsId == goodsId).ToListAsync();
                var goods = await _context.TGoods.FindAsync(goodsId);

                _context.TDiscountGoods.RemoveRange(goodsDiscount);
                _context.TGoodsVariety.RemoveRange(goodsVarity);
                _context.TGoodsSpecificationOptions.RemoveRange(goodsSpecOp);
                _context.TGoodsSpecification.RemoveRange(goodsSpec);
                _context.TGoodsDocument.RemoveRange(images);
                _context.TStockOperation.RemoveRange(goodsStock);
                _context.TGoodsProvider.RemoveRange(goodsProvider);
                _context.TGoodsSurveyAnswers.RemoveRange(goodsSurvey);
                _context.TGoodsComment.RemoveRange(goodsComment);
                _context.TGoodsQueAns.RemoveRange(goodsQs);
                _context.TGoodsView.RemoveRange(goodsView);
                _context.TGoodsLike.RemoveRange(goodsLike);
                _context.TGoods.Remove(goods);
                await _context.SaveChangesAsync();
                return new RepRes<TGoods>(Message.Successfull, true, goods);
            }
            catch (System.Exception)
            {
                return new RepRes<TGoods>(Message.GoodsDelete, false, null);
            }
        }



        public async Task<RepRes<bool>> DeleteGoodsProvider(int goodsId, int goodsProviderId, int shopId)
        {
            try
            {
                var canDelete = await _context.TOrderItem.AsNoTracking().AnyAsync(x => x.FkGoodsId == goodsId && x.FkVarietyId == goodsProviderId && x.FkShopId == shopId);
                if (canDelete)
                {
                    return new RepRes<bool>(Message.GoodsCantDelete, false, false);
                }
                var callRequests = await _context.TCallRequest.Where(x => x.FkGoodsId == goodsId && x.FkGoodsProviderId == goodsProviderId).ToListAsync();
                var goodsDiscount = await _context.TDiscountGoods.Where(x => x.FkGoodsId == goodsId && x.FkVarietyId == goodsProviderId).ToListAsync();
                var goodsVarity = await _context.TGoodsVariety.Where(x => x.FkGoodsId == goodsId && x.FkProviderId == goodsProviderId).ToListAsync();
                var goodsComment = await _context.TGoodsComment.Where(x => x.FkGoodsId == goodsId && x.FkVarietyId == goodsProviderId).ToListAsync();
                var goodsDocument = await _context.TGoodsDocument.Where(x => x.FkGoodsId == goodsId && x.FkVarietyId == goodsProviderId).ToListAsync();
                var discountFreeGoods = await _context.TDiscountFreeGoods.Where(x => x.FkGoodsId == goodsId && x.FkVarietyId == goodsProviderId).ToListAsync();
                var goodsStock = await _context.TStockOperation.Include(t => t.FkStock).Where(x => x.FkStock.FkGoodsId == goodsId && x.FkStockId == goodsProviderId).ToListAsync();
                var goodsProvider = await _context.TGoodsProvider.FindAsync(goodsProviderId);

                _context.TDiscountGoods.RemoveRange(goodsDiscount);
                _context.TGoodsVariety.RemoveRange(goodsVarity);
                _context.TStockOperation.RemoveRange(goodsStock);
                _context.TGoodsComment.RemoveRange(goodsComment);
                _context.TCallRequest.RemoveRange(callRequests);
                _context.TGoodsDocument.RemoveRange(goodsDocument);
                _context.TDiscountFreeGoods.RemoveRange(discountFreeGoods);
                _context.TGoodsProvider.Remove(goodsProvider);
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.GoodsDelete, false, false);
            }
        }

        public async Task<bool> GoodsShow(int goodsId)
        {
            try
            {
                var data = await _context.TGoods.FindAsync(goodsId);
                data.ToBeDisplayed = !data.ToBeDisplayed;
                data.LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<GoodsBaseDetailDto> GetGoodsBaseDetailDto(int goodsId)
        {
            try
            {
                var data = await _context.TGoods
                .AsNoTracking()
                .Include(x => x.FkBrand)
                .Select(s => new GoodsBaseDetailDto()
                {
                    Title = JsonExtensions.JsonValue(s.Title, header.Language),
                    GoodsId = s.GoodsId,
                    Image = s.ImageUrl,
                    Brand = JsonExtensions.JsonValue(s.FkBrand.BrandTitle, header.Language),
                    CategoryId = s.FkCategoryId,
                    SerialNumber = s.SerialNumber,
                    HaveVariation = s.HaveVariation
                })
                .FirstOrDefaultAsync(x => x.GoodsId == goodsId);

                var CategoryTitle = "";
                var catIds = await _categoryRepository.GetParentCatIds(data.CategoryId);
                foreach (var item in catIds)
                {
                    var title = await _context.TCategory.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == item);
                    CategoryTitle = CategoryTitle + JsonExtensions.JsonGet(title.CategoryTitle, header) + " > ";
                }
                CategoryTitle = CategoryTitle.Substring(0, CategoryTitle.Length - 2);
                data.Category = CategoryTitle;
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> GoodsExist(int goodsId)
        {
            try
            {
                var result = await _context.TGoods.AsNoTracking().AnyAsync(x => x.GoodsId == goodsId);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> GoodsDocumentExist(int imageId)
        {
            try
            {
                var result = await _context.TGoodsDocument.AsNoTracking().AnyAsync(x => x.ImageId == imageId);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<string> GetGoodsDescription(int goodsId)
        {
            try
            {
                var data = await _context.TGoods.Select(x => new { x.GoodsId, x.Description }).AsNoTracking().FirstOrDefaultAsync(x => x.GoodsId == goodsId);
                return JsonExtensions.JsonGet(data.Description, header);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<GoodsDto> GetGoodsById(int goodsId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                if (token.Rule == UserGroupEnum.Seller)
                {
                    return await _context.TGoods.AsNoTracking()
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.TGoodsVariety)
                    .Include(x => x.FkCategory)
                    .Select(x => new GoodsDto()
                    {
                        GoodsId = x.GoodsId,
                        FkCategoryId = x.FkCategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        CategoryPath = x.FkCategory.CategoryPath,
                        FkBrandId = x.FkBrandId,
                        VendorId = x.FkOwnerId,
                        FkUnitId = x.FkUnitId,
                        SerialNumber = x.SerialNumber,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        ImageUrl = x.ImageUrl,
                        Weight = x.Weight,
                        Length = x.Length,
                        Width = x.Width,
                        Heigth = x.Heigth,
                        IsCommonGoods = x.IsCommonGoods,
                        ToBeDisplayed = x.ToBeDisplayed,
                        IsAccepted = x.IsAccepted,
                        IsDownloadable = x.IsDownloadable,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        GoodsOwner = x.FkOwnerId == token.Id ? true : false,
                        DownloadableFileUrl = x.DownloadableFileUrl,
                        GoodsProvider = x.HaveVariation == false ? (x.TGoodsProvider
                        .Select(t => new GoodsProviderDto()
                        {
                            ProviderId = t.ProviderId,
                            FkShopId = t.FkShopId,
                            FkGoodsId = t.FkGoodsId,
                            FkGuaranteeId = t.FkGuaranteeId,
                            HaveGuarantee = t.HaveGuarantee,
                            GuaranteeMonthDuration = t.GuaranteeMonthDuration,
                            Vatfree = t.Vatfree,
                            Price = decimal.Round((decimal)t.Price  / rate, 2, MidpointRounding.AwayFromZero),
                            Vatamount = t.Vatamount == null ? (decimal)0.00 : decimal.Round((decimal)t.Vatamount  / rate, 2, MidpointRounding.AwayFromZero),
                            PostTimeoutDayByShop = t.PostTimeoutDayByShop,
                            ReturningAllowed = t.ReturningAllowed,
                            MaxDeadlineDayToReturning = t.MaxDeadlineDayToReturning,
                            HasInventory = t.HasInventory,
                            InventoryCount = t.InventoryCount,
                            MinimumInventory = (int)t.MinimumInventory,
                            ToBeDisplayed = t.ToBeDisplayed,
                            IsAccepted = t.IsAccepted,
                            PostId = t.PostId
                        })
                        .FirstOrDefault(t => t.FkShopId == token.Id)) : null
                    })
                    .FirstOrDefaultAsync(x => x.GoodsId == goodsId);
                }
                else
                {
                    return await _context.TGoods.AsNoTracking()
                        .Include(x => x.TGoodsProvider).ThenInclude(t => t.TGoodsVariety)
                     .Include(x => x.FkCategory)
                        .Select(x => new GoodsDto()
                        {
                            GoodsId = x.GoodsId,
                            FkCategoryId = x.FkCategoryId,
                            CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                            CategoryPath = x.FkCategory.CategoryPath,
                            FkBrandId = x.FkBrandId,
                            FkUnitId = x.FkUnitId,
                            SerialNumber = x.SerialNumber,
                            Title = JsonExtensions.JsonValue(x.Title, header.Language),
                            ImageUrl = x.ImageUrl,
                            Weight = x.Weight,
                            Length = x.Length,
                            Width = x.Width,
                            Heigth = x.Heigth,
                            IsCommonGoods = x.IsCommonGoods,
                            IsDownloadable = x.IsDownloadable,
                            HaveVariation = x.HaveVariation,
                            DownloadableFileUrl = x.DownloadableFileUrl,
                            ToBeDisplayed = x.ToBeDisplayed,
                            IsAccepted = x.IsAccepted,
                            SaleWithCall = x.SaleWithCall,
                            GoodsProvider = x.HaveVariation == false ? (x.TGoodsProvider
                            .Select(t => new GoodsProviderDto()
                            {
                                ProviderId = t.ProviderId,
                                FkShopId = t.FkShopId,
                                FkGoodsId = t.FkGoodsId,
                                FkGuaranteeId = t.FkGuaranteeId,
                                HaveGuarantee = t.HaveGuarantee,
                                GuaranteeMonthDuration = t.GuaranteeMonthDuration,
                                Vatfree = t.Vatfree,
                                Price = decimal.Round((decimal)t.Price  / rate, 2, MidpointRounding.AwayFromZero),
                                Vatamount = t.Vatamount == null ? (decimal)0.00 : decimal.Round((decimal)t.Vatamount  / rate, 2, MidpointRounding.AwayFromZero),
                                PostTimeoutDayByShop = t.PostTimeoutDayByShop,
                                ReturningAllowed = t.ReturningAllowed,
                                MaxDeadlineDayToReturning = t.MaxDeadlineDayToReturning,
                                HasInventory = t.HasInventory,
                                InventoryCount = t.InventoryCount,
                                MinimumInventory = (int)t.MinimumInventory,
                                ToBeDisplayed = t.ToBeDisplayed,
                                IsAccepted = t.IsAccepted,
                                PostId = t.PostId
                            })
                            .FirstOrDefault(t => x.FkOwnerId == null ? false : x.FkOwnerId == t.FkShopId)) : null
                        })
                        .FirstOrDefaultAsync(x => x.GoodsId == goodsId);
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> CanChangeGoodsShow(List<int> catIds)
        {
            try
            {
                var ExistCategoryStatusFalse = await _context.TCategory.AnyAsync(x => x.IsActive == false && catIds.Contains(x.CategoryId));
                return !ExistCategoryStatusFalse;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public async Task<GoodsDescriptionDto> EditDescription(GoodsDescriptionDto goods)
        {
            try
            {
                var data = await _context.TGoods.FindAsync(goods.GoodsId);
                goods.Description = JsonExtensions.JsonEdit(goods.Description, data.Description, header);
                data.Description = goods.Description;
                data.LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                goods.Description = JsonExtensions.JsonGet(goods.Description, header);
                return goods;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TGoodsDocument> UploadGoodsDocument(TGoodsDocument GoodsDocument)
        {
            try
            {
                await _context.TGoodsDocument.AddAsync(GoodsDocument);
                var data = await _context.TGoods.FindAsync(GoodsDocument.FkGoodsId);
                data.LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return GoodsDocument;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<TGoodsDocument>> GetGoodsDocumentById(int goodsId)
        {
            try
            {
                var data = await _context.TGoodsDocument.Where(x => x.FkGoodsId == goodsId).AsNoTracking().ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<RepRes<TGoodsDocument>> DeleteImageById(int imageId)
        {
            try
            {
                var GoodsDocument = await _context.TGoodsDocument.Include(x => x.FkGoods).FirstOrDefaultAsync(x => x.ImageId == imageId);
                GoodsDocument.FkGoods.LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                _context.TGoodsDocument.Remove(GoodsDocument);
                await _context.SaveChangesAsync();
                return new RepRes<TGoodsDocument>(Message.Successfull, true, GoodsDocument);
            }
            catch (System.Exception)
            {
                return new RepRes<TGoodsDocument>(Message.GoodsDocumentDelete, false, null);
            }
        }

        public async Task<List<GoodsBaseDetailDto>> GetGoodsByCategoryId(List<int> CategoryId, string Filter)
        {
            try
            {
                var GoodsBaseDetail = await _context.TGoods
                .Where(x => CategoryId.Contains(x.FkCategoryId) &&
                (string.IsNullOrWhiteSpace(Filter) ? true : ((x.SerialNumber.Contains(Filter) || JsonExtensions.JsonValue(x.Title, header.Language).Contains(Filter))))
                )
                .OrderByDescending(x => x.GoodsId)
                .Include(x => x.FkCategory)
                .Include(x => x.FkBrand)
                .Select(x => new GoodsBaseDetailDto()
                {
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsId = x.GoodsId,
                    Image = x.ImageUrl,
                    SerialNumber = x.SerialNumber,
                    Brand = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                    Category = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                    CategoryId = x.FkCategoryId
                })
                .AsNoTracking()
                .ToListAsync();

                return GoodsBaseDetail;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsListGetDto>> GetAllGoodsByCategoryId(GoodsPaginationDto pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    var Goods = await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Include(x => x.FkBrand)
                    .Where(x => (pagination.CatChilds != null ? pagination.CatChilds.Contains(x.FkCategoryId) : true)
                    && (pagination.ShopId != 0 ? (x.FkOwnerId == pagination.ShopId || x.TGoodsProvider.Any(t => t.FkShopId == pagination.ShopId)) : true)
                    && (string.IsNullOrEmpty(pagination.Name) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Name)))
                    && (string.IsNullOrEmpty(pagination.Code) ? true : x.GoodsCode.Contains(pagination.Code))
                    && (string.IsNullOrEmpty(pagination.Number) ? true : x.GoodsCode.Contains(pagination.Number))
                    && (pagination.Common == true ? x.IsCommonGoods : true)
                    && (pagination.ProductType == 1 ? x.IsCommonGoods == true : true)
                    && (pagination.PriceFrom == 0 ? true : (x.TGoodsProvider.Any(t => t.Price >= (decimal)pagination.PriceFrom)))
                    && (pagination.BrandId == 0 ? true : x.FkBrand.BrandId == pagination.BrandId)
                    && (pagination.PriceTo == 0 ? true : (x.TGoodsProvider.Any(t => t.Price <= (decimal)pagination.PriceTo)))
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    && (pagination.LastUpdateHoure == 0 ? true : (x.LastUpdateDateTime >= DateTime.Now.AddHours(-pagination.LastUpdateHoure)))
                    )
                    .OrderByDescending(x => x.GoodsId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Select(x => new GoodsListGetDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        RegisterDate = Extentions.PersianDateString(x.RegisterDate),
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        ToBeDisplayed = x.ToBeDisplayed,
                        IsAccepted = x.IsAccepted,
                        CategoryId = x.FkCategoryId,
                        IsCommon = x.IsCommonGoods,
                        Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => new ShopFormDto()
                        {
                            ShopId = v.ShopId,
                            ShopTitle = v.ShopTitle

                        }).ToList()),
                        ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => v.ShopTitle)),

                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    return Goods;
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));
                    var Goods = await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Include(x => x.FkBrand)
                    .Where(x => (pagination.CatChilds != null ? pagination.CatChilds.Contains(x.FkCategoryId) : true)
                    && (pagination.ShopId != 0 ? (x.FkOwnerId == pagination.ShopId || x.IsCommonGoods == true) : true)
                    && (string.IsNullOrEmpty(pagination.Name) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Name)))
                    && (string.IsNullOrEmpty(pagination.Code) ? true : x.GoodsCode.Contains(pagination.Code))
                    && (string.IsNullOrEmpty(pagination.Number) ? true : x.GoodsCode.Contains(pagination.Number))
                    && (pagination.Common == true ? x.IsCommonGoods : true)
                    && (pagination.ProductType == 3 ? x.IsCommonGoods == true : (pagination.ProductType == 1 ? x.FkOwnerId == pagination.ShopId :
                        x.TGoodsProvider.Any(a => a.FkShopId == pagination.ShopId)))
                    && (pagination.PriceFrom == 0 ? true : (x.TGoodsProvider.Any(t => t.Price >= (decimal)pagination.PriceFrom)))
                    && (pagination.BrandId == 0 ? true : x.FkBrand.BrandId == pagination.BrandId)
                    && (pagination.PriceTo == 0 ? true : (x.TGoodsProvider.Any(t => t.Price <= (decimal)pagination.PriceTo)))
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    && (pagination.LastUpdateHoure == 0 ? true : (x.LastUpdateDateTime >= DateTime.Now.AddHours(-pagination.LastUpdateHoure)))
                    )
                    .Include(x => x.FkCategory)
                    .WhereAny(predicates.ToArray())
                    .OrderByDescending(x => x.GoodsId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Select(x => new GoodsListGetDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        RegisterDate = Extentions.PersianDateString(x.RegisterDate),
                        CategoryId = x.FkCategoryId,
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        ToBeDisplayed = x.TGoodsProvider.Any(b => b.ToBeDisplayed == true && b.FkShopId == token.Id),
                        IsAccepted = x.IsAccepted,
                        IsCommon = x.IsCommonGoods,
                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    return Goods;
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetAllGoodsByCategoryIdCount(GoodsPaginationDto pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Include(x => x.FkBrand)
                    .AsNoTracking()
                    .CountAsync(x => (pagination.CatChilds != null ? pagination.CatChilds.Contains(x.FkCategoryId) : true)
                    && (pagination.ShopId != 0 ? (x.FkOwnerId == pagination.ShopId || x.TGoodsProvider.Any(t => t.FkShopId == pagination.ShopId)) : true)
                    && (string.IsNullOrEmpty(pagination.Name) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Name)))
                    && (string.IsNullOrEmpty(pagination.Code) ? true : x.GoodsCode.Contains(pagination.Code))
                    && (string.IsNullOrEmpty(pagination.Number) ? true : x.GoodsCode.Contains(pagination.Number))
                    && (pagination.Common == true ? x.IsCommonGoods : true)
                    && (pagination.ProductType == 1 ? x.IsCommonGoods == true : true)
                    && (pagination.BrandId == 0 ? true : x.FkBrand.BrandId == pagination.BrandId)
                    && (pagination.PriceFrom == 0 ? true : (x.TGoodsProvider.Any(t => t.Price >= (decimal)pagination.PriceFrom)))
                    && (pagination.PriceTo == 0 ? true : (x.TGoodsProvider.Any(t => t.Price <= (decimal)pagination.PriceTo)))
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    && (pagination.LastUpdateHoure == 0 ? true : (x.LastUpdateDateTime >= DateTime.Now.AddHours(-pagination.LastUpdateHoure)))
                    );
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));
                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Include(x => x.FkCategory)
                    .Include(x => x.FkBrand)
                    .WhereAny(predicates.ToArray())
                    .AsNoTracking()
                    .CountAsync(x => (pagination.CatChilds != null ? pagination.CatChilds.Contains(x.FkCategoryId) : true)
                    && (pagination.ShopId != 0 ? (x.FkOwnerId == pagination.ShopId || (x.IsCommonGoods == true)) : true)
                    && (string.IsNullOrEmpty(pagination.Name) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Name)))
                    && (string.IsNullOrEmpty(pagination.Code) ? true : x.GoodsCode.Contains(pagination.Code))
                    && (string.IsNullOrEmpty(pagination.Number) ? true : x.GoodsCode.Contains(pagination.Number))
                    && (pagination.Common == true ? x.IsCommonGoods : true)
                    && (pagination.ProductType == 3 ? x.IsCommonGoods == true : (pagination.ProductType == 1 ? x.FkOwnerId == pagination.ShopId :
                        x.TGoodsProvider.Any(a => a.FkShopId == pagination.ShopId)))
                    && (pagination.BrandId == 0 ? true : x.FkBrand.BrandId == pagination.BrandId)
                    && (pagination.PriceFrom == 0 ? true : (x.TGoodsProvider.Any(t => t.Price >= (decimal)pagination.PriceFrom)))
                    && (pagination.PriceTo == 0 ? true : (x.TGoodsProvider.Any(t => t.Price <= (decimal)pagination.PriceTo)))
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    && (pagination.LastUpdateHoure == 0 ? true : (x.LastUpdateDateTime >= DateTime.Now.AddHours(-pagination.LastUpdateHoure)))
                    );
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> DeleteGoodsSpecificationByGoodsId(int goodsId)
        {
            try
            {
                var data = await _context.TGoodsSpecification.Where(x => x.FkGoodsId == goodsId).ToListAsync();
                var goods = await _context.TGoods.FindAsync(goodsId);
                goods.LastUpdateDateTime = DateTime.Now;
                _context.TGoodsSpecification.RemoveRange(data);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<TGoodsSpecification>> AddGoodsSpecification(List<TGoodsSpecification> goodsSpecs)
        {
            try
            {
                var specs = await _context.TGoodsSpecification.Where(b => b.FkGoodsId == goodsSpecs[0].FkGoodsId).ToArrayAsync();
                var updatedSpec = new List<int>();
                for (int i = 0; i < goodsSpecs.Count; i++)
                {
                    var addSpec = true;
                    foreach (var item in specs)
                    {
                        if (item.FkGoodsId == goodsSpecs[i].FkGoodsId && item.FkSpecId == goodsSpecs[i].FkSpecId && !string.IsNullOrWhiteSpace(item.SpecValueText))
                        {
                            addSpec = false;
                            goodsSpecs[i].SpecValueText = JsonExtensions.JsonEdit(goodsSpecs[i].SpecValueText, item.SpecValueText, header);
                        }
                    }
                    if (addSpec)
                    {
                        goodsSpecs[i].SpecValueText = JsonExtensions.JsonAdd(goodsSpecs[i].SpecValueText, header);
                    }
                }

                _context.TGoodsSpecification.RemoveRange(specs);
                await _context.SaveChangesAsync();
                await _context.TGoodsSpecification.AddRangeAsync(goodsSpecs);
                var goods = await _context.TGoods.FindAsync(goodsSpecs[0].FkGoodsId);
                goods.LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return goodsSpecs;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ShopHasThisGoods(int shopId, int goodsId, bool forShowing)
        {
            try
            {
                return await _context.TGoods.AsNoTracking().AnyAsync(x => (x.FkOwnerId == shopId || (forShowing == true ? x.IsCommonGoods == true : x.IsCommonGoods == false)) && goodsId == x.GoodsId);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<int>> GetGoodsParentCategoryIds(int goodsId)
        {
            try
            {
                var goods = await _context.TGoods.Select(x => new { x.GoodsId, x.FkCategoryId }).FirstOrDefaultAsync(x => x.GoodsId == goodsId);
                var catIds = await _categoryRepository.GetParentCatIds(goods.FkCategoryId);
                catIds.Add(goods.FkCategoryId);
                return catIds.Distinct().ToList();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> AddGoodsProvider(TGoodsProvider goodsProvider, List<int> parameterIds, List<string> fileNames)
        {
            try
            {
                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var goodsExist = await _context.TGoods.AsNoTracking().AnyAsync(x => x.GoodsId == goodsProvider.FkGoodsId && x.HaveVariation == true);
                if (goodsExist == false)
                {
                    return new RepRes<bool>(Message.GoodsProviderAddingAndEditing, false, false);
                }
                if (goodsProvider.ProviderId == 0)
                {
                    if (goodsProvider.TGoodsVariety.Count == 0)
                    {
                        return new RepRes<bool>(Message.ThereIsNoVariety, false, false);
                    }
                    var ExistVariation = await _context.TGoodsProvider.Include(x => x.TGoodsVariety)
                    .AsNoTracking().AnyAsync(x => x.FkGoodsId == goodsProvider.FkGoodsId && x.FkShopId == goodsProvider.FkShopId &&
                    x.TGoodsVariety.All(t => goodsProvider.TGoodsVariety.Select(i => i.FkVariationParameterValueId).ToList().Any(i => i == t.FkVariationParameterValueId)) && x.TGoodsVariety.Count() == goodsProvider.TGoodsVariety.Count);

                    if (ExistVariation == true)
                    {
                        return new RepRes<bool>(Message.ThisVariationIsExist, false, false);
                    }
                    //add
                    goodsProvider.Price = goodsProvider.Price / rate;
                    goodsProvider.IsAccepted = true;
                    if (goodsProvider.Vatamount != null && goodsProvider.Vatfree == false)
                    {
                        goodsProvider.Vatamount = goodsProvider.Vatamount / rate;
                    }
                    else
                    {
                        goodsProvider.Vatamount = 0;
                    }

                    if (goodsProvider.InventoryCount > 0)
                    {
                        goodsProvider.HasInventory = true;
                        var opration = new TStockOperation();
                        opration.FkOperationTypeId = (int)StockOperationTypeEnum.Import;
                        opration.FkStockId = goodsProvider.ProviderId;
                        opration.OperationDate = DateTime.Now;
                        opration.OperationStockCount = (double)goodsProvider.InventoryCount;
                        opration.SaleUnitPrice = goodsProvider.Price;
                        opration.OperationComment = "ورود " + goodsProvider.InventoryCount + " به انبار";
                        goodsProvider.TStockOperation = new List<TStockOperation>();
                        goodsProvider.TStockOperation.Add(opration);
                    }
                    await _context.TGoodsProvider.AddAsync(goodsProvider);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // edit
                    var oldData = await _context.TGoodsProvider.Include(x => x.TGoodsVariety).FirstOrDefaultAsync(x => x.ProviderId == goodsProvider.ProviderId && x.FkShopId == goodsProvider.FkShopId);
                    if (oldData == null)
                    {
                        return new RepRes<bool>(Message.GoodsProviderNotFound, false, false);
                    }

                    goodsProvider.Price = goodsProvider.Price / rate;
                    if (goodsProvider.Vatamount != null && goodsProvider.Vatfree == false)
                    {
                        goodsProvider.Vatamount = goodsProvider.Vatamount / rate;
                    }
                    else
                    {
                        goodsProvider.Vatamount = 0;
                    }
                    for (int i = 0; i < fileNames.Count; i++)
                    {
                        foreach (var item in goodsProvider.TGoodsVariety.Where(x => x.FkVariationParameterValueId == parameterIds[i]))
                        {
                            item.ImageUrl = fileNames[i];
                        }
                        foreach (var item in oldData.TGoodsVariety.Where(x => x.FkVariationParameterValueId == parameterIds[i]))
                        {
                            item.ImageUrl = fileNames[i];
                        }
                    }
                    if (oldData.TGoodsVariety.Count > goodsProvider.TGoodsVariety.Count)
                    {
                        return new RepRes<bool>(Message.YouCantDeleteVarityParameterValue, false, false);
                    }
                    else if (oldData.TGoodsVariety.Count == goodsProvider.TGoodsVariety.Count)
                    {
                        if (!oldData.TGoodsVariety.Select(x => x.FkVariationParameterValueId).ToList().All(x => goodsProvider.TGoodsVariety.Any(y => x == y.FkVariationParameterValueId)))
                        {
                            return new RepRes<bool>(Message.YouCantChangeVarityParameterValue, false, false);
                        }
                    }
                    else
                    {
                        var ExistVariation = await _context.TGoodsProvider.Include(x => x.TGoodsVariety)
                        .AsNoTracking().AnyAsync(x => x.FkGoodsId == goodsProvider.FkGoodsId && x.FkShopId == goodsProvider.FkShopId &&
                        x.TGoodsVariety.All(t => goodsProvider.TGoodsVariety.Select(i => i.FkVariationParameterValueId).ToList().Any(i => i == t.FkVariationParameterValueId)) && x.TGoodsVariety.Count() == goodsProvider.TGoodsVariety.Count);

                        if (ExistVariation == true)
                        {
                            return new RepRes<bool>(Message.ThisVariationIsExist, false, false);
                        }
                        foreach (var item in goodsProvider.TGoodsVariety.Where(x => x.VarietyId == 0).ToList())
                        {
                            item.FkProviderId = goodsProvider.ProviderId;
                            item.FkGoodsId = goodsProvider.FkGoodsId;
                        }
                        await _context.TGoodsVariety.AddRangeAsync(goodsProvider.TGoodsVariety.Where(x => x.VarietyId == 0).ToList());
                    }
                    await _context.SaveChangesAsync();
                    goodsProvider.TGoodsVariety = null;
                    _context.Entry(oldData).CurrentValues.SetValues(goodsProvider);
                    _context.Entry(oldData).Property(x => x.HasInventory).IsModified = false;
                    _context.Entry(oldData).Property(x => x.InventoryCount).IsModified = false;
                    _context.Entry(oldData).Property(x => x.IsAccepted).IsModified = false;

                    await _context.SaveChangesAsync();
                }


                var updatedProvider = await _context.TGoodsProvider.FirstOrDefaultAsync(x => x.ProviderId == goodsProvider.ProviderId && x.FkShopId == goodsProvider.FkShopId);
                var goods = await _context.TGoods.FindAsync(goodsProvider.FkGoodsId);
                goods.LastUpdateDateTime = DateTime.Now;
                var price = await this.CalculateDiscountSpecialSell(updatedProvider, goods);
                updatedProvider.DiscountAmount = price.DiscountAmount;
                updatedProvider.DiscountPercentage = price.DiscountPercentage;
                var shop = await _context.TShop.Include(x => x.FkCountry).AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == updatedProvider.FkShopId);
                if (shop.GoodsIncludedVat)
                {
                    if (updatedProvider.Vatfree == true)
                    {
                        updatedProvider.Vatamount = 0;
                    }
                    else
                    {
                        updatedProvider.Vatamount = ((updatedProvider.Price - updatedProvider.DiscountAmount) * shop.FkCountry.Vat) / 100;
                    }
                }
                else
                {
                    updatedProvider.Vatfree = true;
                    updatedProvider.Vatamount = 0;
                }
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.GoodsProviderAddingAndEditing, false, false);
            }
        }

        public async Task<List<GoodsProviderGetDto>> GetGoodsProvider(int goodsId, int shopId)
        {
            try
            {
                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TGoodsProvider
                .Include(x => x.FkGoods)
                .Where(x => x.FkGoodsId == goodsId && x.FkShopId == shopId && x.FkGoods.HaveVariation == true)
                .Include(x => x.FkGuarantee)
                .Include(x => x.TGoodsVariety).ThenInclude(t => t.FkVariationParameter)
                .Include(x => x.TGoodsVariety).ThenInclude(t => t.FkVariationParameterValue)
                .Select(x => new GoodsProviderGetDto()
                {
                    ProviderId = x.ProviderId,
                    FkShopId = x.FkShopId,
                    FkGoodsId = x.FkGoodsId,
                    FkGuaranteeId = x.FkGuaranteeId,
                    GuaranteeTitle = JsonExtensions.JsonValue(x.FkGuarantee.GuaranteeTitle, header.Language),
                    HaveGuarantee = x.HaveGuarantee,
                    GuaranteeMonthDuration = x.GuaranteeMonthDuration,
                    Vatfree = x.Vatfree,
                    Price = decimal.Round((decimal)x.Price  / rate, 2, MidpointRounding.AwayFromZero),
                    Vatamount = x.Vatamount == null ? (decimal)0.00 : decimal.Round((decimal)x.Vatamount  / rate, 2, MidpointRounding.AwayFromZero),
                    PostTimeoutDayByShop = x.PostTimeoutDayByShop,
                    ReturningAllowed = x.ReturningAllowed,
                    MaxDeadlineDayToReturning = x.MaxDeadlineDayToReturning,
                    HasInventory = x.HasInventory,
                    InventoryCount = x.InventoryCount,
                    MinimumInventory = (int)x.MinimumInventory,
                    ToBeDisplayed = x.ToBeDisplayed,
                    IsAccepted = x.IsAccepted,
                    TGoodsVariety = x.TGoodsVariety.Select(t => new GoodsVarietyGetDto()
                    {
                        VarietyId = t.VarietyId,
                        FkProviderId = t.FkProviderId,
                        FkGoodsId = t.FkGoodsId,
                        FkVariationParameterId = t.FkVariationParameterId,
                        ParameterTitle = JsonExtensions.JsonValue(t.FkVariationParameter.ParameterTitle, header.Language),
                        FkVariationParameterValueId = t.FkVariationParameterValueId,
                        ValueTitle = JsonExtensions.JsonValue(t.FkVariationParameterValue.Value, header.Language),
                        ImageUrl = t.ImageUrl,
                    }).ToList()
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsBaseDetailDto>> GoodsGetByIds(List<int> ids)
        {
            try
            {
                var goods = await _context.TGoods.Where(x => ids.Count > 0 ? ids.Contains(x.GoodsId) : true)
                .Include(x => x.FkCategory)
                .Select(x => new GoodsBaseDetailDto()
                {
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsId = x.GoodsId,
                    Image = x.ImageUrl,
                    SerialNumber = x.SerialNumber,
                    Brand = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                    Category = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                    CategoryId = x.FkCategoryId
                })
                .ToListAsync();
                return goods;
            }
            catch (System.Exception)
            {
                return null;
            }
        }





        public async Task<GoodsMetaDto> EditGoodsMeta(GoodsMetaDto goodsMeta)
        {
            try
            {
                var data = await _context.TGoods.FindAsync(goodsMeta.GoodsId);
                goodsMeta.MetaDescription = JsonExtensions.JsonEdit(goodsMeta.MetaDescription, data.MetaDescription, header);
                goodsMeta.MetaKeywords = JsonExtensions.JsonEdit(goodsMeta.MetaKeywords, data.MetaKeywords, header);
                goodsMeta.MetaTitle = JsonExtensions.JsonEdit(goodsMeta.MetaTitle, data.MetaTitle, header);
                goodsMeta.PageTitle = JsonExtensions.JsonEdit(goodsMeta.PageTitle, data.PageTitle, header);
                data.MetaDescription = goodsMeta.MetaDescription;
                data.MetaKeywords = goodsMeta.MetaKeywords;
                data.MetaTitle = goodsMeta.MetaTitle;
                data.PageTitle = goodsMeta.PageTitle;
                data.LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                goodsMeta.MetaDescription = JsonExtensions.JsonGet(goodsMeta.MetaDescription, header);
                goodsMeta.MetaKeywords = JsonExtensions.JsonGet(goodsMeta.MetaKeywords, header);
                goodsMeta.MetaTitle = JsonExtensions.JsonGet(goodsMeta.MetaTitle, header);
                goodsMeta.PageTitle = JsonExtensions.JsonGet(goodsMeta.PageTitle, header);
                return goodsMeta;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<GoodsMetaDto> GetGoodsMeta(int goodsId)
        {
            try
            {
                var data = await _context.TGoods.Where(x => x.GoodsId == goodsId)
                .Select(x => new GoodsMetaDto()
                {
                    MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                    MetaKeywords = JsonExtensions.JsonValue(x.MetaKeywords, header.Language),
                    MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                    PageTitle = JsonExtensions.JsonValue(x.PageTitle, header.Language),
                    GoodsId = x.GoodsId
                }).FirstOrDefaultAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<NoVariationGoodsProviderGetDto> GetNoVariationGoodsProvider(int goodsId, int shopId)
        {
            try
            {
                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var data = new NoVariationGoodsProviderGetDto();
                data.GoodsProvider = await _context.TGoodsProvider
                .Include(x => x.FkGuarantee)
                .Include(x => x.FkGoods)
                .Select(x => new GoodsProviderGetDto()
                {
                    ProviderId = x.ProviderId,
                    FkShopId = x.FkShopId,
                    FkGoodsId = x.FkGoodsId,
                    FkGuaranteeId = x.FkGuaranteeId,
                    GuaranteeTitle = JsonExtensions.JsonValue(x.FkGuarantee.GuaranteeTitle, header.Language),
                    HaveGuarantee = x.HaveGuarantee,
                    GuaranteeMonthDuration = x.GuaranteeMonthDuration,
                    Vatfree = x.Vatfree,
                    Price = decimal.Round((decimal)x.Price  / rate, 2, MidpointRounding.AwayFromZero),
                    Vatamount = x.Vatamount == null ? (decimal)0.00 : decimal.Round((decimal)x.Vatamount  / rate, 2, MidpointRounding.AwayFromZero),
                    PostTimeoutDayByShop = x.PostTimeoutDayByShop,
                    ReturningAllowed = x.ReturningAllowed,
                    MaxDeadlineDayToReturning = x.MaxDeadlineDayToReturning,
                    HasInventory = x.HasInventory,
                    InventoryCount = x.InventoryCount,
                    MinimumInventory = (int)x.MinimumInventory,
                    ToBeDisplayed = x.ToBeDisplayed,
                    IsAccepted = x.IsAccepted,
                    HaveVariation = x.FkGoods.HaveVariation
                }).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == shopId && x.FkGoodsId == goodsId && x.HaveVariation == false);
                var provider = await _context.TGoodsProvider.AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == shopId && x.FkGoodsId == goodsId);
                if (provider != null)
                {
                    data.IsAccepted = provider.IsAccepted;
                }
                else
                {
                    data.IsAccepted = false;
                }
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> CanShopAddGoods(int shopId, int categoryId)
        {
            try
            {
                var shop = await _context.TShop.AsNoTracking().Select(x => new { x.ShopId, x.FkPlanId, x.MaxProduct, x.ExpirationDate, x.FkStatusId }).FirstOrDefaultAsync(x => x.ShopId == shopId);
                if (shop.FkStatusId != (int)ShopStatusEnum.Active)
                {
                    return new RepRes<bool>(Message.ShopIsNotActiveYouCantAddGoods, false, false);
                }
                if (shop.FkPlanId == null)
                {
                    return new RepRes<bool>(Message.FirstGoAndChooseYourPlanToAddGoods, false, false);
                }
                if (shop.ExpirationDate.Value.Date < DateTime.Now.Date)
                {
                    return new RepRes<bool>(Message.YourPlanExpire, false, false);
                }
                if (shop.MaxProduct != null)
                {
                    var shopGoodsCount = await _context.TGoods.AsNoTracking().CountAsync(x => x.FkOwnerId == shopId);
                    if (shopGoodsCount >= shop.MaxProduct)
                    {
                        return new RepRes<bool>(Message.YourCapacityOfAddGoodsInPlanIsOver, false, false);
                    }
                }

                return new RepRes<bool>(Message.Successfull, true, true);

            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.GoodsAdding, false, false);
            }
        }

        public async Task<bool> AcceptShopGoodsAdding()
        {
            try
            {
                return await _context.TSetting.AsNoTracking().Select(x => x.SysAutoActiveGoods).FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> GetGoodsIncludeVat(int shopId)
        {
            try
            {
                return await _context.TShop.AsNoTracking().AnyAsync(x => x.ShopId == shopId && x.GoodsIncludedVat == true);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> GoodsGroupEditing(GoodsGroupEditingDto goodsGroupEditing, int shopId)
        {
            try
            {
                var goodsList = new List<TGoods>();
                if(goodsGroupEditing.ProductsId.Count > 0) {
                 goodsList = await _context.TGoods.Where
                (c => goodsGroupEditing.ProductsId.Any(v=>v == c.GoodsId) &&
                   (shopId != 0 ? c.FkOwnerId == shopId : true))
                                .Include(v => v.TGoodsProvider).ToListAsync();
                } else {
                 goodsList = await _context.TGoods.Where
                (c => c.FkCategoryId == goodsGroupEditing.CategoryId &&
                   (shopId != 0 ? c.FkOwnerId == shopId : true))
                                .Include(v => v.TGoodsProvider).ToListAsync();
                }

                foreach (var goods in goodsList)
                {
                    if (goodsGroupEditing.GoodsFieldWeight)
                    {

                        if (goodsGroupEditing.OperationTypeIncrease)
                        {
                            if (goodsGroupEditing.ValueTypeFixed)
                            {
                                goods.Weight = goods.Weight + goodsGroupEditing.Value;
                            }
                            else
                            {
                                if (goodsGroupEditing.Value > 100)
                                {
                                    goodsGroupEditing.Value = 100;
                                }
                                var number = (goods.Weight * goodsGroupEditing.Value) / 100;
                                goods.Weight = goods.Weight + number;

                            }
                        }
                        else
                        {
                            if (goodsGroupEditing.ValueTypeFixed)
                            {
                                goods.Weight = goods.Weight - goodsGroupEditing.Value;
                                if (goods.Weight < 0)
                                {
                                    goods.Weight = 0;
                                }
                            }
                            else
                            {
                                if (goodsGroupEditing.Value > 100)
                                {
                                    goodsGroupEditing.Value = 100;
                                }
                                var number = (goods.Weight * goodsGroupEditing.Value) / 100;
                                goods.Weight = goods.Weight - number;
                                if (goods.Weight < 0)
                                {
                                    goods.Weight = 0;
                                }
                            }
                        }

                    }
                    if (goodsGroupEditing.GoodsFieldPrice)
                    {

                        foreach (var item in goods.TGoodsProvider)
                        {

                            if (goodsGroupEditing.OperationTypeIncrease)
                            {
                                if (goodsGroupEditing.ValueTypeFixed)
                                {
                                    item.Price = item.Price + goodsGroupEditing.Value;
                                }
                                else
                                {
                                    if (goodsGroupEditing.Value > 100)
                                    {
                                        goodsGroupEditing.Value = 100;
                                    }
                                    var number = (item.Price * goodsGroupEditing.Value) / 100;
                                    item.Price = item.Price + number;

                                }
                            }
                            else
                            {
                                if (goodsGroupEditing.ValueTypeFixed)
                                {
                                    item.Price = item.Price - goodsGroupEditing.Value;
                                    if (item.Price < 0)
                                    {
                                        item.Price = 0;
                                    }
                                }
                                else
                                {
                                    if (goodsGroupEditing.Value > 100)
                                    {
                                        goodsGroupEditing.Value = 100;
                                    }
                                    var number = (item.Price * goodsGroupEditing.Value) / 100;
                                    item.Price = item.Price - number;
                                    if (item.Price < 0)
                                    {
                                        item.Price = 0;
                                    }
                                }
                            }

                        }

                    }
                }

                await _context.SaveChangesAsync();
                return true;

            }
            catch (System.Exception)
            {
                return false;
            }
        }


    }
}
