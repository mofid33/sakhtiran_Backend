namespace MarketPlace.API.Data.Dtos.NotificationSetting
{
     public class NotificationSettingDto
     {
          public int NotificationSettingId { get; set; }
          public string Description { get; set; }
          public string Title { get; set; }
          public string NotificationText { get; set; }
          public string EmailText { get; set; }
          public bool? DaysType { get; set; }
          public int? Days { get; set; }
          public string NotifJson { get; set; }
          public bool Sms { get; set; }
          public bool Email { get; set; }
          public bool MobilePushNotif { get; set; }
          public bool WebPushNotif { get; set; }
     }
}