using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class WebSlider
    {
        public WebSlider()
        {
            WebSliderItems = new HashSet<WebSliderItems>();
        }

        public int SliderId { get; set; }
        public int SequenceNumber { get; set; }
        public string ImageUrl { get; set; }
        public string ResponsiveImageUrl { get; set; }
        public bool? HaveLink { get; set; }
        public bool? LinkType { get; set; }
        public string ExternalLinkUrl { get; set; }
        public int? FkCollectionTypeId { get; set; }
        public string XitemIds { get; set; }
        public short? CriteriaType { get; set; }
        public decimal? CriteriaFrom { get; set; }
        public decimal? CriteriaTo { get; set; }
        public int? FkCategoryId { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual WebCollectionType FkCollectionType { get; set; }
        public virtual ICollection<WebSliderItems> WebSliderItems { get; set; }
    }
}
