using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos
{
    public class ShopSurveyQuestionsDto
    {
        public int QueId { get; set; }
        public bool Status { get; set; }
        public string QuestionText { get; set; }
    }
}