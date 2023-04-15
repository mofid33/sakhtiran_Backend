using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.WareHouse;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class WareHouseRepository : IWareHouseRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }


        public WareHouseRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<List<WareHouseOprationListDto>> GetWareHouseOprationList(WareHouseOprationListPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsProvider
                .Include(x => x.FkGoods)
                .Where(x =>
                (pagination.CategoryId == 0 ? true : x.FkGoods.FkCategoryId == pagination.CategoryId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId)
                )
                .OrderByDescending(x => x.ProviderId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkGoods).ThenInclude(t => t.FkCategory)
                .Include(x => x.FkShop)
                .Select(x => new WareHouseOprationListDto()
                {
                    CategoryId = x.FkGoods.FkCategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.FkGoods.FkCategory.CategoryTitle, header.Language),
                    FkGoodsId = x.FkGoodsId,
                    FkShopId = x.FkShopId,
                    GoodsCode = x.FkGoods.GoodsCode,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    HasInventory = x.HasInventory,
                    ImageUrl = x.FkGoods.ImageUrl,
                    InventoryCount = x.InventoryCount,
                    ProviderId = x.ProviderId,
                    SerialNumber = x.FkGoods.SerialNumber,
                    ShopTitle = x.FkShop.StoreName,
                    Varity = x.TGoodsVariety.Select(t => new GoodsVarietyGetDto()
                    {
                        FkGoodsId = t.FkGoodsId,
                        FkProviderId = t.FkProviderId,
                        FkVariationParameterId = t.FkVariationParameterId,
                        FkVariationParameterValueId = t.FkVariationParameterValueId,
                        ImageUrl = t.ImageUrl,
                        ParameterTitle = JsonExtensions.JsonValue(t.FkVariationParameter.ParameterTitle, header.Language),
                        ValueTitle = JsonExtensions.JsonValue(t.FkVariationParameterValue.Value, header.Language),
                        VarietyId = t.VarietyId,
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> AddWareHouseOperation(WareHouseOprationAddDto opration)
        {
            try
            {
                var goodsprovider = await _context.TGoodsProvider.FirstAsync(x=>x.ProviderId == opration.GoodsProviderId&& x.FkShopId == token.Id);
                if(opration.In == false)
                {
                    if(goodsprovider.InventoryCount<opration.OperationStockCount)
                    {
                        return new RepRes<bool>(Message.InventoryCountIsLower,false,false);
                    }
                }
                var type = opration.In == true ? (int)StockOperationTypeEnum.Import : (int)StockOperationTypeEnum.Export;
                var result = await this.AddStockOpration(type,opration.GoodsProviderId,null,opration.OperationStockCount,null,opration.OperationComment);
                if(result == false)
                {
                    return new RepRes<bool>(Message.WareHouseOprationAdd,false,false);
                }
                return new RepRes<bool>(Message.Successfull,true,true);
                
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.WareHouseOprationAdd,false,false);
            }
        }

        public async Task<int> GetWareHouseOprationListCount(WareHouseOprationListPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsProvider
                .Include(x => x.FkGoods)
                .AsNoTracking()
                .CountAsync(x =>
                (pagination.CategoryId == 0 ? true : x.FkGoods.FkCategoryId == pagination.CategoryId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<WareHouseOprationDetailDto>> GetWareHouseOperationDetail(PaginationFormDto pagination)
        {
            try
            {
                var rate = (decimal) 1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.0 : (decimal) currency.RatesAgainstOneDollar;
                }
                return await _context.TStockOperation.Where(x => x.FkStockId == pagination.Id)
                .OrderByDescending(x => x.OperationId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x=>x.FkOperationType)
                .Select(x => new WareHouseOprationDetailDto()
                {
                    Balance = Extentions.CalculateBalance((_context.TStockOperation.Where(t =>t.FkStockId== x.FkStockId && (t.FkOperationTypeId == (int)StockOperationTypeEnum.Import || t.FkOperationTypeId == (int)StockOperationTypeEnum.SaleReturn) && t.OperationId <= x.OperationId).Sum(t => t.OperationStockCount)),(_context.TStockOperation.Where(t =>t.FkStockId== x.FkStockId &&  (t.FkOperationTypeId == (int)StockOperationTypeEnum.Export || t.FkOperationTypeId == (int)StockOperationTypeEnum.PurchaseReturn)  && t.OperationId <= x.OperationId).Sum(t => t.OperationStockCount)))   ,
                    FkOperationTypeId = x.FkOperationTypeId,
                    OperationTypeTitle = JsonExtensions.JsonValue(x.FkOperationType.OperationTypeTitle,header.Language),
                    FkOrderItem = x.FkOrderItem,
                    FkStockId = x.FkStockId,
                    OperationComment = x.OperationComment,
                    OperationDate = Extentions.PersianDateString(x.OperationDate),
                    OperationId = x.OperationId,
                    OperationStockCount = x.OperationStockCount,
                    SaleUnitPrice = decimal.Round((decimal) x.SaleUnitPrice  / rate, 2, MidpointRounding.AwayFromZero)
                })
                .AsNoTracking().ToListAsync();

            }
            catch (System.Exception)
            {
                return null;
            }
        }



        public async Task<int> GetWareHouseOperationDetailCount(PaginationFormDto pagination)
        {
            try
            {
                return await _context.TStockOperation.AsNoTracking().CountAsync(x => x.FkStockId == pagination.Id);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> AddStockOpration(int fkOperationTypeId, int fkStockId, long? fkOrderItem,  double operationStockCount, decimal? saleUnitPrice, string operationComment)
        {
            try
            {
                var operation = new TStockOperation();
                operation.OperationDate = DateTime.Now;
                operation.FkOperationTypeId = fkOperationTypeId;
                operation.FkStockId = fkStockId;
                operation.FkOrderItem = fkOrderItem;
                operation.OperationDate = DateTime.Now;
                operation.OperationStockCount = operationStockCount;
                operation.SaleUnitPrice = saleUnitPrice;
                operation.OperationComment = operationComment;
                await _context.TStockOperation.AddAsync(operation);
                await _context.SaveChangesAsync();

                var negativeCount = await _context.TStockOperation.Where(x=>x.FkStockId == operation.FkStockId && (x.FkOperationTypeId == (int)StockOperationTypeEnum.Export||x.FkOperationTypeId == (int)StockOperationTypeEnum.PurchaseReturn)).SumAsync(x=>x.OperationStockCount);
                var PosetiveCount = await _context.TStockOperation.Where(x=>x.FkStockId == operation.FkStockId && (x.FkOperationTypeId == (int)StockOperationTypeEnum.Import || x.FkOperationTypeId == (int)StockOperationTypeEnum.SaleReturn)).SumAsync(x=>x.OperationStockCount);


                var goodsProvider = await _context.TGoodsProvider.FindAsync(operation.FkStockId);
                goodsProvider.InventoryCount = PosetiveCount - negativeCount;
                if(goodsProvider.InventoryCount <= 0)
                {
                    goodsProvider.HasInventory = false;
                    goodsProvider.InventoryCount = 0 ;
                }
                else {
                    goodsProvider.HasInventory = true;
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