using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class HelpService : IHelpService
    {
        public IMapper _mapper { get; }
        public IHelpRepository _helpRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public HelpService(IMapper mapper, 
         ICategoryRepository categoryRepository,
        IHelpRepository helpRepository, 
        IFileUploadService fileUploadService,
        IMessageLanguageService ms,
        IHttpContextAccessor httpContextAccessor)
        {
            _fileUploadService = fileUploadService;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._helpRepository = helpRepository;
            this._mapper = mapper;
            _ms = ms;
        }

        public async Task<ApiResponse<bool>> AddHelpTopic(HelpTopicSerializeDto helpTopicDto)
        {
            var topicObj = Extentions.Deserialize<HelpTopicAddDto>(helpTopicDto.HelpTopic);
            if (topicObj == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.BrandDeserialize));
            }
            var TopicFileName = "";
            if (helpTopicDto.Icon != null)
            {
                TopicFileName = _fileUploadService.UploadImage(helpTopicDto.Icon, Pathes.TopicTemp);
                if (TopicFileName == null)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.UploadFile));
                }
                topicObj.IconUrl = TopicFileName;
            }
            var mapTopic = _mapper.Map<THelpTopic>(topicObj);
            var craetedTopic = await _helpRepository.AddHelpTopic(mapTopic);
            if (craetedTopic == null)
            {
                if (helpTopicDto.Icon != null)
                {
                    _fileUploadService.DeleteImage(TopicFileName, Pathes.TopicTemp);
                }
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.HelpTopicAdding));
            }
            if (helpTopicDto.Icon != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(TopicFileName, Pathes.TopicTemp, Pathes.Topic + craetedTopic.TopicId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(TopicFileName, Pathes.TopicTemp);
                }
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditHelpTopic(HelpTopicSerializeDto helpTopicDto)
        {
            var topicObj = Extentions.Deserialize<HelpTopicAddDto>(helpTopicDto.HelpTopic);
            if (topicObj == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.BrandDeserialize));
            }
            var fileName = "";
            var oldFileName = "";
            if (helpTopicDto.Icon != null)
            {
                oldFileName = topicObj.IconUrl;
                fileName = _fileUploadService.UploadImage(helpTopicDto.Icon, Pathes.Topic + topicObj.TopicId + "/");
                if (fileName == null)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.UploadFile));
                }
                topicObj.IconUrl= fileName;
            }
            var mapTopic = _mapper.Map<THelpTopic>(topicObj);
            var editedTopic = await _helpRepository.EditHelpTopic(mapTopic);
            if (editedTopic == false)
            {
                if(helpTopicDto.Icon != null)
                {
                    _fileUploadService.DeleteImage(fileName, Pathes.Topic + topicObj.TopicId + "/");
                }
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.HelpTopicEditing));
            }
            if (helpTopicDto.Icon != null)
            {
                _fileUploadService.DeleteImage(oldFileName, Pathes.Topic + topicObj.TopicId + "/");
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<HelpTopicListDto>>> GetHelpTopicList(PaginationFormDto pagination)
        {
            var data = await _helpRepository.GetHelpTopicList(pagination);
            if(data == null)
            {
                return new ApiResponse<Pagination<HelpTopicListDto>>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.HelpTopicGetting));
            }
            var count = await _helpRepository.GetHelpTopicListCount(pagination);
            return new ApiResponse<Pagination<HelpTopicListDto>>(ResponseStatusEnum.Success,new Pagination<HelpTopicListDto>(count,data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<HelpTopicGetDto>> GetHelpTopicById(int topicId)
        {
            var data = await _helpRepository.GetHelpTopicById(topicId);
            if(data == null)
            {
                return new ApiResponse<HelpTopicGetDto>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.HelpTopicGetting));
            }
            return new ApiResponse<HelpTopicGetDto>(ResponseStatusEnum.Success,data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> AddHelpArticle(HelpArticleAddDto helpArticleDto)
        {
            var mapArticle = _mapper.Map<THelpArticle>(helpArticleDto);
            var craetedArticle = await _helpRepository.AddHelpArticle(mapArticle);
            if (craetedArticle == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.HelpArticleAdding));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> EditHelpArticle(HelpArticleAddDto helpArticleDto)
        {
            var mapArticle = _mapper.Map<THelpArticle>(helpArticleDto);
            var editedArticle = await _helpRepository.EditHelpArticle(mapArticle);
            if (editedArticle == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.HelpArticleEditing));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<HelpArticleListDto>>> GetHelpArticleList(PaginationFormDto pagination)
        {
            var data = await _helpRepository.GetHelpArticleList(pagination);
            if(data == null)
            {
                return new ApiResponse<Pagination<HelpArticleListDto>>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.HelpArticleGetting));
            }
            var count = await _helpRepository.GetHelpArticleListCount(pagination);
            return new ApiResponse<Pagination<HelpArticleListDto>>(ResponseStatusEnum.Success,new Pagination<HelpArticleListDto>(count,data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<HelpArticleGetDto>> GetHelpArticleById(int articleId)
        {
            var data = await _helpRepository.GetHelpArticleById(articleId);
            if(data == null)
            {
                return new ApiResponse<HelpArticleGetDto>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.HelpArticleGetting));
            }
            return new ApiResponse<HelpArticleGetDto>(ResponseStatusEnum.Success,data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> TopicDelete(int topicId)
        {
            var result = await _helpRepository.TopicDelete(topicId);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }        
        }

        public async Task<ApiResponse<bool>> ArticleDelete(int articleId)
        {
            var result = await _helpRepository.ArticleDelete(articleId);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }          
        }


    }
}