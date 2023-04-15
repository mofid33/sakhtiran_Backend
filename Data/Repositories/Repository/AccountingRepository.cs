using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class AccountingRepository : IAccountingRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public AccountingRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
            token = new TokenParseDto(httpContextAccessor) ;
        }
        public async Task<List<TransactionGetDto>> GetAccountingList(AccountingListPaginationDto pagination)
        {
            try
            {
                var rate = (decimal) 1.00;
                if (header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.0 : (decimal) currency.RatesAgainstOneDollar;
                }
                return await _context.TUserTransaction
                .Include(x => x.FkUser)
                .Where(x => x.FkUser.FkShopId != null &&
                (pagination.ShopId == 0 ? true : x.FkUser.FkShopId == pagination.ShopId) &&
                (pagination.Status == null ? true : x.FkApprovalStatusId == pagination.Status) &&
                (pagination.Type == 0 ? true : x.FkTransactionTypeId == pagination.Type) &&
                (pagination.FromDate == (DateTime?)null ? true : x.TransactionDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.TransactionDateTime <= pagination.ToDate)
                )
                .OrderByDescending(x => x.TransactionDateTime)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkApprovalStatus)
                .Include(x => x.FkTransactionType)
                .Include(x => x.FkUser).ThenInclude(t => t.FkShop)
                .Select(x => new TransactionGetDto()
                {
                    Amount = decimal.Round(x.Amount / rate, 2, MidpointRounding.AwayFromZero) ,
                    ApprovalStatusTitle = JsonExtensions.JsonValue(x.FkApprovalStatus.StatusTitle, header.Language),
                    Comment = x.Comment,
                    FkApprovalStatusId = x.FkApprovalStatusId,
                    FkOrderId = x.FkOrderId,
                    FkOrderItemId = x.FkOrderItemId,
                    FkReturningId = x.FkReturningId,
                    FkTransactionTypeId = x.FkTransactionTypeId,
                    FkUserId = x.FkUserId,
                    ShopTitle = x.FkUser.FkShop.StoreName,
                    TransactionDateTime = Extentions.PersianDateString(x.TransactionDateTime),
                    TransactionId = x.TransactionId,
                    TransactionTypeTitle = JsonExtensions.JsonValue(x.FkTransactionType.TransactionTypeTitle, header.Language)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetAccountingListCount(AccountingListPaginationDto pagination)
        {
            try
            {
                return await _context.TUserTransaction
                .Include(x => x.FkUser)
                .AsNoTracking()
                .CountAsync(x => x.FkUser.FkShopId != null &&
                (pagination.ShopId == 0 ? true : x.FkUser.FkShopId == pagination.ShopId) &&
                (pagination.Status == null ? true : x.FkApprovalStatusId == pagination.Status) &&
                (pagination.Type == 0 ? true : x.FkTransactionTypeId == pagination.Type) &&
                (pagination.FromDate == (DateTime?)null ? true : x.TransactionDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.TransactionDateTime <= pagination.ToDate)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> GetAccountingListTotal(AccountingListPaginationDto pagination)
        {
            try
            {
                return await _context.TUserTransaction
                .Include(x => x.FkUser)
                .AsNoTracking()
                .Where(x => x.FkUser.FkShopId != null &&
                (pagination.ShopId == 0 ? true : x.FkUser.FkShopId == pagination.ShopId) &&
                (pagination.Status == null ? true : x.FkApprovalStatusId == pagination.Status) &&
                (pagination.Type == 0 ? true : x.FkTransactionTypeId == pagination.Type) &&
                (pagination.FromDate == (DateTime?)null ? true : x.TransactionDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.TransactionDateTime <= pagination.ToDate)
                ).SumAsync(x => x.Amount);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<ShopWithdrawalRequestGetDto>> GetShopWithdrawalRequestList(ShopWithdrawalRequestPaginationDto pagination)
        {
            try
            {
                var rate =(decimal) 1.00;
                if (header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.00 : (decimal) currency.RatesAgainstOneDollar;
                }
                return await _context.TShopWithdrawalRequest
                .Where(x =>
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.SetStatus == false ? true : x.Status == pagination.Status) &&
                (pagination.FromDate == (DateTime?)null ? true : x.RequestDate >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.RequestDate <= pagination.ToDate)
                )
                .OrderByDescending(x => x.RequestDate)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new ShopWithdrawalRequestGetDto()
                {
                    Amount = decimal.Round(x.Amount / rate, 2, MidpointRounding.AwayFromZero),
                    DocumentUrl = x.DocumentUrl,
                    FkShopId = x.FkShopId,
                    RequestDate = Extentions.PersianDateString(x.RequestDate),
                    RequestId = x.RequestId,
                    RequestText = x.RequestText,
                    ResponseText = x.ResponseText,
                    ShopTitle = x.FkShop.StoreName,
                    Status = x.Status
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetShopWithdrawalRequestListCount(ShopWithdrawalRequestPaginationDto pagination)
        {
            try
            {
                return await _context.TShopWithdrawalRequest
                .AsNoTracking()
                .CountAsync(x =>
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.SetStatus == false ? true : x.Status == pagination.Status) &&
                (pagination.FromDate == (DateTime?)null ? true : x.RequestDate >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.RequestDate <= pagination.ToDate)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<RepRes<bool>> EditShopWithdrawalRequest(ShopWithdrawalRequestDto requestDto)
        {
            try
            {
                var data = await _context.TShopWithdrawalRequest.FindAsync(requestDto.RequestId);
                if (data == null)
                {
                    return new RepRes<bool>(Message.ShopWithdrawalRequestEdit, false, false);
                }
                if (data.Status == true)
                {
                    return new RepRes<bool>(Message.ShopWithdrawalRequestStatusIsNotPending, false, false);
                }
                var rate = (decimal) 1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.00 : (decimal) currency.RatesAgainstOneDollar;
                }
                data.Amount = decimal.Round(requestDto.Amount / rate, 2, MidpointRounding.AwayFromZero);
                data.DocumentUrl = requestDto.DocumentUrl;
                data.ResponseText = requestDto.ResponseText;
                data.Status = requestDto.Status;
                await _context.SaveChangesAsync();
                if (requestDto.Status == true)
                {
                    var userId = await _context.TUser.Select(x => new { x.UserId, x.FkShopId }).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == data.FkShopId);
                    await this.AddTransaction((int)TransactionTypeEnum.PayoutMoney, userId.UserId,null,null,null,(int)TransactionStatusEnum.Completed,  data.Amount, "تایید درخواست برداشت و پرداخت " + data.Amount + "تومان به تامین کننده");
                }

                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.ShopWithdrawalRequestEdit, false, false);
            }
        }

        public async Task<bool> AddTransaction(int fkTransactionTypeId, Guid fkUserId, long? fkOrderId, long? fkOrderItemId, int? fkReturningId, int fkApprovalStatusId,decimal amount, string comment)
        {
            try
            {
                var data = new TUserTransaction();
                data.TransactionDateTime = DateTime.Now;
                data.FkTransactionTypeId = fkTransactionTypeId;
                data.FkUserId = fkUserId;
                data.FkOrderId = fkOrderId;
                data.FkOrderItemId = fkOrderItemId;
                data.FkReturningId = fkReturningId;
                data.FkApprovalStatusId = fkApprovalStatusId;
                data.Amount = amount;
                data.Comment = comment;
                await _context.TUserTransaction.AddAsync(data);

                await _context.SaveChangesAsync();

                var PositivAmount = await _context.TUserTransaction.Where(x=>x.FkUserId == data.FkUserId&& 
                x.FkApprovalStatusId == (int)TransactionStatusEnum.Completed && 
                (x.FkTransactionTypeId == (int)TransactionTypeEnum.Sales ||x.FkTransactionTypeId == (int)TransactionTypeEnum.CashPayment||x.FkTransactionTypeId == (int)TransactionTypeEnum.GiftCard || x.FkTransactionTypeId == (int)TransactionTypeEnum.Refund ||x.FkTransactionTypeId == (int)TransactionTypeEnum.RentPayment)).SumAsync(x=>x.Amount);

                var negativeAmount = await _context.TUserTransaction.Where(x=>x.FkUserId == data.FkUserId&& 
                x.FkApprovalStatusId == (int)TransactionStatusEnum.Completed && 
                (x.FkTransactionTypeId == (int)TransactionTypeEnum.Purchased || x.FkTransactionTypeId == (int)TransactionTypeEnum.Withdraw || x.FkTransactionTypeId == (int)TransactionTypeEnum.PayoutMoney||x.FkTransactionTypeId == (int)TransactionTypeEnum.BlockAmount || x.FkTransactionTypeId == (int)TransactionTypeEnum.Commission ||x.FkTransactionTypeId == (int)TransactionTypeEnum.RentAmount)).SumAsync(x=>x.Amount);


                var user = await _context.TUser.Include(x=>x.FkShop).Include(x=>x.FkCustumer).FirstOrDefaultAsync(x=>x.UserId == data.FkUserId);
                if(user.FkShopId != null)
                {
                    user.FkShop.Credit = PositivAmount - negativeAmount;
                }
                if(user.FkCustumerId != null)
                {
                    user.FkCustumer.Credit = PositivAmount - negativeAmount;
                }

                await _context.SaveChangesAsync();


                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }

        public async Task<bool> UpdateCredit(Guid userId)
        {
            try
            {

                var PositivAmount = await _context.TUserTransaction.Where(x=>x.FkUserId == userId&& 
                x.FkApprovalStatusId == (int)TransactionStatusEnum.Completed && 
                (x.FkTransactionTypeId == (int)TransactionTypeEnum.Sales ||x.FkTransactionTypeId == (int)TransactionTypeEnum.CashPayment||x.FkTransactionTypeId == (int)TransactionTypeEnum.GiftCard || x.FkTransactionTypeId == (int)TransactionTypeEnum.Refund ||x.FkTransactionTypeId == (int)TransactionTypeEnum.RentPayment)).SumAsync(x=>x.Amount);

                var negativeAmount = await _context.TUserTransaction.Where(x=>x.FkUserId == userId&& 
                x.FkApprovalStatusId == (int)TransactionStatusEnum.Completed && 
                (x.FkTransactionTypeId == (int)TransactionTypeEnum.Purchased ||x.FkTransactionTypeId == (int)TransactionTypeEnum.PayoutMoney||x.FkTransactionTypeId == (int)TransactionTypeEnum.BlockAmount || x.FkTransactionTypeId == (int)TransactionTypeEnum.Commission ||x.FkTransactionTypeId == (int)TransactionTypeEnum.RentAmount)).SumAsync(x=>x.Amount);


                var user = await _context.TUser.Include(x=>x.FkShop).Include(x=>x.FkCustumer).FirstOrDefaultAsync(x=>x.UserId == userId);
                if(user.FkShopId != null)
                {
                    user.FkShop.Credit = PositivAmount - negativeAmount;
                }
                if(user.FkCustumerId != null)
                {
                    user.FkCustumer.Credit = PositivAmount - negativeAmount;
                }

                await _context.SaveChangesAsync();


                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<decimal> GetShopBalance()
        {
            try
            {
                var rate = (decimal) 1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.00 : (decimal) currency.RatesAgainstOneDollar;
                }

                var data = await _context.TShop.Select(x=> new{x.ShopId , x.Credit}).AsNoTracking().FirstOrDefaultAsync(x=>x.ShopId == token.Id);
                return decimal.Round(data.Credit / rate, 2, MidpointRounding.AwayFromZero);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<RepRes<bool>> AddShopWithdrawalRequest(ShopAddWithdrawalRequestDto request)
        {
            try
            {
                var rate = (decimal) 1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.00 : (decimal) currency.RatesAgainstOneDollar;
                }
                request.Amount = decimal.Round(request.Amount / rate, 2, MidpointRounding.AwayFromZero) ;
                var shop = await _context.TShop.Select(x=> new{x.ShopId , x.Credit}).AsNoTracking().FirstOrDefaultAsync(x=>x.ShopId == token.Id);
                var oldReqPendingStatus = await _context.TShopWithdrawalRequest.Where(x=>x.Status == null && x.FkShopId == token.Id).AsNoTracking().SumAsync(x=>x.Amount);
                if(shop.Credit <(request.Amount + oldReqPendingStatus))
                {
                    return new RepRes<bool>(Message.CantWithdrawalRequestMoreThanYourCredit,false,false);
                }
                var data = new TShopWithdrawalRequest();

                data.Amount = request.Amount;
                data.FkShopId = token.Id;
                data.RequestDate = DateTime.Now;
                data.RequestText = request.RequestText;
                data.Status = null;
                await _context.TShopWithdrawalRequest.AddAsync(data);
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull,true,true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.WithdrawalRequestAdding,false,false);
            }
        }
    }
}