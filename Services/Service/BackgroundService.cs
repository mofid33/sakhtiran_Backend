using System;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Services.IService;

namespace MarketPlace.API.Services.Service
{
    public class BackgroundService : IBackgroundService
    {
        public IDiscountRepository _discountRepository { get; }
        public IShopRepository _shopRepository { get; }
        public IUserOrderRepository _userOrderRepository { get; }

        public BackgroundService(IDiscountRepository discountRepository,IShopRepository shopRepository,IUserOrderRepository userOrderRepository )
        {
            this._discountRepository = discountRepository;
            this._shopRepository = shopRepository;
            this._userOrderRepository = userOrderRepository;
        }
        public async Task<bool> SetAndUnsetDiscount()
        {
            var date = DateTime.Now;
            var discounts = await _discountRepository.GetDiscountByDateAndTime(date);
            if (discounts != null)
            {
                if (discounts.Count > 0)
                {
                    var Seted = discounts.Where(x => x.StartDateTime.Value.Date == date.Date).ToList();
                    if (Seted.Count > 0)
                    {
                        foreach (var item in Seted)
                        {
                            var x = await _discountRepository.SetDiscount(item.PlanId);
                        }
                    }

                    var UnSeted = discounts.Where(x => x.EndDateTime.Value.Date == date.Date).ToList();
                    if (UnSeted.Count > 0)
                    {
                        foreach (var item in UnSeted)
                        {
                            var x = await _discountRepository.UnSetDiscount(item.PlanId);
                        }
                    }
                }
            }
            return true;
        }
        
        public async Task<bool> CheckShopStatus()
        {
            var result = await _shopRepository.ChangeShopStatus();
            return true;
        }

        public async Task<bool> CheckAbandonedShoppingCarts()
        {
            var result = await _userOrderRepository.SendMessageForCustomerForUnUseCartOrder();
            return true;
        }

    }
}