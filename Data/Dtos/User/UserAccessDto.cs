using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class UserAccessDto
    {

        public Guid UserId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public bool IsAdmin { get; set; }
        public List<AccessControlDto> TUserAccessControl { get; set; }

    }
}