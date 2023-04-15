namespace MarketPlace.API.Data.Dtos.WareHouse
{
    public class WareHouseOprationAddDto
    {
        public bool In { get; set; }
        public int GoodsProviderId { get; set; }
        public double OperationStockCount { get; set; }
        public string OperationComment { get; set; }
    }
}