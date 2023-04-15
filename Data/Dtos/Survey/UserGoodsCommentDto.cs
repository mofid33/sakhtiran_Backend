using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Survey
{
    public class UserGoodsCommentDto
    {
        public GoodsCommentGetDto Comment { get; set; }
        public GoodsBaseDetailDto Goods { get; set; }
        public List<ShopSurveyQuestionsDto> Questions { get; set; }
    }
}