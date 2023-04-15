using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Shop;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class GoodsDetailesDto
    {
        public int GoodsId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string ModelNumber { get; set; }
        public string Category { get; set; }
        public int FkCategoryId { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string UnitTitle { get; set; }
        public bool Like { get; set; }
        public bool HaveVariation { get; set; }
        public bool SaleWithCall { get; set; }
        public bool IsDownloadable { get; set; }
        public List<GoodsDocumentDto> GoodsDocument { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string PageTitle { get; set; }
        public long ViewCount { get; set; }
        public long LikedCount { get; set; }
        public int? SurveyCount { get; set; }
        public double? SurveyScore { get; set; }
        public List<HomeGoodsProviderDto> GoodsProviderVarity { get; set; }
        public List<HomeGoodsProviderDto> OtherProvider { get; set; }
        public int ShopCityId { get; set; }
        public string ShopCityTitle { get; set; }
        public string DescriptionCalculateShopRate { get; set; }
        public int ShopCountryId { get; set; }
        public int ShopProvinceId { get; set; }
        public int BrandId { get; set; }
        public bool ShopDetailAccess { get; set; }

        public List<GoodsHomeDto> Recommendation { get; set; }
        public List<CategoryTreeViewDto> ParentCategory { get; set; }


    }
}