using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.NotificationSetting;
using MarketPlace.API.FirebaseServices;
using System;
using MarketPlace.API.Data.Constants;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationSettingController : ControllerBase
    {
        public INotificationSettingService _notificationSettingService { get; }
        public INotificationService _notificationService { get; }
        public IMessagingService _messagingService { get; }
        
        public NotificationSettingController(INotificationSettingService notificationSettingService,
        INotificationService notificationService, IMessagingService messagingService)
        {
            _notificationSettingService = notificationSettingService;
            _notificationService = notificationService;
            _messagingService = messagingService;
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<NotificationSettingDto>))]
        [HttpPut("EditNotificationSetting")]
        public async Task<IActionResult> Edit([FromBody] NotificationSettingDto model)
        {
            var result = await _notificationSettingService.EditNotificationSettingAsync(model);
            return new Response<NotificationSettingDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<NotificationSettingDto>>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDto pagination)
        {
            var result = await _notificationSettingService.GetNotificationSettingsPaginationAsync(pagination);
            return new Response<Pagination<NotificationSettingDto>>().ResponseSending(result);
        }


        [HttpPost("SendTest")]
        public async Task<IActionResult> SendTestData()
        {
            // Extentions.SendSmsText("Tesssst", "+4915207829731");
            // _notificationService.SendNotification()a
            var tokens = new List<String>();
            tokens.Add("c8XXGZYYSOZ3qj17YUkBhU:APA91bHSIo9Ido199yE6EzgjglWOTVedXr-TuzzVk14Q_8eA_k08aJJvDeEOGPkpYdDqVWQlYFhkewBpPQw_xDReBTpW4NKUQR7FHkftzp2quGu_H40GSQSFnklTKsLoP0ui3cFUuh0r");
            await _messagingService.SendToTopic(AppConstants.FirebaseNotificationTopics.PROVIDER_PANEL, "fdfjljdflfj", "fdfdf", null);
            return Ok();
        }

    }
}