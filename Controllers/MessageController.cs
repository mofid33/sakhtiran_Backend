using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Message;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        public IMessageService _messageService { get; }
        public MessageController(IMessageService messageService)
        {
            this._messageService = messageService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Last")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<MessageHeaderDto>>))]
        public async Task<IActionResult> GetLastFiveMessage()
        {
            var result = await _messageService.GetLastFiveMessage();
            return new Response<Pagination<MessageHeaderDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("View/{messageId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> ViewMessage([FromRoute]int messageId)
        {
            var result = await _messageService.ViewMessage(messageId);
            return new Response<int>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Sent/{messageId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<MessageGetOneDto>))]
        public async Task<IActionResult> GetSentMessageOne([FromRoute]int messageId)
        {
            var result = await _messageService.GetSentMessageOne(messageId);
            return new Response<MessageGetOneDto>().ResponseSending(result);
        }             
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Inbox/{messageId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<MessageGetOneDto>))]
        public async Task<IActionResult> GetInboxMessageOne([FromRoute]int messageId)
        {
            var result = await _messageService.GetInboxMessageOne(messageId);
            return new Response<MessageGetOneDto>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Inbox")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<MessageInboxListDto>>))]
        public async Task<IActionResult> GetInboxMessageList([FromQuery]PaginationMessageDto pagination)
        {
            var result = await _messageService.GetInboxMessageList(pagination);
            return new Response<Pagination<MessageInboxListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("GetInboxMessageUnreadCount")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> GetInboxMessageUnreadCount()
        {
            var result = await _messageService.GetInboxMessageUnreadCount();
            return new Response<int>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Sent")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<MessageSentListDto>>))]
        public async Task<IActionResult> GetSentMessageList([FromQuery]PaginationMessageDto pagination)
        {
            var result = await _messageService.GetSentMessageList(pagination);
            return new Response<Pagination<MessageSentListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Receptions")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<MessageRecipientDto>>))]
        public async Task<IActionResult> GetRecipientList([FromQuery]PaginationFormDto pagination)
        {
            var result = await _messageService.GetRecipientList(pagination);
            return new Response<Pagination<MessageRecipientDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpDelete("Unread/{recipientId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UnreadMessage([FromRoute]int recipientId)
        {
            var result = await _messageService.UnreadMessage(recipientId);
            return new Response<bool>().ResponseSending(result);
        }  

        [Authorize(Roles = "Admin,Seller")]
        [HttpPost("Reply")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ReplyMessage([FromForm] SerializMessageDto messageDto)
        {
            var result = await _messageService.ReplyMessage(messageDto);
            return new Response<bool>().ResponseSending(result);
        }  

        [Authorize(Roles = "Admin,Seller")]
        [HttpPost("Send")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> SendMessage([FromForm] SerializAddMessageDto messageDto)
        {
            var result = await _messageService.SendMessage(messageDto);
            return new Response<bool>().ResponseSending(result);
        }  
    }
}