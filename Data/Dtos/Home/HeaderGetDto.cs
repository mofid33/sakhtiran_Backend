using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class HeaderGetDto
    {
        public List<CategoryWebGetDto> Categories { get; set; }
        public int  WishListCount { get; set; }
        public int  CartCount { get; set; }
        public string  CustomerFullName { get; set; }
        public string  LogoUrlShopHeader { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaTitle { get; set; }
        public string PageTitle { get; set; }
        public bool LiveChatStatus { get; set; }
    }
}