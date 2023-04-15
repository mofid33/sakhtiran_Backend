using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Helper;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.NotificationSetting;

namespace MarketPlace.API.Services.IService
{
    public interface INotificationSettingService
    {
        Task<ApiResponse<NotificationSettingDto>> EditNotificationSettingAsync(NotificationSettingDto model);
        Task<ApiResponse<Pagination<NotificationSettingDto>>> GetNotificationSettingsPaginationAsync(PaginationDto pagination);
    }
}