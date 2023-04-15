using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IAccountingRepository
    {
         Task<List<TransactionGetDto>> GetAccountingList(AccountingListPaginationDto pagination);
         Task<int> GetAccountingListCount(AccountingListPaginationDto pagination);
         Task<decimal> GetAccountingListTotal(AccountingListPaginationDto pagination);
         Task<List<ShopWithdrawalRequestGetDto>> GetShopWithdrawalRequestList(ShopWithdrawalRequestPaginationDto pagination);
         Task<int> GetShopWithdrawalRequestListCount(ShopWithdrawalRequestPaginationDto pagination);
         Task<RepRes<bool>> EditShopWithdrawalRequest(ShopWithdrawalRequestDto requestDto);
         Task<bool> AddTransaction(int fkTransactionTypeId, Guid fkUserId, long? fkOrderId, long? fkOrderItemId, int? fkReturningId, int fkApprovalStatusId,decimal amount, string comment);
         Task<bool> UpdateCredit(Guid userId);
         Task<decimal> GetShopBalance();
         Task<RepRes<bool>> AddShopWithdrawalRequest(ShopAddWithdrawalRequestDto request);
    }
}