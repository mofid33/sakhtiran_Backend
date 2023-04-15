
namespace MarketPlace.API.Data.Dtos.PupupItem
{
    public class PupupItemDto
    {
        public int PopupId { get; set; }
        public string Title { get; set; }
        public string PopupImageUrl { get; set; }
        public int? FkCategoryId { get; set; }
        public long? FkTDiscountPlanId { get; set; }
        public bool JustNewGoods { get; set; }
        public bool JustShowOnce { get; set; }
        public bool Status { get; set; }
    }
}