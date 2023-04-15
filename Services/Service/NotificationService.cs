using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.NotificationSetting;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.FirebaseServices;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MarketPlace.API.Services.Service
{
    public class NotificationService : INotificationService
    {
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public INotificationSettingRepository _notificationSettingRepository { get; set; }
        public IMessagingService _messagingService { get; set; }
        public IMessageRepository _messagingRepository { get; set; }
        public IEmailService _emailService { get; }
        public IWebHostEnvironment hostingEnvironment;

        public NotificationService(
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        IFileUploadService fileUploadService,
        INotificationSettingRepository notificationSettingRepository,
        IMessagingService messagingService,
        IMessageRepository messagingRepository,
        IEmailService emailService,
        IWebHostEnvironment environment
        )
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._ms = ms;
            _notificationSettingRepository = notificationSettingRepository;
            _messagingService = messagingService;
            _messagingRepository = messagingRepository;
            this._emailService = emailService;
            hostingEnvironment = environment;
        }

        public async Task<bool>  SendNotification(int type,
                string providerFirebasePushNotificationKeyOrClientWebFirebasePushNotificationKey,
                string mobileFirebaseNotificationKey,
                string email, string phoneNumber, Guid? shopUserId = null, string goodsName = null)
        {

            try
            {

                var notifSetting = await _notificationSettingRepository.GetByIdAsync(type);


                if (notifSetting.Sms)
                {
                    if (!string.IsNullOrEmpty(phoneNumber))
                        Extentions.SendPodinisSmsForProvider(notifSetting.NotificationText, phoneNumber);
                }

                if (notifSetting.WebPushNotif)
                {
                    if (shopUserId != null)
                    {
                        Guid id = shopUserId ?? Guid.Empty ;
                        await _messagingRepository.SendMessageToVendor(notifSetting.NotificationText +
                         (string.IsNullOrWhiteSpace(goodsName) ? "" : " ( نام کالا :  " + goodsName + " ) " ) , notifSetting.Title, id);
                        // var tokenList = new List<string>();
                        // tokenList.Add(providerFirebasePushNotificationKeyOrClientWebFirebasePushNotificationKey);
                        // await _messagingService.SendMulticastNotification(notifSetting.Title, notifSetting.Description, null, tokenList);
                    }
                }

                if (notifSetting.MobilePushNotif)
                {
                    if (!string.IsNullOrEmpty(mobileFirebaseNotificationKey))
                    {
                        var tokenList = new List<string>();
                        tokenList.Add(mobileFirebaseNotificationKey);
                        await _messagingService.SendMulticastNotification(notifSetting.Title, notifSetting.NotificationText, null, tokenList);
                    }
                }

                if (notifSetting.Email)
                {
                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(notifSetting.EmailText))
                    {
                        string emailPath = Path.Combine(hostingEnvironment.ContentRootPath, "emailTemplate/description-email.html");
                        string subject = "sakhtiran.com";

                        string text = File.ReadAllText(emailPath);
                        text = text.Replace("#description", notifSetting.EmailText);
                        var resultEmail = await _emailService.Send(email, subject, text);
                    }
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }


        }
    }
}