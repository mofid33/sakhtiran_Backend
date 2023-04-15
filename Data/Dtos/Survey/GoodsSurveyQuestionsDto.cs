using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos
{
    public class GoodsSurveyQuestionsDto
    {
        public int QueId { get; set; }
        public int FkCategoryId { get; set; }
        [Required]
        [MaxLength(250)]
        public string QuestionText { get; set; }
    }
}