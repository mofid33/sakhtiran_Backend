using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Pagination
{
    public class WebsiteFilterDto
    {
        public int Type { get; set; }
        public int Id { get; set; }
        public string Search { get; set; }
        public int GoodsCreatedDay { get; set; }
        public List<int> BrandId { get; set; }
        public decimal FromPrice { get; set; }
        public decimal ToPrice { get; set; }
        public bool JustExist { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int OrderByType { get; set; }
        public List<int> OptionIds { get; set; }
        public bool GetBrand { get; set; }
        public bool GetSpecs { get; set; }
        public bool GetChild { get; set; }
        public bool GetParent { get; set; }
        public bool GetAllCount { get; set; }
        public bool GetMaxPrice { get; set; }
        public int ShopId { get; set; }
    }
}