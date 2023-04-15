namespace MarketPlace.API.Services.IService
{
    public interface IMessageLanguageService
    {
        string MessageService(string token);
        string MessageService(string token,string language);
    }
}