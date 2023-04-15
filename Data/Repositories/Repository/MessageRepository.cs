using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Message;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class MessageRepository : IMessageRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IEmailService _emailService { get; }
        public IWebHostEnvironment hostingEnvironment;
        public MessageRepository(MarketPlaceDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IEmailService emailService,
        IWebHostEnvironment environment)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
            this._emailService = emailService;
            hostingEnvironment = environment;
        }

        public async Task<int> GetAllMessageCount()
        {
            try
            {
                return await _context.TMessageRecipient.AsNoTracking().CountAsync(x => x.FkRecieverId == token.UserId && x.ViewFlag == false);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<MessageHeaderDto>> GetLastFiveMessage()
        {
            try
            {
                return await _context.TMessageRecipient
                .Where(x => x.FkRecieverId == token.UserId && x.ViewFlag == false)
                .Include(x => x.FkMessage)
                .OrderByDescending(x => x.FkMessage.SendDateTime)
                .Take(5)
                .Select(x => new MessageHeaderDto()
                {
                    MessageId = x.FkMessageId,
                    SendDateTime = Extentions.PersianDateString(x.FkMessage.SendDateTime),
                    Subject = JsonExtensions.JsonValue(x.FkMessage.Subject, header.Language),
                    Text = JsonExtensions.JsonValue(x.FkMessage.Text, header.Language),
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ViewMessage(int messageId)
        {
            try
            {
                var data = await _context.TMessageRecipient.FirstAsync(x => x.FkRecieverId == token.UserId && x.FkMessageId == messageId);
                data.ViewFlag = true;
                data.ViewDateTime = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<MessageGetOneDto> GetSentMessageOne(int messageId)
        {
            try
            {
                return await _context.TMessage
                .Where(x => x.MessageId == messageId && x.FkSenderId == token.UserId)
                .Include(t => t.FkSender)
                .Include(t => t.TMessageAttachment)
                .Select(x => new MessageGetOneDto()
                {
                    MessageId = x.MessageId,
                    SendDateTime = Extentions.PersianDateString(x.SendDateTime),
                    FkSenderId = x.FkSenderId,
                    ViewFlag = true,
                    TMessageAttachment = x.TMessageAttachment.Select(t => new MessageAttachmentDto()
                    {
                        AttachmentId = t.AttachmentId,
                        FileUrl = t.FileUrl,
                        FkMessageId = t.FkMessageId,
                        Title = t.Title
                    }).ToList(),
                    Subject = JsonExtensions.JsonValue(x.Subject, header.Language),
                    Text = JsonExtensions.JsonValue(x.Text, header.Language),
                    Name = null
                })
                .AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<MessageGetOneDto> GetInboxMessageOne(int messageId)
        {
            try
            {
                return await _context.TMessageRecipient
                .Where(x => x.FkMessageId == messageId && x.FkRecieverId == token.UserId)
                .Include(x => x.FkMessage).ThenInclude(t => t.FkSender).ThenInclude(p => p.FkCustumer)
                .Include(x => x.FkMessage).ThenInclude(t => t.FkSender).ThenInclude(p => p.FkShop)
                .Include(x => x.FkMessage).ThenInclude(t => t.TMessageAttachment)
                .Select(x => new MessageGetOneDto()
                {
                    MessageId = x.FkMessageId,
                    SendDateTime = Extentions.PersianDateString(x.FkMessage.SendDateTime),
                    FkSenderId = x.FkMessage.FkSenderId,
                    ViewFlag = x.ViewFlag,
                    TMessageAttachment = x.FkMessage.TMessageAttachment.Select(t => new MessageAttachmentDto()
                    {
                        AttachmentId = t.AttachmentId,
                        FileUrl = t.FileUrl,
                        FkMessageId = t.FkMessageId,
                        Title = t.Title
                    }).ToList(),
                    Name = (x.FkMessage.FkSender.FkCustumer != null ? (x.FkMessage.FkSender.FkCustumer.Name + " " + x.FkMessage.FkSender.FkCustumer.Family) : (x.FkMessage.FkSender.FkShop != null ? (x.FkMessage.FkSender.FkShop.StoreName) : ("ادمین"))),
                    Subject = JsonExtensions.JsonValue(x.FkMessage.Subject, header.Language),
                    Text = JsonExtensions.JsonValue(x.FkMessage.Text, header.Language),
                })
                .AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<MessageInboxListDto>> GetInboxMessageList(PaginationMessageDto pagination)
        {
            try
            {
                return await _context.TMessageRecipient
                .Include(x => x.FkMessage)
                .Include(x => x.FkMessage).ThenInclude(t => t.FkSender).ThenInclude(p => p.FkCustumer)
                .Include(x => x.FkMessage).ThenInclude(t => t.FkSender).ThenInclude(p => p.FkShop)
                .Where(x => x.FkRecieverId == token.UserId 
                && (pagination.ReadOrNot == -1 ? true : (pagination.ReadOrNot == 1 ? x.ViewFlag == true : x.ViewFlag == false))
                && (string.IsNullOrWhiteSpace(pagination.Subject) ? true : x.FkMessage.Subject.Contains(pagination.Subject))
                && (string.IsNullOrWhiteSpace(pagination.Sender) ? true :
                 (x.FkMessage.FkSender.FkCustumer != null ? (x.FkMessage.FkSender.FkCustumer.Name + " " + x.FkMessage.FkSender.FkCustumer.Family).Contains(pagination.Sender)
                  : (x.FkMessage.FkSender.FkShop != null ? (x.FkMessage.FkSender.FkShop.StoreName).Contains(pagination.Sender) : pagination.Sender == "ادمین"))) &&
                  (pagination.FromDate == (DateTime?)null ? true : x.FkMessage.SendDateTime >= pagination.FromDate) &&
                  (pagination.ToDate == (DateTime?)null ? true : x.FkMessage.SendDateTime <= pagination.ToDate)                 
                )
                .OrderByDescending(x => x.ViewFlag == false).ThenByDescending(x => x.FkMessage.SendDateTime)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new MessageInboxListDto()
                {
                    MessageId = x.FkMessageId,
                    SendDateTime = Extentions.PersianDateString((DateTime)x.FkMessage.SendDateTime),
                    FkSenderId = x.FkMessage.FkSenderId,
                    ViewFlag = x.ViewFlag,
                    HasAttachment = x.FkMessage.TMessageAttachment.Any(),
                    Name = (x.FkMessage.FkSender.FkCustumer != null ? (x.FkMessage.FkSender.FkCustumer.Name + " " + x.FkMessage.FkSender.FkCustumer.Family) : (x.FkMessage.FkSender.FkShop != null ? (x.FkMessage.FkSender.FkShop.StoreName) : ("ادمین"))),
                    Subject = JsonExtensions.JsonValue(x.FkMessage.Subject, header.Language),
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetInboxMessageListCount(PaginationMessageDto pagination)
        {
            try
            {
                return await _context.TMessageRecipient
                .AsNoTracking()
                .Include(x => x.FkMessage)
                .Include(x => x.FkMessage).ThenInclude(t => t.FkSender).ThenInclude(p => p.FkCustumer)
                .Include(x => x.FkMessage).ThenInclude(t => t.FkSender).ThenInclude(p => p.FkShop)
                .CountAsync(x => x.FkRecieverId == token.UserId 
                && (pagination.ReadOrNot == -1 ? true : (pagination.ReadOrNot == 1 ? x.ViewFlag == true : x.ViewFlag == false))
                && (string.IsNullOrWhiteSpace(pagination.Subject) ? true : x.FkMessage.Subject.Contains(pagination.Subject))
                && (string.IsNullOrWhiteSpace(pagination.Sender) ? true :
                 (x.FkMessage.FkSender.FkCustumer != null ? (x.FkMessage.FkSender.FkCustumer.Name + " " + x.FkMessage.FkSender.FkCustumer.Family).Contains(pagination.Sender)
                  : (x.FkMessage.FkSender.FkShop != null ? (x.FkMessage.FkSender.FkShop.StoreName).Contains(pagination.Sender) :  pagination.Sender == "ادمین"))) &&
                    (pagination.FromDate == (DateTime?)null ? true : x.FkMessage.SendDateTime >= pagination.FromDate) &&
                  (pagination.ToDate == (DateTime?)null ? true : x.FkMessage.SendDateTime <= pagination.ToDate)    
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<MessageSentListDto>> GetSentMessageList(PaginationMessageDto pagination)
        {
            try
            {
                return await _context.TMessage
                .Include(t => t.TMessageRecipient)
                .ThenInclude(b => b.FkReciever)
                .ThenInclude(d => d.FkShop)
                .Where(x => x.FkSenderId == token.UserId
                && (pagination.ReadOrNot == -1 ? true : (pagination.ReadOrNot == 1 ?  x.TMessageRecipient.Any(c=>c.ViewFlag == true) :  x.TMessageRecipient.Any(c=>c.ViewFlag == false)))
                && (string.IsNullOrWhiteSpace(pagination.Subject) ? true : x.Subject.Contains(pagination.Subject))
                && (string.IsNullOrWhiteSpace(pagination.Sender) ? true :
                 (x.TMessageRecipient.Count() != 0 ? 
                 x.TMessageRecipient.Any(c=>c.FkReciever.FkShop.StoreName.Contains(pagination.Sender)):true)) &&
                  (pagination.FromDate == (DateTime?)null ? true : x.SendDateTime >= pagination.FromDate) &&
                  (pagination.ToDate == (DateTime?)null ? true : x.SendDateTime <= pagination.ToDate)      
                )
                .OrderByDescending(x => x.SendDateTime)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(t => t.TMessageAttachment)
                .Select(x => new MessageSentListDto()
                {
                    MessageId = x.MessageId,
                    SendDateTime = Extentions.PersianDateString(x.SendDateTime),
                    Subject = JsonExtensions.JsonValue(x.Subject, header.Language),
                    HasAttachment = x.TMessageAttachment.Any(),
                    RecipientCount = x.TMessageRecipient.Count(),
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetSentMessageListCount(PaginationMessageDto pagination)
        {
            try
            {
                return await _context.TMessage
                .Include(t => t.TMessageRecipient)
                .ThenInclude(b => b.FkReciever)
                .ThenInclude(d => d.FkShop)
                .AsNoTracking()
                .CountAsync(x => x.FkSenderId == token.UserId
                && (pagination.ReadOrNot == -1 ? true : (pagination.ReadOrNot == 1 ?  x.TMessageRecipient.Any(c=>c.ViewFlag == true) :  x.TMessageRecipient.Any(c=>c.ViewFlag == false)))
                && (string.IsNullOrWhiteSpace(pagination.Subject) ? true : x.Subject.Contains(pagination.Subject))
                && (string.IsNullOrWhiteSpace(pagination.Sender) ? true :
                 (x.TMessageRecipient.Count() != 0 ? 
                 x.TMessageRecipient.Any(c=>c.FkReciever.FkShop.StoreName.Contains(pagination.Sender)):true)) &&
                  (pagination.FromDate == (DateTime?)null ? true : x.SendDateTime >= pagination.FromDate) &&
                  (pagination.ToDate == (DateTime?)null ? true : x.SendDateTime <= pagination.ToDate)      
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<MessageRecipientDto>> GetRecipientList(PaginationFormDto pagination)
        {
            try
            {
                return await _context.TMessageRecipient
                .Include(x => x.FkMessage)
                .Where(x => x.FkMessageId == pagination.Id && x.FkMessage.FkSenderId == token.UserId)
                .OrderByDescending(x => x.RecipientId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkReciever).ThenInclude(p => p.FkCustumer)
                .Include(x => x.FkReciever).ThenInclude(p => p.FkShop)
                .Include(x => x.FkReciever).ThenInclude(p => p.FkUserGroup)
                .Select(x => new MessageRecipientDto()
                {
                    ViewFlag = x.ViewFlag,
                    RecipientId = x.RecipientId,
                    Type = x.FkReciever.FkUserGroup.UserGroupTitle == "Vendor" ? "تامین کننده" : (x.FkReciever.FkUserGroup.UserGroupTitle == "Admin" ? "ادمین" : "مشتری"),
                    Name = (x.FkReciever.FkCustumer != null ? (x.FkReciever.FkCustumer.Name + " " + x.FkReciever.FkCustumer.Family) : (x.FkReciever.FkShop != null ? (x.FkReciever.FkShop.StoreName) : ("ادمین"))),
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetRecipientListCount(PaginationFormDto pagination)
        {
            try
            {
                return await _context.TMessageRecipient
                .Include(x => x.FkMessage)
                .AsNoTracking()
                .CountAsync(x => x.FkMessageId == pagination.Id && x.FkMessage.FkSenderId == token.UserId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> UnreadMessage(int recipientId)
        {
            try
            {
                var data = await _context.TMessageRecipient.Include(x => x.FkMessage).FirstAsync(x => x.RecipientId == recipientId && x.ViewFlag == false && x.FkMessage.FkSenderId == token.UserId);
                _context.TMessageRecipient.Remove(data);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<TMessage> ReplyMessage(TMessage message)
        {
            try
            {
                var senderMessage = await _context.TMessage.FirstAsync(x => x.MessageId == message.FkInResponseMessageId);
                var senderUser = await _context.TUser
                .Include(x => x.FkCustumer)
                .ThenInclude(x => x.FkCountry)
                .Include(x => x.FkShop)
                .FirstAsync(x => x.UserId == senderMessage.FkSenderId);
                message.TMessageRecipient = new List<TMessageRecipient>();
                var MessageRecipient = new TMessageRecipient();
                MessageRecipient.FkRecieverId = senderMessage.FkSenderId;
                MessageRecipient.ViewFlag = false;
                message.Subject = JsonExtensions.JsonAdd(message.Subject, header);
                message.Text = JsonExtensions.JsonAdd(message.Text, header);
                message.TMessageRecipient.Add(MessageRecipient);
                message.SendDateTime = DateTime.Now;
                if (message.Email && !string.IsNullOrEmpty(senderUser.UserName) && !string.IsNullOrEmpty(message.Text))
                {
                    string emailPath = Path.Combine(hostingEnvironment.ContentRootPath, "emailTemplate/description-email.html");
                    string subject = "sakhtiran.com - " + message.Subject;
                    string text = File.ReadAllText(emailPath);
                    text = text.Replace("#description", message.Text);
                    var resultEmail = await _emailService.Send(senderUser.UserName, subject, text);
                }
                if (message.Sms)
                {
                    try
                    {
                        var mobileNumber = senderUser.FkCustumer != null ? (senderUser.FkCustumer.FkCountry.PhoneCode + senderUser.FkCustumer.MobileNumber)
                           : (senderUser.FkShop != null ? senderUser.FkShop.Phone : null);

                        if (mobileNumber != null)
                        {
                            Extentions.SendPodinisSmsForProvider(message.Text, mobileNumber);
                        }
                    }
                    catch (System.Exception)
                    {

                    }

                }
                await _context.TMessage.AddAsync(message);
                await _context.SaveChangesAsync();
                return message;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TMessage> SendMessage(TMessage message, MessageFilterDto filter)
        {
            try
            {
                var userIds = new List<TUser>();
                if (token.Rule == UserGroupEnum.Admin)
                {
                    if (filter.IsSingle == true)
                    {
                        userIds = await _context.TUser
                             .Include(x => x.FkCustumer)
                            .ThenInclude(x => x.FkCountry)
                            .Include(x => x.FkShop)
                        .Where(x => filter.UserId.Contains(x.UserId)).ToListAsync();
                    }
                    else
                    {
                        if (filter.UserType == (int)MessageUserType.AllUser)
                        {
                            userIds = await _context.TUser
                            .Include(x => x.FkCustumer)
                            .ThenInclude(x => x.FkCountry)
                            .Include(x => x.FkShop)
                            .Where(x =>
                            x.FkUserGroupId != Guid.Parse(GroupTypes.Admin) &&
                            (filter.Country.Count > 0 ? ((filter.Country.Contains(x.FkCustumer.FkCountryId == null ? 0 : (int)x.FkCustumer.FkCountryId)) || filter.Country.Contains(x.FkShop.FkCountryId)) : true) &&
                            (filter.Province.Count > 0 ? ((filter.Province.Contains(x.FkCustumer.FkProvinceId == null ? 0 : (int)x.FkCustumer.FkProvinceId)) || filter.Province.Contains((int)x.FkShop.FkProvinceId)) : true) &&
                            (filter.City.Count > 0 ? ((filter.City.Contains(x.FkCustumer.FkCityId == null ? 0 : (int)x.FkCustumer.FkCityId)) || filter.City.Contains((int)x.FkShop.FkCityId)) : true) &&
                            (filter.PlanId.Count > 0 ? filter.PlanId.Contains(x.FkShop.FkPlanId == null ? 0 : (int)x.FkShop.FkPlanId) : true)
                            ).AsNoTracking().ToListAsync();
                        }
                        else if (filter.UserType == (int)MessageUserType.AllVendors)
                        {
                            userIds = await _context.TUser
                            .Include(x => x.FkShop)
                            .Where(x =>
                            x.FkUserGroupId != Guid.Parse(GroupTypes.Admin) &&
                            (filter.Country.Count > 0 ? (filter.Country.Contains(x.FkShop.FkCountryId)) : true) &&
                            (filter.City.Count > 0 ? (filter.City.Contains((int)x.FkShop.FkCityId)) : true) &&
                            (filter.Province.Count > 0 ? (filter.Province.Contains((int)x.FkShop.FkProvinceId)) : true) &&
                            (filter.PlanId.Count > 0 ? filter.PlanId.Contains(x.FkShop.FkPlanId == null ? 0 : (int)x.FkShop.FkPlanId) : true)
                            ).AsNoTracking().ToListAsync();
                        }
                        else if (filter.UserType == (int)MessageUserType.AllCustomer)
                        {
                            userIds = await _context.TUser
                            .Include(x => x.FkCustumer)
                            .ThenInclude(x => x.FkCountry)
                            .Where(x =>
                            x.FkUserGroupId != Guid.Parse(GroupTypes.Admin) &&
                            (filter.Country.Count > 0 ? ((filter.Country.Contains(x.FkCustumer.FkCountryId == null ? 0 : (int)x.FkCustumer.FkCountryId))) : true) &&
                            (filter.City.Count > 0 ? ((filter.City.Contains(x.FkCustumer.FkCityId == null ? 0 : (int)x.FkCustumer.FkCityId))) : true) &&
                            (filter.Province.Count > 0 ? ((filter.Province.Contains(x.FkCustumer.FkProvinceId == null ? 0 : (int)x.FkCustumer.FkProvinceId))) : true)
                            ).AsNoTracking().ToListAsync();
                        }
                    }
                }
                else if (token.Rule == UserGroupEnum.Seller)
                {
                    // if (filter.IsSingle == true)
                    // {
                    //     userIds = await _context.TUser
                    //                                 .Include(x => x.FkCustumer)
                    //         .ThenInclude(x => x.FkCountry)
                    //         .Include(x => x.FkShop)
                    //     .Where(x => filter.UserId.Contains(x.UserId)).ToListAsync();
                    // }
                    // else
                    // {
                    // if (filter.UserType == (int)MessageUserType.Admin)
                    // {
                    userIds = await _context.TUser
                    .Include(x => x.FkCustumer)
                    .ThenInclude(x => x.FkCountry)
                    .Include(x => x.FkShop)
                    .Where(x => x.UserId == Guid.Parse(UserAdminId.ID)).AsNoTracking().ToListAsync();
                    // }
                    //     else if (filter.UserType == (int)MessageUserType.AllCustomer)
                    //     {
                    //         userIds = await _context.TUser
                    //         .Include(x => x.FkCustumer)
                    //         .ThenInclude(x => x.FkCountry)
                    //         .Include(x => x.FkCustumer).ThenInclude(x => x.TOrder).ThenInclude(t => t.TOrderItem)
                    //         .Where(x =>
                    //         x.FkCustumerId != null &&
                    //         (x.FkCustumer.TOrder.Any(t => t.TOrderItem.Any(i => i.FkShopId == token.Id && i.FkStatusId != (int)OrderStatusEnum.Cart))) && x.FkCustumerId == (int)CustomerTypeEnum.Unknown &&
                    //         (filter.Country.Count > 0 ? ((filter.Country.Contains(x.FkCustumer.FkCountryId == null ? 0 : (int)x.FkCustumer.FkCountryId))) : true) &&
                    //         (filter.City.Count > 0 ? ((filter.City.Contains(x.FkCustumer.FkCityId == null ? 0 : (int)x.FkCustumer.FkCityId))) : true) &&
                    //         (filter.Province.Count > 0 ? ((filter.Province.Contains(x.FkCustumer.FkProvinceId == null ? 0 : (int)x.FkCustumer.FkProvinceId))) : true)
                    //         ).AsNoTracking().ToListAsync();
                    //     }
                    // }


                }

                if (userIds.Count < 1)
                {
                    return null;
                }


                message.SendDateTime = DateTime.Now;
                message.FkInResponseMessageId = null;

                message.TMessageRecipient = new List<TMessageRecipient>();
                foreach (var item in userIds)
                {
                    var MessageRecipient = new TMessageRecipient();
                    MessageRecipient.FkRecieverId = item.UserId;
                    MessageRecipient.ViewFlag = false;
                    message.TMessageRecipient.Add(MessageRecipient);
                    if (message.Email && !string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(message.Text))
                    {
                        string emailPath = Path.Combine(hostingEnvironment.ContentRootPath, "emailTemplate/description-email.html");
                        string subject = "sakhtiran.com - " + message.Subject;
                        string text = File.ReadAllText(emailPath);
                        text = text.Replace("#description", message.Text);
                        var resultEmail = await _emailService.Send(item.UserName, subject, text);
                    }
                    if (message.Sms)
                    {
                        try
                        {
                            var mobileNumber = item.FkCustumer != null ? (item.FkCustumer.FkCountry.PhoneCode + item.FkCustumer.MobileNumber)
                               : (item.FkShop != null ? item.FkShop.Phone : null);

                            if (mobileNumber != null)
                            {
                                Extentions.SendPodinisSmsForProvider(message.Text, mobileNumber);
                            }
                        }
                        catch (System.Exception)
                        {

                        }

                    }
                }
                message.Subject = JsonExtensions.JsonAdd(message.Subject, header);
                message.Text = JsonExtensions.JsonAdd(message.Text, header);
                await _context.TMessage.AddAsync(message);
                await _context.SaveChangesAsync();
                return message;
            }
            catch (System.Exception)
            {
                return null;
            }
        }




        public async Task<int> GetInboxMessageUnreadCount()
        {
            try
            {
                return await _context.TMessageRecipient
                .Include(c => c.FkMessage)
                .AsNoTracking()
                .CountAsync(x => x.FkRecieverId == token.UserId && !x.ViewFlag);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }





        public async Task<bool> SendMessageToAdmin(string text, string subject)
        {
            try
            {
                var adminUser = await _context.TUser.
                Include(v => v.TUserAccessControl)
                    .Where(x => (x.FkUserGroupId == Guid.Parse(GroupTypes.Admin) && x.UserId == Guid.Parse(UserAdminId.ID)) ||
                          (x.TUserAccessControl.Any(b => b.FkMenuItemId == 41 && b.FkUserId != Guid.Parse(UserAdminId.ID)))).ToListAsync();
                // 41 modiri ke be message dasrasi dare payamo bebine

                var message = new TMessage();
                message.FkSenderId = adminUser[0].UserId;
                message.SendDateTime = DateTime.Now;
                message.FkInResponseMessageId = null;
                message.TMessageRecipient = new List<TMessageRecipient>();
                foreach (var item in adminUser)
                {
                    var MessageRecipient = new TMessageRecipient();
                    MessageRecipient.FkRecieverId = item.UserId;
                    MessageRecipient.ViewFlag = false;
                    message.TMessageRecipient.Add(MessageRecipient);
                }
                message.Subject = JsonExtensions.JsonAdd(subject, header);
                message.Text = JsonExtensions.JsonAdd(text, header);
                await _context.TMessage.AddAsync(message);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }


        public async Task<bool> SendMessageToVendor(string text, string subject, Guid shopUserId)
        {
            try
            {
                var adminUser = await _context.TUser.
                Include(v => v.TUserAccessControl)
                    .FirstAsync(x => (x.FkUserGroupId == Guid.Parse(GroupTypes.Admin) && x.UserId == Guid.Parse(UserAdminId.ID)));
                // 41 modiri ke be message dasrasi dare payamo bebine

                var message = new TMessage();
                message.FkSenderId = adminUser.UserId;
                message.SendDateTime = DateTime.Now;
                message.FkInResponseMessageId = null;
                message.TMessageRecipient = new List<TMessageRecipient>();
                var MessageRecipient = new TMessageRecipient();
                MessageRecipient.FkRecieverId = shopUserId;
                MessageRecipient.ViewFlag = false;
                message.TMessageRecipient.Add(MessageRecipient);
                message.Subject = JsonExtensions.JsonAdd(subject, header);
                message.Text = JsonExtensions.JsonAdd(text, header);
                await _context.TMessage.AddAsync(message);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }








    }
}