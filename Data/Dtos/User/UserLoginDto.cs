using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class UserLoginDto
    {
        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
        public string CaptchaToken { get; set; }
        public string NotificationKey { get; set; }
        public int Type { get; set; } // mobile or website or provider panel
    }
}