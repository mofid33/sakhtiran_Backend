using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountFilterDto
    {
        public int PlanTypeId { get; set; }
        public int CategoryId { get; set; }
        public int GoodsId { get; set; }
        public int ShopId { get; set; }
        public string Title { get; set; }
        public bool? Status { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<int> CatIds { get; set; }
    }
}