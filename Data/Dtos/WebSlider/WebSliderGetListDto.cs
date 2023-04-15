namespace MarketPlace.API.Data.Dtos.WebSlider
{
    public class WebSliderGetListDto
    {
        public int SliderId { get; set; }
        public int SequenceNumber { get; set; }
        public string ImageUrl { get; set; }
        public string ResponsiveImageUrl { get; set; }
        public int? FkCollectionTypeId { get; set; }
        public short? CriteriaType { get; set; }
        public decimal? CriteriaFrom { get; set; }
        public decimal? CriteriaTo { get; set; }
        public string XitemIds { get; set; }
        public string ExternalLinkUrl { get; set; }
        public int? FkCategoryId { get; set; }
        public bool? HaveLink { get; set; }


    }
}