using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TSetting
    {
        public int SettingId { get; set; }
        public int IndexCollectionCount { get; set; }
        public int IndexCollectionLastDay { get; set; }
        public int RecommendationMaxItemCount { get; set; }
        public string ShopTitle { get; set; }
        public string InstagramUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string TelegramUrl { get; set; }
        public string AparatUrl { get; set; }
        public string LinkedinUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string Phone { get; set; }
        public string SupportPhone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Address { get; set; }
        public string LogoUrlShopHeader { get; set; }
        public string LogoUrlShopFooter { get; set; }
        public string LogoUrlLoginPage { get; set; }
        public string SmsSender { get; set; }
        public string SmsApiKey { get; set; }
        public string SmsUrl { get; set; }
        public string SmsUsername { get; set; }
        public string SmsPassword { get; set; }
        public string PageTitle { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public string AboutUs { get; set; }
        public string ShortDescription { get; set; }
        public int FooterMaxItem { get; set; }
        public int FooterMaxItemPerColumn { get; set; }
        public string FooterWarrantyPolicy { get; set; }
        public string FooterTermOfUser { get; set; }
        public string FooterTermOfSale { get; set; }
        public string FooterPrivacyPolicy { get; set; }
        public string FooterCustomerRights { get; set; }
        public int MaxDeadlineDayToReturning { get; set; }
        public int MaxSliderForShopWebPage { get; set; }
        public bool SysAutoActiveGoods { get; set; }
        public bool SysAutoActiveBrand { get; set; }
        public bool SysAutoActiveGuarantee { get; set; }
        public bool SysDisplayCategoriesWithoutGoods { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public int DashboardTablesRows { get; set; }
        public string CustomerLoginPageBackgroundImage { get; set; }
        public string HelpPageBackgroundImage { get; set; }
        public int? DefaultMinimumInventory { get; set; }
        public string RegistrationFinalMessage { get; set; }
        public int? VendorCountInStoreList { get; set; }
        public string InactiveShopMessage { get; set; }
        public bool? LiveChatStatus { get; set; }
        public string ShopDefaultBannerImageUrl { get; set; }
        public string ShopCalculateComment { get; set; }
        public string ShopWelcomeMessage { get; set; }
        public int? ShopSearchZoneKilometer { get; set; }
    }
}
