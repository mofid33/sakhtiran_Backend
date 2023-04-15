using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCategoryGuarantee
    {
        public int CategoryGuaranteeId { get; set; }
        public int FkCategoryId { get; set; }
        public int FkGuaranteeId { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual TGuarantee FkGuarantee { get; set; }
    }
}
