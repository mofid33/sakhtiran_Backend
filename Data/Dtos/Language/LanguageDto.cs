namespace MarketPlace.API.Data.Dtos.Language
{
    public class LanguageDto
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageTitle { get; set; }
        public string JsonFile { get; set; }
        public bool IsRtl { get; set; }
        public bool DefaultLanguage { get; set; }
    }
}