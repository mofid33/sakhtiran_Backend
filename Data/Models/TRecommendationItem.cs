using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TRecommendationItem
    {
        public int RecommendationItemId { get; set; }
        public int FkRecommendationId { get; set; }
        public int FkRecommendationItemTypeId { get; set; }
        public int XItemId { get; set; }
        public int RandomItemCount { get; set; }

        public virtual TRecommendation FkRecommendation { get; set; }
        public virtual TRecommendationItemType FkRecommendationItemType { get; set; }
    }
}
