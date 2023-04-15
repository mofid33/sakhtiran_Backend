using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TSpecificationGroup
    {
        public TSpecificationGroup()
        {
            TCategorySpecificationGroup = new HashSet<TCategorySpecificationGroup>();
            TSpecification = new HashSet<TSpecification>();
        }

        public int SpecGroupId { get; set; }
        public string SpecGroupTitle { get; set; }

        public virtual ICollection<TCategorySpecificationGroup> TCategorySpecificationGroup { get; set; }
        public virtual ICollection<TSpecification> TSpecification { get; set; }
    }
}
