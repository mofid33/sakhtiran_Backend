namespace MarketPlace.API.Data.Dtos.Country
{
    public class CountryFormDto
    {
        public int CountryId { get; set; }
        public string CountryTitle { get; set; }
        public string FlagUrl { get; set; }
        public string Iso { get; set; }
        public string PhoneCode { get; set; }
        public bool? DefualtPreCode { get; set; }

    }
}