using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class UserMenuDto
    {
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool Expanded { get; set; }
        public int? ParentId { get; set; }

        public List<UserMenuDto> Child { get; set; }

    }
}