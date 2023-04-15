using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.WebModule
{
    public class WebModuleCollectionsGetDto
    {
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
        public WebCollectionTypeDto FkCollectionType { get; set; }
        public List<GoodsBaseDetailDto> Goods { get; set; }
        public List<CategoryTreeView> Category { get; set; }
        public List<SpecialSellPlanDto> SpecialSellPlan { get; set; }
        public CategoryAddGetDto CategorySelected { get; set; }
    }
}