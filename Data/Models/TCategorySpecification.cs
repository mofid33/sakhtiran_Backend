using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCategorySpecification
    {
        public int Gcsid { get; set; }
        public int FkCategoryId { get; set; }
        public int FkSpecId { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual TSpecification FkSpec { get; set; }
    }
}
