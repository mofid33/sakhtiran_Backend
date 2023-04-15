using System;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class AccessControlDto
    {

        public int UserAccessControlId { get; set; }
        public Guid FkUserId { get; set; }
        public int FkMenuItemId { get; set; }
        public string MenuItem { get; set; }
    }
}