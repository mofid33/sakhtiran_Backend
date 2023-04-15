using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class FacebookResponseDto
    {
       public string id {get;set;}
        public string email {get;set;}
        public string name {get;set;}
    }
}