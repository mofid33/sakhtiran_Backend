namespace MarketPlace.API.Data.Dtos.Setting
{
    public class SettingGeneralDto
    {
        public int IndexCollectionCount { get; set; }
        public int IndexCollectionLastDay { get; set; }
        public int RecommendationMaxItemCount { get; set; }
        public int FooterMaxItem { get; set; }
        public int FooterMaxItemPerColumn { get; set; }

        public bool SysAutoActiveGoods { get; set; }
        public bool SysAutoActiveBrand { get; set; }
        public bool SysAutoActiveGuarantee { get; set; }
        public bool SysDisplayCategoriesWithoutGoods { get; set; }
        public bool LiveChatStatus { get; set; }

        public int MaxDeadlineDayToReturning { get; set; }
        public int MaxSliderForShopWebPage { get; set; }
        public int DefaultMinimumInventory { get; set; }
        public int DashboardTablesRows { get; set; }
        public int VendorCountInStoreList { get; set; }
        public int? ShopSearchZoneKilometer { get; set; }

    }
}