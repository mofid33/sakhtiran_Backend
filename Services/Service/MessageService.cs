using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Message;
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
    public class MessageService : IMessageService
    {
        public IMapper _mapper { get; }
        public IMessageRepository _messageRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public MessageService(IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IFileUploadService fileUploadService,
        IMessageLanguageService ms, IMessageRepository messageRepository)
        {
            this._fileUploadService = fileUploadService;
            this._messageRepository = messageRepository;
            this._mapper = mapper;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }

        public async Task<ApiResponse<Pagination<MessageHeaderDto>>> GetLastFiveMessage()
        {
            var data = await _messageRepository.GetLastFiveMessage();
            var count = await _messageRepository.GetAllMessageCount();
            return new ApiResponse<Pagination<MessageHeaderDto>>(ResponseStatusEnum.Success, new Pagination<MessageHeaderDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<int>> ViewMessage(int messageId)
        {
            var result = await _messageRepository.ViewMessage(messageId);
            if (result == false)
            {
                return new ApiResponse<int>(ResponseStatusEnum.BadRequest, 0, _ms.MessageService(Message.MessageView));
            }
            var unreadMessage = await _messageRepository.GetInboxMessageUnreadCount();
            return new ApiResponse<int>(ResponseStatusEnum.Success, unreadMessage, _ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<MessageGetOneDto>> GetSentMessageOne(int messageId)
        {
            var data = await _messageRepository.GetSentMessageOne(messageId);
            if (data == null)
            {
                return new ApiResponse<MessageGetOneDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MessageGetting));
            }
            return new ApiResponse<MessageGetOneDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }        
        
        public async Task<ApiResponse<MessageGetOneDto>> GetInboxMessageOne(int messageId)
        {
            var data = await _messageRepository.GetInboxMessageOne(messageId);
            if (data == null)
            {
                return new ApiResponse<MessageGetOneDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MessageGetting));
            }
            return new ApiResponse<MessageGetOneDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<MessageInboxListDto>>> GetInboxMessageList(PaginationMessageDto pagination)
        {
             var data = await _messageRepository.GetInboxMessageList(pagination);
             if(data == null)
             {
                 return new ApiResponse<Pagination<MessageInboxListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MessageGetting));
             }
            var count = await _messageRepository.GetInboxMessageListCount(pagination);
            return new ApiResponse<Pagination<MessageInboxListDto>>(ResponseStatusEnum.Success, new Pagination<MessageInboxListDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<MessageSentListDto>>> GetSentMessageList(PaginationMessageDto pagination)
        {
            var data = await _messageRepository.GetSentMessageList(pagination);
             if(data == null)
             {
                 return new ApiResponse<Pagination<MessageSentListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MessageGetting));
             }
            var count = await _messageRepository.GetSentMessageListCount(pagination);
            return new ApiResponse<Pagination<MessageSentListDto>>(ResponseStatusEnum.Success, new Pagination<MessageSentListDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<MessageRecipientDto>>> GetRecipientList(PaginationFormDto pagination)
        {
            var data = await _messageRepository.GetRecipientList(pagination);
             if(data == null)
             {
                 return new ApiResponse<Pagination<MessageRecipientDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MessageRecipientGetting));
             }
            var count = await _messageRepository.GetRecipientListCount(pagination);
            return new ApiResponse<Pagination<MessageRecipientDto>>(ResponseStatusEnum.Success, new Pagination<MessageRecipientDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> UnreadMessage(int recipientId)
        {
            var data = await _messageRepository.UnreadMessage(recipientId);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.MessageUnread));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ReplyMessage(SerializMessageDto messageDto)
        {
            var messageObj = Extentions.Deserialize<MessageAddDto>(messageDto.Message);
            if (messageObj == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ShopDeserialize));
            }
            if (messageDto.Attachment != null)
            {
                if (messageDto.Attachment.Count != messageObj.TMessageAttachment.Count)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                }
                for (int i = 0; i < messageDto.Attachment.Count; i++)
                {
                    messageObj.TMessageAttachment[i].FileUrl = _fileUploadService.UploadImage(messageDto.Attachment[i], Pathes.MessageTemp);
                    if (messageObj.TMessageAttachment[i].FileUrl == null)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            _fileUploadService.DeleteImage(messageObj.TMessageAttachment[j].FileUrl, Pathes.MessageTemp);
                        }
                        return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                    }
                }
            }
            messageObj.FkSenderId = token.UserId;
            var addData = await _messageRepository.ReplyMessage(_mapper.Map<TMessage>(messageObj));
            if (addData == null)
            {
                foreach (var item in messageObj.TMessageAttachment)
                {
                    _fileUploadService.DeleteImage(item.FileUrl, Pathes.MessageTemp);
                }
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.MessageAdding));
            }
            foreach (var item in addData.TMessageAttachment)
            {
                _fileUploadService.ChangeDestOfFile(item.FileUrl, Pathes.MessageTemp, Pathes.Message + addData.MessageId + "/");
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> SendMessage(SerializAddMessageDto messageDto)
        {
            var messageObj = Extentions.Deserialize<MessageAddDto>(messageDto.Message);
            if (messageObj == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ShopDeserialize));
            }
            var filter = Extentions.Deserialize<MessageFilterDto>(messageDto.Filter);
            if (filter == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ShopDeserialize));
            }
            if (messageDto.Attachment != null)
            {
                if (messageDto.Attachment.Count != messageObj.TMessageAttachment.Count)
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                }
                for (int i = 0; i < messageDto.Attachment.Count; i++)
                {
                    messageObj.TMessageAttachment[i].FileUrl = _fileUploadService.UploadImage(messageDto.Attachment[i], Pathes.MessageTemp);
                    if (messageObj.TMessageAttachment[i].FileUrl == null)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            _fileUploadService.DeleteImage(messageObj.TMessageAttachment[j].FileUrl, Pathes.MessageTemp);
                        }
                        return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.UploadFile));
                    }
                }
            }
            messageObj.FkSenderId = token.UserId;
            var addData = await _messageRepository.SendMessage(_mapper.Map<TMessage>(messageObj),filter);
            if (addData == null)
            {
                foreach (var item in messageObj.TMessageAttachment)
                {
                    _fileUploadService.DeleteImage(item.FileUrl, Pathes.MessageTemp);
                }
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.MessageAdding));
            }
            foreach (var item in addData.TMessageAttachment)
            {
                _fileUploadService.ChangeDestOfFile(item.FileUrl, Pathes.MessageTemp, Pathes.Message + addData.MessageId + "/");
            }




            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<int>> GetInboxMessageUnreadCount()
        {
        {
            var data = await _messageRepository.GetInboxMessageUnreadCount();

            return new ApiResponse<int>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }        
    }
    }
}