using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class ShopRequestDto
    {
        public int ShopId { get; set; }
        public string ShopTitle { get; set; }
        public string Date { get; set; }
        public string Email { get; set; }
        public int CountryId { get; set; }
        public string CountryTitle { get; set; }
        public int? PlanId { get; set; }
        public string PlanTitle { get; set; }
        public List<CategoryFormGetDto> Category { get; set; }
    }
}