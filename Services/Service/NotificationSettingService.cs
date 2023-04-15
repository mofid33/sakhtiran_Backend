using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.NotificationSetting;

namespace MarketPlace.API.Services.Service
{
    public class NotificationSettingService : INotificationSettingService
    {
        public IMapper _mapper { get; }
        public INotificationSettingRepository _notifcationRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public ICategoryRepository _categoryRepository { get; set; }

        public NotificationSettingService(
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        ICategoryRepository categoryRepository,
        IFileUploadService fileUploadService,
        INotificationSettingRepository notifcationRepository
        )
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._mapper = mapper;
            this._fileUploadService = fileUploadService;
            this._ms = ms;
            _categoryRepository = categoryRepository;
            _notifcationRepository = notifcationRepository;
        }

        public async Task<ApiResponse<NotificationSettingDto>> EditNotificationSettingAsync(NotificationSettingDto model)
        {
            var mappedNotification = _mapper.Map<TNotificationSetting>(model);
            var result = await _notifcationRepository.EditAsync(mappedNotification);
            return new ApiResponse<NotificationSettingDto>(ResponseStatusEnum.Success, model, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<NotificationSettingDto>>> GetNotificationSettingsPaginationAsync(PaginationDto pagination)
        {
            var data = await _notifcationRepository.GetNotificationSettingsWithPaginationAsync(pagination);

            var count = await _notifcationRepository.GetNotificationSettingsCountAsync(pagination);
            return new ApiResponse<Pagination<NotificationSettingDto>>(ResponseStatusEnum.Success, new Pagination<NotificationSettingDto>(count, data), _ms.MessageService(Message.Successfull));
        }

    }
}