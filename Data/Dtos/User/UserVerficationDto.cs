using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class UserVerficationDto
    {      
        [Required]
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string CaptchaToken { get; set; }
    }
}