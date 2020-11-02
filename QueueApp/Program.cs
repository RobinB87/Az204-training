using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace QueueApp
{
    class Program
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=storagera1987;AccountKey=4RPvP3x3tq44zhqCEMa127izCsCJxP+pc0WM7lwMwIGakdgO0fFWqANdiFG9VccvnVpaTxUxmToV1/W3MBLC8w==";

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                var value = String.Join(" ", args);
                await SendArticleAsync(value);
                Console.WriteLine($"Sent: {value}");
            }
            else
            {
                var value = await ReceiveArticleAsync();
                Console.WriteLine($"Received {value}");
            }
        }

        static async Task SendArticleAsync(string newsMessage)
        {
            var queue = GetQueue();

            // only the publisher should create the queue
            var createdQueue = await queue.CreateIfNotExistsAsync();
            if (createdQueue)
            {
                Console.WriteLine("The queue of news articles was created.");
            }

            var articleMessage = new CloudQueueMessage(newsMessage);
            await queue.AddMessageAsync(articleMessage);
        }

        static async Task<string> ReceiveArticleAsync()
        {
            var queue = GetQueue();
            if (await queue.ExistsAsync())
            {
                var message = await queue.GetMessageAsync();
                if (message != null)
                {
                    var newsMessage = message.AsString;
                    await queue.DeleteMessageAsync(message);
                    return newsMessage;
                }
            }

            return "<queue empty or not created>";
        }

        static CloudQueue GetQueue()
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference("newsqueue");
        }
    }
}
