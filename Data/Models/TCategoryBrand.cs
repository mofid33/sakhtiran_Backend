using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCategoryBrand
    {
        public int BrandCategoryId { get; set; }
        public int FkCategoryId { get; set; }
        public int FkBrandId { get; set; }

        public virtual TBrand FkBrand { get; set; }
        public virtual TCategory FkCategory { get; set; }
    }
}
