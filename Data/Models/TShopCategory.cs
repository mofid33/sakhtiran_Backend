using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopCategory
    {
        public int ShopCategoryId { get; set; }
        public int FkShopId { get; set; }
        public int FkCategoryId { get; set; }
        public decimal? ContractCommissionFee { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual TShop FkShop { get; set; }
    }
}
