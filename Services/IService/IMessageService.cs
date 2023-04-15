using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Message;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IMessageService
    {
        Task<ApiResponse<Pagination<MessageHeaderDto>>> GetLastFiveMessage();
        Task<ApiResponse<int>> ViewMessage(int messageId);
        Task<ApiResponse<MessageGetOneDto>> GetSentMessageOne(int messageId);
        Task<ApiResponse<MessageGetOneDto>> GetInboxMessageOne(int messageId);
        Task<ApiResponse<Pagination<MessageInboxListDto>>> GetInboxMessageList(PaginationMessageDto pagination);
        Task<ApiResponse<Pagination<MessageSentListDto>>> GetSentMessageList(PaginationMessageDto pagination);
        Task<ApiResponse<Pagination<MessageRecipientDto>>> GetRecipientList(PaginationFormDto pagination);
        Task<ApiResponse<bool>> UnreadMessage(int recipientId);
        Task<ApiResponse<bool>> ReplyMessage(SerializMessageDto messageDto);
        Task<ApiResponse<bool>> SendMessage(SerializAddMessageDto messageDto);
         Task<ApiResponse<int>> GetInboxMessageUnreadCount();

    }
}