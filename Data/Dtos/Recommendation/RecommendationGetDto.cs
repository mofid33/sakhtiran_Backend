
namespace MarketPlace.API.Data.Dtos.Recommendation
{
    public partial class RecommendationGetDto
    {
        public int RecommendationId { get; set; }
        public string ItemType { get; set; }
        public string Title { get; set; }
        public int? RandomCount { get; set; }
        public bool Status { get; set; }

    }
}
