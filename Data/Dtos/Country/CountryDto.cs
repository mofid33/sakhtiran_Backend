namespace MarketPlace.API.Data.Dtos.Country
{
    public class CountryDto
    {
        public int CountryId { get; set; }
        public string CountryTitle { get; set; }
        public string FlagUrl { get; set; }
        public decimal Vat { get; set; }
        public bool Status { get; set; }
    }
}