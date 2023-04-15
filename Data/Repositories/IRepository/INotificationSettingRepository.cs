using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.NotificationSetting;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
     public interface INotificationSettingRepository
     {
          Task<TNotificationSetting> EditAsync(TNotificationSetting notifSetting);
          Task<List<NotificationSettingDto>> GetNotificationSettingsWithPaginationAsync(PaginationDto pagination);
          Task<int> GetNotificationSettingsCountAsync(PaginationDto pagination);
          Task<TNotificationSetting> GetByIdAsync(int notificationSettingId);
     }
}