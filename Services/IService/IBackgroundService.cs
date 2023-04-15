using System;
using System.Threading.Tasks;

namespace MarketPlace.API.Services.IService
{
    public interface IBackgroundService
    {
        Task<bool> SetAndUnsetDiscount();
        Task<bool> CheckAbandonedShoppingCarts();
        Task<bool> CheckShopStatus();

    }
}