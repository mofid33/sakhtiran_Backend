using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopListGetDto
    {
        public int ShopId { get; set; }
        public short FkStatusId { get; set; }
        public string StatusTitle { get; set; }
        public string RegisteryDateTime { get; set; }
        public string StoreName { get; set; }
        public string CategoryTitle { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? FkPlanId { get; set; }
        public string PlanTitle { get; set; }
        public List<CategoryFormGetDto> Category { get; set; }
        public decimal Credit { get; set; }

    }
}