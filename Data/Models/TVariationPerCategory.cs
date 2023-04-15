using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TVariationPerCategory
    {
        public long Id { get; set; }
        public int FkCategoryId { get; set; }
        public int FkParameterId { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual TVariationParameter FkParameter { get; set; }
    }
}
