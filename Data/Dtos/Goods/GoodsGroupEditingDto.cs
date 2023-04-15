using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsGroupEditingDto
    {
        public int? CategoryId { get; set; }
        public List<int> ProductsId { get; set; }
        public bool GoodsFieldWeight { get; set; }
        public bool GoodsFieldPrice { get; set; }
        public bool OperationTypeIncrease { get; set; }
        public bool OperationTypeDecrease { get; set; }
        public int Value { get; set; }
        public bool ValueTypePercent { get; set; }
        public bool ValueTypeFixed { get; set; }
    }
}