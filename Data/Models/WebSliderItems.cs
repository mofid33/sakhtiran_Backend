using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class WebSliderItems
    {
        public int ItemId { get; set; }
        public int FkSliderId { get; set; }
        public int XitemId { get; set; }

        public virtual WebSlider FkSlider { get; set; }
    }
}
