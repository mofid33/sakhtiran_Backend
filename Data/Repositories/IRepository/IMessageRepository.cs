using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Message;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IMessageRepository
    {
        Task<int> GetAllMessageCount();
        Task<List<MessageHeaderDto>> GetLastFiveMessage();
        Task<bool> ViewMessage(int messageId);
        Task<MessageGetOneDto> GetSentMessageOne(int messageId);
        Task<MessageGetOneDto> GetInboxMessageOne(int messageId);
        Task<List<MessageInboxListDto>> GetInboxMessageList(PaginationMessageDto pagination);
        Task<int> GetInboxMessageListCount(PaginationMessageDto pagination);
        Task<List<MessageSentListDto>> GetSentMessageList(PaginationMessageDto pagination);
        Task<int> GetSentMessageListCount(PaginationMessageDto pagination);
        Task<List<MessageRecipientDto>> GetRecipientList(PaginationFormDto pagination);
        Task<int> GetRecipientListCount(PaginationFormDto pagination);
        Task<bool> UnreadMessage(int recipientId);
        Task<TMessage> ReplyMessage(TMessage message);
        Task<TMessage> SendMessage(TMessage message,MessageFilterDto filter);
        Task<int> GetInboxMessageUnreadCount();
        Task<bool> SendMessageToAdmin(string text , string subject);
        Task<bool> SendMessageToVendor(string text , string subject , Guid shopUserId);
    }
}