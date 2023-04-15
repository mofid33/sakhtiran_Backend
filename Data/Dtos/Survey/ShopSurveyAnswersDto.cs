
namespace MarketPlace.API.Data.Dtos.Survey
{
    //
    public class ShopSurveyAnswersDto
    {
        public int AnsId { get; set; }
        public int? FkCommentId { get; set; }
        public int FkQuestionId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkShopId { get; set; }
        public int AnsValue { get; set; } 

    }
}
