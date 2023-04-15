using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopStatus
    {
        public TShopStatus()
        {
            TShop = new HashSet<TShop>();
        }

        public short StatusId { get; set; }
        public string StatusTitle { get; set; }
        public string Comment { get; set; }

        public virtual ICollection<TShop> TShop { get; set; }
    }
}
