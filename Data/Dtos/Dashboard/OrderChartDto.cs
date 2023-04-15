namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class OrderChartDto
    {
        public string Date { get; set; }
        public long order { get; set; }
        public long Canceled { get; set; }
    }
}