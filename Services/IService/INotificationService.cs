using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketPlace.API.Services.IService
{
    public interface INotificationService
    {
       Task<bool>  SendNotification(int type,
               string providerFirebasePushNotificationKeyOrClientWebFirebasePushNotificationKey,
               string mobileFirebaseNotificationKey,
               string email, string phoneNumber, Guid? shopUserId = null, string goodsName = null);
    }
}