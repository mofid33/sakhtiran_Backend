namespace MarketPlace.API.Data.Dtos.Setting
{
    public class WebsiteSettingWebDto
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
        public int? FooterMaxItem { get; set; }
        public int? FooterMaxItemPerColumn { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public string PageTitle { get; set; }
        public string DescriptionCalculateShopRatePrice { get; set; }
        public bool LiveChatStatus { get; set; }


    }
}