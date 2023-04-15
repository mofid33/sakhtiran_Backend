using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TRecommendationItemType
    {
        public TRecommendationItemType()
        {
            TRecommendation = new HashSet<TRecommendation>();
        }

        public int ItemCode { get; set; }
        public string ItemType { get; set; }

        public virtual ICollection<TRecommendation> TRecommendation { get; set; }
    }
}
