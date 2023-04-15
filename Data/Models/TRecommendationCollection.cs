using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TRecommendationCollection
    {
        public int ItemId { get; set; }
        public int FkRecommendationId { get; set; }
        public int XItemId { get; set; }

        public virtual TRecommendation FkRecommendation { get; set; }
    }
}
