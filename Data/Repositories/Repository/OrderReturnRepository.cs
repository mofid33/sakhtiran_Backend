using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.OrderReturning;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.ReturningStatus;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class OrderReturnRepository : IOrderReturnRepository
    {
        public MarketPlaceDbContext _context { get; }
        public IAccountingRepository _accountingRepository { get; }
        public IWareHouseRepository _wareHouseRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IEmailService _emailService { get; }
        public IWebHostEnvironment hostingEnvironment;
        public OrderReturnRepository(MarketPlaceDbContext context, 
        IWareHouseRepository wareHouseRepository, 
        IAccountingRepository accountingRepository, 
        IEmailService emailService,
        IWebHostEnvironment environment,
        IHttpContextAccessor httpContextAccessor)
        {
            _wareHouseRepository = wareHouseRepository;
            _accountingRepository = accountingRepository;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
            this._emailService = emailService;
            hostingEnvironment = environment;
        }

        public async Task<List<OrderReturningListDto>> GetOrderReturningList(OrderReturningPaginationDto pagination)
        {
            try
            {
                var returningStatus = await _context.TReturningStatus.ToListAsync();
                return await _context.TOrderReturning
                .Include(x => x.FkOrder)
                .Include(x => x.FkOrderItem)
                .Where(x =>
                (pagination.CustomerId == 0 ? true : pagination.CustomerId == x.FkOrder.FkCustomerId) &&
                (pagination.GoodsId == 0 ? true : pagination.GoodsId == x.FkOrderItem.FkGoodsId) &&
                (pagination.ShopId == 0 ? true : pagination.ShopId == x.FkOrderItem.FkShopId) &&
                (pagination.statusId == 0 ? true : pagination.statusId == x.FkStatusId) &&
                (pagination.FromDate == (DateTime?)null ? true : pagination.FromDate <= x.RegisterDateTime) &&
                (pagination.ToDate == (DateTime?)null ? true : pagination.ToDate >= x.RegisterDateTime)
                )
                .OrderByDescending(x => x.ReturningId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkStatus)
                .Include(x => x.FkOrder).ThenInclude(t => t.FkCustomer)
                .Include(x => x.FkOrderItem).ThenInclude(t => t.FkShop)
                .Include(x => x.FkOrderItem).ThenInclude(t => t.FkGoods)
                .Select(x => new OrderReturningListDto()
                {
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    FkOrderId = x.FkOrderId,
                    FkOrderItemId = x.FkOrderItemId,
                    FkStatusId = x.FkStatusId,
                    FkReturningActionId = x.FkReturningActionId,
                    GoodsCode = x.FkOrderItem.FkGoods.GoodsCode,
                    GoodsId = x.FkOrderItem.FkGoodsId,
                    GoodsImage = x.FkOrderItem.FkGoods.ImageUrl,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkOrderItem.FkGoods.Title, header.Language),
                    GoodsSerialNumber = x.FkOrderItem.FkGoods.SerialNumber,
                    RegisterDateTime = Extentions.PersianDateString((DateTime)x.RegisterDateTime),
                    ReturningId = x.ReturningId,
                    shopId = x.FkOrderItem.FkShopId,
                    ShopTitle = x.FkOrderItem.FkShop.StoreName,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                    ReturningStatusList = _context.TReturningStatus.Where(c =>
                       (
                       (x.FkReturningActionId == 1 ? // refund 
                       (c.StatusId != (int)ReturningStatusEnum.ResendProduct && c.StatusId != (int)ReturningStatusEnum.Delivered &&
                       c.StatusId != (int)ReturningStatusEnum.ExchangeComplete) :
                       (c.StatusId != (int)ReturningStatusEnum.RefundComplete)
                       )))
                 .Select(d => new ReturningStatusFormDto()
                 {
                     StatusTitle = JsonExtensions.JsonGet(d.StatusTitle, header),
                     Description = JsonExtensions.JsonGet(d.Description, header),
                     StatusId = d.StatusId
                 })
                       .ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetOrderReturningListCount(OrderReturningPaginationDto pagination)
        {
            try
            {
                return await _context.TOrderReturning
                .Include(x => x.FkOrder)
                .Include(x => x.FkOrderItem)
                .AsNoTracking()
                .CountAsync(x =>
                (pagination.CustomerId == 0 ? true : pagination.CustomerId == x.FkOrder.FkCustomerId) &&
                (pagination.GoodsId == 0 ? true : pagination.GoodsId == x.FkOrderItem.FkGoodsId) &&
                (pagination.ShopId == 0 ? true : pagination.ShopId == x.FkOrderItem.FkShopId) &&
                (pagination.statusId == 0 ? true : pagination.statusId == x.FkStatusId) &&
                (pagination.FromDate == (DateTime?)null ? true : pagination.FromDate <= x.RegisterDateTime) &&
                (pagination.ToDate == (DateTime?)null ? true : pagination.ToDate >= x.RegisterDateTime)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<OrderReturningLogDto>> GetOrderReturningLog(int returnId)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TOrderReturningLog
                    .Where(x => x.FkReturningId == returnId)
                    .OrderByDescending(x => x.LogDateTime)
                    .Include(x => x.FkStatus)
                    .Include(x => x.FkUser)
                    .ThenInclude(x => x.FkCustumer)
                    .Include(x => x.FkUser)
                    .ThenInclude(x => x.FkShop)
                    .Select(x => new OrderReturningLogDto()
                    {
                        FkReturningId = x.FkReturningId,
                        FkStatusId = x.FkStatusId,
                        FkUserId = x.FkUserId,
                        LogComment = x.LogComment,
                        LogDateTime = Extentions.PersianDateString((DateTime)x.LogDateTime),
                        LogId = x.LogId,
                        StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                        UserName = x.FkUser.UserName,
                        FullName = x.FkUser.FkCustumer != null ? (x.FkUser.FkCustumer.Name + " " + x.FkUser.FkCustumer.Family) :
                                   (x.FkUser.FkShop != null ? (x.FkUser.FkShop.FullName) : "ادمین")
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    return await _context.TOrderReturningLog
                    .Include(x => x.FkReturning).ThenInclude(t => t.FkOrderItem)
                    .Where(x => x.FkReturningId == returnId && (token.Rule == UserGroupEnum.Seller ? x.FkReturning.FkOrderItem.FkShopId == token.Id : true))
                    .OrderByDescending(x => x.LogDateTime)
                    .Include(x => x.FkStatus)
                    .Include(x => x.FkUser)
                    .ThenInclude(x => x.FkCustumer)
                    .Include(x => x.FkUser)
                    .ThenInclude(x => x.FkShop)
                    .Select(x => new OrderReturningLogDto()
                    {
                        FkReturningId = x.FkReturningId,
                        FkStatusId = x.FkStatusId,
                        FkUserId = x.FkUserId,
                        LogComment = x.LogComment,
                        LogDateTime = Extentions.PersianDateString((DateTime)x.LogDateTime),
                        LogId = x.LogId,
                        StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                        UserName = x.FkUser.UserName,
                        FullName = x.FkUser.FkCustumer != null ? (x.FkUser.FkCustumer.Name + " " + x.FkUser.FkCustumer.Family) :
                                   (x.FkUser.FkShop != null ? (x.FkUser.FkShop.FullName) : "ادمین")
                    })
                    .AsNoTracking().ToListAsync();

                }

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<OrderReturningDetailDto> GetOrderReturningDetail(int returnId)
        {
            try
            {
                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }


                var orderReturn = await _context.TOrderReturning
                .Include(x => x.FkOrder).ThenInclude(t => t.FkCustomer)
                .Include(x => x.FkOrderItem).ThenInclude(t => t.FkShop)
                .Include(x => x.FkOrderItem).ThenInclude(t => t.FkGoods)
                .Select(x => new OrderReturningDetailDto()
                {
                    CustomerEmail = x.FkOrder.FkCustomer.Email,
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    CustomerPhone = x.FkOrder.FkCustomer.MobileNumber,
                    DeliveredDate = Extentions.PersianDateString((DateTime)x.FkOrderItem.DeliveredDate),
                    DiscountAmount = decimal.Round((decimal)x.FkOrderItem.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    FinalPrice = decimal.Round((decimal)x.FkOrderItem.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    FkOrderId = x.FkOrderId,
                    FkOrderItemId = x.FkOrderItemId,
                    FkReturningActionId = x.FkReturningActionId,
                    FkReturningReasonId = x.FkReturningReasonId,
                    FkShippingMethodId = x.FkShippingMethodId,
                    FkStatusId = x.FkStatusId,
                    GoodsCode = x.FkOrderItem.FkGoods.GoodsCode,
                    GoodsId = x.FkOrderItem.FkGoods.GoodsId,
                    GoodsImage = x.FkOrderItem.FkGoods.ImageUrl,
                    GoodsSerialNumber = x.FkOrderItem.FkGoods.SerialNumber,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkOrderItem.FkGoods.Title, header.Language),
                    ItemCount = x.FkOrderItem.ItemCount,
                    PlacedDateTime = Extentions.PersianDateString((DateTime)x.FkOrder.PlacedDateTime),
                    ProviderComment = x.ProviderComment,
                    RegisterDateTime = Extentions.PersianDateString((DateTime)x.RegisterDateTime),
                    RequestComment = x.RequestComment,
                    ReturningActionTitle = JsonExtensions.JsonValue(x.FkReturningAction.ReturningTypeTitle, header.Language),
                    ReturningId = x.ReturningId,
                    ReturningReasonTitle = JsonExtensions.JsonValue(x.FkReturningReason.ReasonTitle, header.Language),
                    ShippingAmount = decimal.Round((decimal)x.ShippingAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    ShippingMethodTitle = JsonExtensions.JsonValue(x.FkShippingMethod.ShippingMethodTitle, header.Language),
                    ShippmentDate = Extentions.PersianDateString((DateTime)x.FkOrderItem.ShippmentDate),
                    ShopId = x.FkOrderItem.FkShopId,
                    BlockSuccess = x.BlockSuccess,
                    RefundSuccess = x.RefundSuccess,
                    ShopTitle = x.FkOrderItem.FkShop.StoreName,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                    UnitPrice = decimal.Round((decimal)x.FkOrderItem.UnitPrice  / rate, 2, MidpointRounding.AwayFromZero)
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ReturningId == returnId && (token.Rule == UserGroupEnum.Seller ? x.ShopId == token.Id : true));

                var shipingMethods = await _context.TShippingMethod.Where(x => x.Active == true).Select(x => new ShippingMethodFormDto()
                {
                    BaseWeight = x.BaseWeight,
                    CashOnDelivery = x.CashOnDelivery,
                    HaveOnlineService = x.HaveOnlineService,
                    Id = x.Id,
                    ShippingMethodTitle = JsonExtensions.JsonValue(x.ShippingMethodTitle, header.Language)
                }).ToListAsync();
                var shop = await _context.TShop.Select(x => new { x.ShippingPermission, x.ShopId, x.FkCountryId, x.ShippingPossibilities }).AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == orderReturn.ShopId);
                if (shop.ShippingPossibilities == false || shop.ShippingPermission == false)
                {
                    shipingMethods = shipingMethods.Where(x => x.Id != (int)ShippingMethodEnum.Market).ToList();
                }

                orderReturn.ShippingMethodList = shipingMethods;

                orderReturn.ReturningStatusList = await _context.TReturningStatus.Where(x =>
                    (
                    (orderReturn.FkReturningActionId == 1 ? // refund 
                    (x.StatusId != (int)ReturningStatusEnum.ResendProduct && x.StatusId != (int)ReturningStatusEnum.Delivered &&
                    x.StatusId != (int)ReturningStatusEnum.ExchangeComplete) :
                    (x.StatusId != (int)ReturningStatusEnum.RefundComplete)
                    ))
                    )
                    .Select(x => new ReturningStatusFormDto()
                    {
                        StatusTitle = JsonExtensions.JsonValue(x.StatusTitle, header.Language),
                        Description = JsonExtensions.JsonValue(x.Description, header.Language),
                        StatusId = x.StatusId
                    }).AsNoTracking().ToListAsync();

                return orderReturn;


            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> EditOrderReturning(OrderReturningChangeDto orderReturning)
        {
            try
            {
                var data = await _context.TOrderReturning
                .Include(x => x.FkOrderItem).ThenInclude(x => x.FkGoods)
                .Include(x => x.FkOrder).ThenInclude(t => t.TOrderItem)
                .FirstOrDefaultAsync(x => x.ReturningId == orderReturning.ReturningId && (token.Rule == UserGroupEnum.Seller ? x.FkOrderItem.FkShopId == token.Id : true));
                var statusOrder = await _context.TReturningStatus.FirstOrDefaultAsync(c => c.StatusId == orderReturning.FkStatusId);
                bool canChangeStatus = (bool)(token.Rule == UserGroupEnum.Seller ? statusOrder.ShopAvailable : statusOrder.AdminAvailable);
                if (data == null)
                {
                    return new RepRes<bool>(Message.OrderReturningEditing, false, false);
                }
                if (!canChangeStatus)
                {
                    return new RepRes<bool>(Message.OrderReturningCantChangeToThisStatus, false, false);
                }
                if (!ReturningStatusEnumMethods.CheckCanChangeToNewStatus((ReturningStatusEnum)data.FkStatusId, (ReturningStatusEnum)orderReturning.FkStatusId, token.Rule))
                {
                    return new RepRes<bool>(Message.OrderReturningCantChangeToThisStatus, false, false);
                }


                if (orderReturning.ShippingAmount != null)
                {
                    var rate = 1.0;
                    if ( header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? 1.0 : currency.RatesAgainstOneDollar;
                    }
                    data.ShippingAmount = orderReturning.ShippingAmount / rate;
                }
                if (orderReturning.FkStatusId == (int)ReturningStatusEnum.FullyRejected && data.FkStatusId != (int)ReturningStatusEnum.FullyRejected)
                {
                    data.FkOrder.TOrderItem.First(x => x.ItemId == data.FkOrderItemId).FkStatusId = (int)OrderStatusEnum.Completed;
                    data.FkOrderItem.FkStatusId = (int)OrderStatusEnum.Completed;
                    await this.AddOrderLog(data.FkOrderId, data.FkOrderItemId, data.FkOrderItem.FkStatusId, token.UserId, "وضعیت سفارش تغییر کرد");
                }
                if ((orderReturning.FkStatusId == (int)ReturningStatusEnum.RefundComplete || orderReturning.FkStatusId == (int)ReturningStatusEnum.ExchangeComplete) && (data.FkStatusId != (int)ReturningStatusEnum.ExchangeComplete && data.FkStatusId != (int)ReturningStatusEnum.RefundComplete))
                {
                    data.FkOrder.TOrderItem.First(x => x.ItemId == data.FkOrderItemId).FkStatusId = (int)OrderStatusEnum.ReturnComplete;
                    data.FkOrderItem.FkStatusId = (int)OrderStatusEnum.ReturnComplete;
                    await this.AddOrderLog(data.FkOrderId, data.FkOrderItemId, data.FkOrderItem.FkStatusId, token.UserId, "وضعیت سفارش تغییر کرد");
                    if (orderReturning.FkStatusId == (int)ReturningStatusEnum.ExchangeComplete)
                    {
                        var userId = await _context.TUser.Select(x => new { x.UserId, x.FkShopId }).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == _context.TOrderReturning.Include(t => t.FkOrderItem).FirstOrDefault(t => t.ReturningId == data.ReturningId).FkOrderItem.FkShopId);
                        var price = await _context.TUserTransaction.Where(x => x.FkUserId == userId.UserId && x.FkReturningId == data.ReturningId && x.FkTransactionTypeId == (int)TransactionTypeEnum.BlockAmount && x.FkApprovalStatusId == (int)TransactionStatusEnum.Completed).SumAsync(x => x.Amount);
                        if (price != 0)
                        {
                            await _accountingRepository.AddTransaction((int)TransactionTypeEnum.Sales, userId.UserId, null, null, data.ReturningId, (int)TransactionStatusEnum.Completed, price, "پرداخت " + Extentions.DecimalRoundWithZiro(price) + "تومان مسدود شده به تامین کننده");
                        }
                    }
                    else if (orderReturning.FkStatusId == (int)ReturningStatusEnum.RefundComplete)
                    {
                        data.FkOrderItem.FkGoods.SaleCount = data.FkOrderItem.FkGoods.SaleCount - (long)data.Quantity;
                        await _wareHouseRepository.AddStockOpration((int)StockOperationTypeEnum.SaleReturn, data.FkOrderItem.FkVarietyId, data.FkOrderItemId, (double)data.Quantity, data.FkOrderItem.UnitPrice, "برگرداندن آیتم سفارش به وضعیت بازپرداخت کامل");
                    }
                }
                var statusDesc = "";
                if (statusOrder != null)
                {
                    statusDesc = JsonExtensions.JsonGet(statusOrder.Description, header);
                }
                data.FkStatusId = orderReturning.FkStatusId;
                data.FkShippingMethodId = orderReturning.FkShippingMethodId;
                data.ProviderComment = orderReturning.ProviderComment;
                data.FkOrder.FkOrderStatusId = data.FkOrder.TOrderItem.Min(x => x.FkStatusId);
                var returnLog = new TOrderReturningLog();
                returnLog.FkReturningId = data.ReturningId;
                returnLog.FkStatusId = data.FkStatusId;
                returnLog.FkUserId = token.UserId;
                returnLog.LogDateTime = DateTime.Now;
                returnLog.LogId = 0;
                returnLog.LogComment = statusDesc;
                await _context.TOrderReturningLog.AddAsync(returnLog);
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.OrderReturningEditing, false, false);
            }
        }

        public async Task<RepRes<bool>> BlockAmountOrderReturning(AmountDto amount)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var orderReturn = await _context.TOrderReturning.Include(t => t.FkOrderItem).FirstOrDefaultAsync(t => t.ReturningId == amount.ReturnId);
                if (orderReturn.BlockSuccess == true)
                {
                    return new RepRes<bool>(Message.BlockAmountBefore, false, false);
                }
                var shop = await _context.TShop.FirstOrDefaultAsync(t => t.ShopId == orderReturn.FkOrderItem.FkShopId);
                if (shop.Credit < amount.Amount)
                {
                    return new RepRes<bool>(Message.BlockAmountBefore, false, false);
                }
                var userId = await _context.TUser.Select(x => new { x.UserId, x.FkShopId }).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == orderReturn.FkOrderItem.FkShopId);
                var resultTransaction = await _accountingRepository.AddTransaction((int)TransactionTypeEnum.BlockAmount, userId.UserId, null, null, amount.ReturnId, (int)TransactionStatusEnum.Completed, amount.Amount / rate, amount.Comment);
                if (resultTransaction)
                {
                    orderReturn.BlockSuccess = true;
                    await _context.SaveChangesAsync();
                }
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.BlockAmountOrderReturning, false, false);
            }
        }

        public async Task<RepRes<bool>> RefoundAmountOrderReturning(AmountDto amount)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var orderReturn = await _context.TOrderReturning.Include(t => t.FkOrderItem).FirstOrDefaultAsync(t => t.ReturningId == amount.ReturnId);
                if (orderReturn.RefundSuccess == true)
                {
                    return new RepRes<bool>(Message.RefundAmountBefore, false, false);
                }
                if (orderReturn.BlockSuccess != true)
                {
                    return new RepRes<bool>(Message.BlockAmountFirst, false, false);
                }
                var userId = await _context.TUser.Select(x => new { x.UserId, x.FkCustumerId }).AsNoTracking().FirstOrDefaultAsync(x => x.FkCustumerId == _context.TOrderReturning.Include(t => t.FkOrder).FirstOrDefault(t => t.ReturningId == amount.ReturnId).FkOrder.FkCustomerId);
                var resultTransaction = await _accountingRepository.AddTransaction((int)TransactionTypeEnum.Refund, userId.UserId, null, null, amount.ReturnId, (int)TransactionStatusEnum.Completed, amount.Amount / rate, amount.Comment);
                if (resultTransaction)
                {
                    orderReturn.RefundSuccess = true;
                    await _context.SaveChangesAsync();
                }
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.BlockAmountOrderReturning, false, false);
            }
        }

        public async Task<AmountDto> GetOrderReturningAmount(int returnId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TOrderReturning.Include(x => x.FkOrderItem).Select(x => new AmountDto()
                {
                    ReturnId = x.ReturningId,
                    Amount = decimal.Round((decimal)x.FkOrderItem.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero)
                }).AsNoTracking().FirstOrDefaultAsync(x => x.ReturnId == returnId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddOrderLog(long fkOrderId, long? fkOrderItemId, int? fkStatusId, Guid fkUserId, string logComment)
        {
            try
            {
                var log = new TOrderLog();
                log.FkOrderId = fkOrderId;
                log.FkOrderItemId = fkOrderItemId;
                log.FkStatusId = fkStatusId;
                log.FkUserId = fkUserId;
                log.LogComment = logComment;
                log.LogDateTime = DateTime.Now;

                await _context.TOrderLog.AddAsync(log);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
   
   
   
        public async Task<bool> SendEmailAndSMS(int returnId , string msg)
        {
            try
            {
              
                var orderReturn = await _context.TOrderReturning
                .Include(x => x.FkOrder).ThenInclude(t => t.FkCustomer)
                .Include(x => x.FkOrder).ThenInclude(t => t.AdFkCountry)
                .Include(x => x.FkOrderItem)
                .Select(x => new OrderReturningDetailDto()
                {
                    CustomerEmail = x.FkOrder.FkCustomer.Email,
                    ReturningId = x.ReturningId,
                    ShopId = x.FkOrderItem.FkShopId,
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    CustomerPhone =  "+" + x.FkOrder.AdFkCountry.PhoneCode +  x.FkOrder.FkCustomer.MobileNumber
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ReturningId == returnId && (token.Rule == UserGroupEnum.Seller ? x.ShopId == token.Id : true));

                Extentions.SendPodinisSmsForProvider(msg , orderReturn.CustomerPhone) ;
                if (!string.IsNullOrEmpty(orderReturn.CustomerEmail) && !string.IsNullOrEmpty(msg))
                    {
                        string emailPath = Path.Combine(hostingEnvironment.ContentRootPath, "emailTemplate/description-email.html");
                        string subject = "sakhtiran.com";

                        string text = File.ReadAllText(emailPath);
                        text = text.Replace("#description", msg);
                        var resultEmail = await _emailService.Send(orderReturn.CustomerEmail, subject, text);
                    }

                return true;


            }
            catch (System.Exception)
            {
                return false;
            }
        }

   
   
   
   
    }
}