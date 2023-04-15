using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.User
{
    public class UserUpdateDto
    {
        [Required]
        public string UserName { get; set; }

        public string Name { get; set; }
        public string Family { get; set; }
        public string NationalCode { get; set; }
        public string BirthDate { get; set; }
        public string MobileNumber { get; set; }
        public int? FkCountryId { get; set; }
        public int? FkCityId { get; set; }    
        public int? FkProvinceId { get; set; }    
    }
}