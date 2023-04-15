using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Dtos.Home{
    public class FilterGoodsGetDto
    { 
        public Pagination<GoodsHomeDto> Goods { get; set; }
        public List<WebsiteBrandDto> Brands { get; set; }
        public List<WebsiteSpecificationDto> Specs { get; set; }
        public int AllGoodsCount { get; set; }
        public decimal MaxPrice { get; set; }
        public List<CategoryTreeViewDto> ParentCategory { get; set; }
        public CategoryTreeViewFilterDto ChildCategory { get; set; }
        public short? CriteriaType { get; set; }
        public decimal? CriteriaFrom { get; set; }
        public decimal? CriteriaTo { get; set; }
    }
}