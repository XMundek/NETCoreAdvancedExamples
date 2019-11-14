using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRClient
{
    public class ChatUser
    {
        public string Nick { get; set; }
        public int Age { get; set; }
    }
    class Program
    {

        static string ReadValue(string label)
        {
            Console.WriteLine();
            Console.Write(label + ":");
            return Console.ReadLine();
        }
        static async Task Main(string[] args)
        {
         
            Console.WriteLine("Signal-R Sender!");

            const string url = "http://localhost:54768/hubs/chat";

            var connection = new HubConnectionBuilder().WithUrl(url)               
                .Build();

            connection.Closed += ex=> Task.Run(()=>Console.WriteLine($"Connection closed:{ex?.Message?? string.Empty}"));
            connection.On("Message", new Action<ChatUser, string>(
                (user, msg) => {
                    Console.WriteLine($"\n{user.Nick}:{msg}");
                }));
            connection.On("Attached", new Action<ChatUser>(
                user => {
                    Console.WriteLine($"\nAttached:{user.Nick}");
                }));
            Action<string> connectionAction = msg => Console.WriteLine("\n" + msg);

            connection.On("Connected", connectionAction);
            connection.On("Disconnected", connectionAction);

            var customer = new ChatUser
            {
                Nick = ReadValue("Nick"),
                Age = int.Parse(ReadValue("Age"))
            };


            Console.WriteLine("Connecting...");
            await connection.StartAsync();
            Console.WriteLine("Connected.");

            await connection.SendAsync("Attach", customer);
            while (true)
            {
                var msg = ReadValue("Send message");
                if (string.IsNullOrEmpty(msg))
                    continue;
                if (msg == "EXIT")
                {
                    await connection.StopAsync();
                    break;
                }
                if (connection.State==HubConnectionState.Connected)
                    await connection.SendAsync("Send",customer, msg);
                else
                {
                    Console.WriteLine($"Hub is in {connection.State} state");
                }
                ///await Task.Delay(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("Program finished.");
            Console.ReadLine();
            
        }
    }
}
