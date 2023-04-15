namespace MarketPlace.API.Data.Dtos.Token
{
    public class TokenDto
    {
        public TokenDto(string token)
        {
            this.Token = token;
            this.Count = 0;
        }
        public TokenDto(string token,int count)
        {
            this.Token = token;
            this.Count = count;
        }
        public int Count { get; set; }
        public string Token { get; set; }
    }
}