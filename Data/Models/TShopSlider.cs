using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopSlider
    {
        public int SliderId { get; set; }
        public int FkShopId { get; set; }
        public string ImageUrl { get; set; }
        public bool Status { get; set; }

        public virtual TShop FkShop { get; set; }
    }
}
