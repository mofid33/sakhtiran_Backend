using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Survey
{
    public class GoodsCommentPointsDto
    {
        public int PointId { get; set; }
        public int FkCommentId { get; set; }
        [MaxLength(350)]
        public string PointText { get; set; }
        public bool PointType { get; set; }
    }
}
 