using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryPathDto
    {
        public int CategoryId { get; set; }
        public string CategoryPath { get; set; }
    }
}