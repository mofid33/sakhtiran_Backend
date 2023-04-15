using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class WebModuleCollections
    {
        public int CollectionId { get; set; }
        public int FkIModuleId { get; set; }
        public int FkCollectionTypeId { get; set; }
        public string XitemIds { get; set; }
        public int SequenceNumber { get; set; }
        public string CollectionTitle { get; set; }
        public bool? HaveLink { get; set; }
        public string LinkUrl { get; set; }
        public string ImageUrl { get; set; }
        public string ResponsiveImageUrl { get; set; }
        public short? CriteriaType { get; set; }
        public decimal? CriteriaFrom { get; set; }
        public decimal? CriteriaTo { get; set; }

        public virtual WebCollectionType FkCollectionType { get; set; }
        public virtual WebIndexModuleList FkIModule { get; set; }
    }
}
