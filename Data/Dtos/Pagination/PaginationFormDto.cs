namespace MarketPlace.API.Data.Dtos.Pagination
{
    public class PaginationFormDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Id { get; set; }
        public string Filter { get; set; }
        public int Type { get; set; }
    }
}