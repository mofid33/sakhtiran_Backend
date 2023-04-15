namespace MarketPlace.API.Data.Dtos.Pagination
{
    public class PaginationUserDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}