using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationGroupDto
    {
        public int SpecGroupId { get; set; }
        [Required]
        [MaxLength(50)]
        public string SpecGroupTitle { get; set; }
    }
}