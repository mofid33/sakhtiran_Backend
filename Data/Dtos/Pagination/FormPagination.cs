namespace MarketPlace.API.Data.Dtos.Pagination
{
    public class FormPagination
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string Filter { get; set; }
        public string valueId { get; set; }
        public int Id { get; set; }
        public bool? ProductCallRequest { get; set; }

    }
}