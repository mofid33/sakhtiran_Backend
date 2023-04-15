namespace MarketPlace.API.Data.Dtos.Setting
{
    public class SettingDto
    {
        public int SettingId { get; set; }
        public int IndexCollectionCount { get; set; }
        public int IndexCollectionLastDay { get; set; }
        public int? RecommendationMaxItemCount { get; set; }
        public string ShopTitle { get; set; }
        public string InstagramUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string TelegramUrl { get; set; }
        public string AparatUrl { get; set; }
        public string LinkedinUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string Phone { get; set; }
        public string SupportPhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string LogoUrlShopHeader { get; set; }
        public string LogoUrlShopFooter { get; set; }
        public string LogoUrlLoginPage { get; set; }
        public string SmsSender { get; set; }
        public string SmsApiKey { get; set; }
        public string SmsUrl { get; set; }
        public string SmsUsername { get; set; }
        public string SmsPassword { get; set; }
        public string ShortDescription { get; set; }
        public int? MaxDeadlineDayToReturning { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public string AboutUs { get; set; }
        public int? MaxItemPerFooterColumn { get; set; }
    }
}