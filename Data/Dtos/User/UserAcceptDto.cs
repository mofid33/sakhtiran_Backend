using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class UserAcceptDto
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }

    }
}