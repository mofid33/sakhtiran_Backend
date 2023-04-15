using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.WebModule
{

     // لیست آیتم ماژول هایی که به صفحه ی اصلی میفرستیم
    public partial class WebHomeModuleCollectionsDto 
    {
        public string CollectionTypeTitle { get; set; }
        public int CollectionId { get; set; }
        public int FkIModuleId { get; set; }
        public int FkCollectionTypeId { get; set; }
        public int SequenceNumber { get; set; }
        public string CollectionTitle { get; set; }
        public bool? HaveLink { get; set; }
        public string LinkUrl { get; set; }
        public string ImageUrl { get; set; }
        public short? CriteriaType { get; set; }
        public decimal? CriteriaFrom { get; set; }
        public decimal? CriteriaTo { get; set; } 
        public string XitemIds { get; set; }
        public string ResponsiveImageUrl { get; set; }
        public List<GoodsHomeDto> Goods { get; set; }
    }
}
