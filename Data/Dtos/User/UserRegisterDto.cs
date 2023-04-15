using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class UserRegisterDto
    {
        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Family { get; set; }        
        [Required]
        public string MobileNumber { get; set; }
        [Required]

        public string VerfiyCode { get; set; }
        [Required]

        public string RequestId { get; set; }
        public string PhoneCode { get; set; }
        public int? CountryId { get; set; }
        public string NotificationKey { get; set; }
        public int Type { get; set; } // notfication type (mobile, browser client or ..)

    }
}