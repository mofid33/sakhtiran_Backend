using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class UpdateUserNotificationKeyDto
    {
        public string NotificationKey { get; set; }
        [Required]
        public int Type { get; set; }
    }
}