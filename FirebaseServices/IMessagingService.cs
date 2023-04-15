using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketPlace.API.FirebaseServices
{
    public interface IMessagingService
    {
        Task SendMulticastNotification(string title, string body, string dataJsonString, List<string> tokens);
        Task SubscribeToTopicAsync(string topic, List<string> tokens);
        Task SendToTopic(string topic, string title, string body, string dataJsonString);
    }
}