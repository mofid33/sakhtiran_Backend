using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TRecommendationCollectionType
    {
        public TRecommendationCollectionType()
        {
            TRecommendation = new HashSet<TRecommendation>();
        }

        public int CollectionTypeId { get; set; }
        public string CollectionTypeTitle { get; set; }

        public virtual ICollection<TRecommendation> TRecommendation { get; set; }
    }
}
