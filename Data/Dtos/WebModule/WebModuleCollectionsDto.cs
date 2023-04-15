using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.WebModule
{
    public partial class WebModuleCollectionsDto
    {
        public int CollectionId { get; set; }
        public int FkIModuleId { get; set; }
        public int FkCollectionTypeId { get; set; }
        public int SequenceNumber { get; set; }
        public string CollectionTitle { get; set; }
        public bool? HaveLink { get; set; }
        public bool? LinkType { get; set; }
        public string ExternalLinkUrl { get; set; }
        public string ImageUrl { get; set; }
        public short? CriteriaType { get; set; }
        public double? CriteriaFrom { get; set; }
        public double? CriteriaTo { get; set; }
        public string XitemIds { get; set; }
        public string ResponsiveImageUrl { get; set; }
        public WebCollectionTypeDto FkCollectionType { get; set; }
    }
}
