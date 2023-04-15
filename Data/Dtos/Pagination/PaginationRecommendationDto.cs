namespace MarketPlace.API.Data.Dtos.Pagination
{
    public class PaginationRecommendationDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
    }
}