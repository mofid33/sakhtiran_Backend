namespace MarketPlace.API.Data.Dtos.Currency
{
    public class CurrencyDto
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyTitle { get; set; }
        public double RatesAgainstOneDollar { get; set; }
        public bool DefaultCurrency { get; set; }
    }
}