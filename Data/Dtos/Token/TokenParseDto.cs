using System;
using System.Security.Claims;
using MarketPlace.API.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Token
{
    public class TokenParseDto
    {
        public TokenParseDto(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                UserId = new Guid(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Rule = UserGroupEnumMethods.GetUserGroupRole(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value);
                UserName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;
                Id = int.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.GroupSid).Value);
                if (!string.IsNullOrWhiteSpace(httpContextAccessor.HttpContext.Request.Headers["CookieId"]))
                {
                    CookieId = new Guid(httpContextAccessor.HttpContext.Request.Headers["CookieId"]);
                }
                else
                {
                    CookieId = Guid.Empty;
                }

            }
            catch (System.Exception)
            {
                this.Id = 0;
                this.UserId = Guid.Empty;
                this.Rule = UserGroupEnum.Customer;
                this.UserName = null;
                try
                {
                    if (!string.IsNullOrWhiteSpace(httpContextAccessor.HttpContext.Request.Headers["CookieId"]))
                    {
                        CookieId = new Guid(httpContextAccessor.HttpContext.Request.Headers["CookieId"]);
                    }
                    else
                    {
                        CookieId = Guid.Empty;
                    }
                }
                catch (System.Exception)
                {

                    CookieId = Guid.Empty;
                }
            }

        }

        public TokenParseDto()
        {
            this.CookieId = Guid.Empty;
            this.Id = 0;
            this.UserId = Guid.Empty;
            this.Rule = UserGroupEnum.Customer;
            this.UserName = null;
        }

        public Guid CookieId { get; set; }
        public Guid UserId { get; set; }
        public UserGroupEnum Rule { get; set; }
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}