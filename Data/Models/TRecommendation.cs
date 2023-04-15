using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TRecommendation
    {
        public int RecommendationId { get; set; }
        public int FkItemType { get; set; }
        public int XItemId { get; set; }
        public int FkCollectionItemTypeId { get; set; }
        public string XCollectionItemIds { get; set; }
        public int? RandomCount { get; set; }
        public bool Status { get; set; }

        public virtual TRecommendationCollectionType FkCollectionItemType { get; set; }
        public virtual TRecommendationItemType FkItemTypeNavigation { get; set; }
    }
}
