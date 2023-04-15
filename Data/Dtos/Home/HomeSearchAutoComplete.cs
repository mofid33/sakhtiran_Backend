using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class HomeSearchAutoComplete
    {
        public List<CategoryFormGetDto> Category { get; set; }
        public List<BrandFormDto> Brand { get; set; }
        public List<GoodsSearchDto> Goods { get; set; }
    }
}