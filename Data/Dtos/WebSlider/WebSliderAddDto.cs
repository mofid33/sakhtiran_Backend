using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.WebSlider
{
    public partial class WebSliderAddDto
    {
        public int SliderId { get; set; }
        public int SequenceNumber { get; set; }
        public string ImageUrl { get; set; }
        public string ResponsiveImageUrl { get; set; }
        public int? FkCollectionTypeId { get; set; }
        public short? CriteriaType { get; set; }
        public double? CriteriaFrom { get; set; }
        public double? CriteriaTo { get; set; }
        public string XitemIds { get; set; }
        public int? FkCategoryId { get; set; }
        public bool? HaveLink { get; set; }

    }
}
