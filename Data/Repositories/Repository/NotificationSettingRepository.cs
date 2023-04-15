using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.NotificationSetting;

namespace MarketPlace.API.Data.Repositories.Repository
{
     public class NotificationSettingRepository : INotificationSettingRepository
     {
          public MarketPlaceDbContext _context { get; }
          public HeaderParseDto header { get; set; }

          public NotificationSettingRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
          {
               header = new HeaderParseDto(httpContextAccessor);
               this._context = context;
          }

          public async Task<TNotificationSetting> GetByIdAsync(int notificationSettingId)
          {
               try
               {
                    var notifSetting = await _context.TNotificationSetting.FindAsync(notificationSettingId);

                    return notifSetting;
               }
               catch (System.Exception)
               {
                    return null;
               }
          }

          public async Task<TNotificationSetting> EditAsync(TNotificationSetting notifSetting)
          {
               try
               {
                    var mainNotif = await _context.TNotificationSetting.AsNoTracking().SingleOrDefaultAsync(x => x.NotificationSettingId == notifSetting.NotificationSettingId);
                    notifSetting.Description = mainNotif.Description; // prevent editing this field

                    _context.Entry(notifSetting).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return notifSetting;
               }
               catch (System.Exception)
               {
                    return null;
               }
          }

          public async Task<List<NotificationSettingDto>> GetNotificationSettingsWithPaginationAsync(PaginationDto pagination)
          {
               try
               {
                    return await _context.TNotificationSetting
                                        .OrderBy(x => x.NotificationSettingId)
                                        .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                                        .Select(x => new NotificationSettingDto()
                                        {
                                             Days = x.Days,
                                             DaysType = x.DaysType,
                                             Email = x.Email,
                                             MobilePushNotif = x.MobilePushNotif,
                                             NotificationSettingId = x.NotificationSettingId,
                                             NotificationText = x.NotificationText,
                                             NotifJson = x.NotifJson,
                                             Sms = x.Sms,
                                             Title = x.Title,
                                             EmailText = x.EmailText,
                                             WebPushNotif = x.WebPushNotif,
                                             Description = JsonExtensions.JsonValue(x.Description, header.Language),
                                        })
                                        .AsNoTracking().ToListAsync();
               }
               catch (System.Exception)
               {
                    return null;
               }
          }

          public async Task<int> GetNotificationSettingsCountAsync(PaginationDto pagination)
          {
               try
               {
                    return await _context.TNotificationSetting.CountAsync();
               }
               catch (System.Exception)
               {
                    return 0;
               }
          }

     }
}