using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCategorySpecificationGroup
    {
        public int CatSpecGroupId { get; set; }
        public int FkCategoryId { get; set; }
        public int FkSpecGroupId { get; set; }
        public int PriorityNumber { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual TSpecificationGroup FkSpecGroup { get; set; }
    }
}
