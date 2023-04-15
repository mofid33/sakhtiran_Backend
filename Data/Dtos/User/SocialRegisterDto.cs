using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class SocialRegisterDto
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public int SocialType { get; set; }
    }
}