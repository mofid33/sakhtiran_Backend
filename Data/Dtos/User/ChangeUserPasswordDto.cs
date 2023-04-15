namespace MarketPlace.API.Data.Dtos.User
{
    public class ChangeUserPasswordDto
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string UserName { get; set; }
    }
}