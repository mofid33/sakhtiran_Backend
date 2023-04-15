
namespace MarketPlace.API.Data.Dtos.Recommendation
{
    public partial class RecommendationAddDto
    {
        public int RecommendationId { get; set; }
        public int FkItemType { get; set; }
        public int XItemId { get; set; }
        public int FkCollectionItemTypeId { get; set; }
        public string XCollectionItemIds { get; set; }
        public string TitleType { get; set; }
        public string CategoryPathType { get; set; }
        public string TitleCollection { get; set; }
        public string CategoryPathCollection { get; set; }
        public int? RandomCount { get; set; }
        public bool Status { get; set; }

    }
}
