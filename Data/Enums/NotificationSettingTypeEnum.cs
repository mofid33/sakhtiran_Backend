namespace MarketPlace.API.Data.Enums
{
    public enum NotificationSettingTypeEnum
    {
        AfterRegisterProvider = 1,
        NewOrderProvider = 2,
        InventoryReachToMinimumProvider = 3,
        RentPaymentNoticeProvider = 4,
        ConfirmationSubscriptionRenewalAfterPaymentRentProvider = 5,
        CompleteRegistrationOrPlanSelectionProvider = 6,
        AfterRegistrationClient = 7,
        AfterOrderClient = 8,
        BeforeShipmentDeliveryClient = 9,
        AbandonedShoppingCartsDaysLater = 10
    }
}