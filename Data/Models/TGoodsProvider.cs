using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsProvider
    {
        public TGoodsProvider()
        {
            TCallRequest = new HashSet<TCallRequest>();
            TDiscountFreeGoods = new HashSet<TDiscountFreeGoods>();
            TDiscountGoods = new HashSet<TDiscountGoods>();
            TGoodsComment = new HashSet<TGoodsComment>();
            TGoodsDocument = new HashSet<TGoodsDocument>();
            TGoodsVariety = new HashSet<TGoodsVariety>();
            TOrderItem = new HashSet<TOrderItem>();
            TStockOperation = new HashSet<TStockOperation>();
        }

        public int ProviderId { get; set; }
        public int FkShopId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkGuaranteeId { get; set; }
        public bool HaveGuarantee { get; set; }
        public int? GuaranteeMonthDuration { get; set; }
        public bool Vatfree { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Vatamount { get; set; }
        public decimal? FinalPrice { get; set; }
        public int? PostTimeoutDayByShop { get; set; }
        public bool ReturningAllowed { get; set; }
        public int? MaxDeadlineDayToReturning { get; set; }
        public bool HasInventory { get; set; }
        public double? InventoryCount { get; set; }
        public bool ToBeDisplayed { get; set; }
        public int? MinimumInventory { get; set; }
        public bool IsAccepted { get; set; }

        public int? PostId { get; set; }

        public virtual TGoods FkGoods { get; set; }
        public virtual TGuarantee FkGuarantee { get; set; }
        public virtual TShop FkShop { get; set; }
        public virtual ICollection<TCallRequest> TCallRequest { get; set; }
        public virtual ICollection<TDiscountFreeGoods> TDiscountFreeGoods { get; set; }
        public virtual ICollection<TDiscountGoods> TDiscountGoods { get; set; }
        public virtual ICollection<TGoodsComment> TGoodsComment { get; set; }
        public virtual ICollection<TGoodsDocument> TGoodsDocument { get; set; }
        public virtual ICollection<TGoodsVariety> TGoodsVariety { get; set; }
        public virtual ICollection<TOrderItem> TOrderItem { get; set; }
        public virtual ICollection<TStockOperation> TStockOperation { get; set; }
    }
}
