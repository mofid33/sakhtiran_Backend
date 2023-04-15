using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MarketPlace.API.Data.Constants;
using Newtonsoft.Json;

namespace MarketPlace.API.FirebaseServices
{
    public class MessagingService : IMessagingService
    {
        // public async Task SendNotification(string title, string body, string tokens, string dataJsonString)
        // {
        //     try
        //     {
        //         var firebaseInstance = FirebaseApp.GetInstance("[DEFAULT]");
        //         if (firebaseInstance == null)
        //         {
        //             var defaultApp = FirebaseApp.Create(new AppOptions()
        //             {
        //                 Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
        //             });
        //             Console.WriteLine(defaultApp.Name); // "[DEFAULT]"
        //         }

        //         // var obj = new
        //         // {
        //         //     tripId = "77b53e9e-44fc-ea11-b4e0-e82a44e740ce",
        //         // };

        //         // string dataJson = null;
        //         // try
        //         // {
        //         //     if (data != null)
        //         //         dataJson = JsonConvert.SerializeObject(data);
        //         // }
        //         // catch (System.Exception)
        //         // {

        //         // }


        //         var message = new Message()
        //         {
        //             Data = new Dictionary<string, string>()
        //             {
        //                 ["HasNavigate"] = hasNavigate, // 1 true , 0 false
        //                 ["ScreenName"] = screenName,
        //                 ["Params"] = dataJson
        //             },
        //             // Data = new Dictionary<string, string>()
        //             // {
        //             //     ["HasNavigate"] = "1", // 1 true , 0 false
        //             //     ["ScreenName"] = "DriverTripDetail",
        //             //     ["Params"] = result2
        //             // },
        //             Notification = new Notification
        //             {
        //                 Title = title,
        //                 Body = body,
        //                 // ImageUrl = ""
        //             },

        //             Token = token,
        //             // Topic = "news",
        //         };
        //         var messaging = FirebaseMessaging.DefaultInstance;
        //         var result = await messaging.SendAsync(message);
        //         Console.WriteLine(result); //projects/myapp/messages/2492588335721724324
        //     }
        //     catch (System.Exception e)
        //     {
        //         Console.WriteLine(e);
        //     }

        // }

        public async Task SendMulticastNotification(string title, string body, string dataJsonString, List<string> tokens)
        {
            try
            {
                var firebaseInstance = FirebaseApp.GetInstance("[DEFAULT]");
                if (firebaseInstance == null)
                {
                    var defaultApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
                    });
                    Console.WriteLine(defaultApp.Name); // "[DEFAULT]"
                }

                var message = new MulticastMessage()
                {
                    Data = new Dictionary<string, string>()
                    {
                        ["Data"] = dataJsonString
                    },
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body,
                        // ImageUrl = ""
                    },

                    Tokens = tokens,
                    // Topic = "news",
                };
                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendMulticastAsync(message);
                Console.WriteLine(result); //projects/myapp/messages/2492588335721724324
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SubscribeToTopicAsync(string topic, List<string> tokens)
        {
            try
            {
                // Subscribe the devices corresponding to the registration tokens to the
                // topic
                var firebaseInstance = FirebaseApp.GetInstance("[DEFAULT]");
                if (firebaseInstance == null)
                {
                    var defaultApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
                    });
                    Console.WriteLine(defaultApp.Name); // "[DEFAULT]"
                }

                var messaging = FirebaseMessaging.DefaultInstance;
                var response = await messaging.SubscribeToTopicAsync(
                    tokens, topic);
                // See the TopicManagementResponse reference documentation
                // for the contents of response.
                Console.WriteLine($"{response.SuccessCount} tokens were subscribed successfully");
            }
            catch (System.Exception)
            {

            }

        }

        public async Task SendToTopic(string topic, string title, string body, string dataJsonString)
        {
            try
            {
                var firebaseInstance = FirebaseApp.GetInstance("[DEFAULT]");
                if (firebaseInstance == null)
                {
                    var defaultApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
                    });
                    Console.WriteLine(defaultApp.Name); // "[DEFAULT]"
                }

                var message = new Message()
                {
                    Data = new Dictionary<string, string>()
                    {
                        ["Data"] = dataJsonString
                    },
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body,
                        // ImageUrl = ""
                    },
                    Topic = topic,
                };
                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendAsync(message);
                Console.WriteLine(result); //projects/myapp/messages/2492588335721724324
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
