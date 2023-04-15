using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationKeyAndRequiredDto
    {
        [Required]
        public bool IsKey { get; set; }
        [Required]
        public bool Value { get; set; }
        [Required]
        public int SpecId { get; set; }
    }
}