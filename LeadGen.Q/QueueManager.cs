using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace LeadGen.Q
{
    public class QueueManager
    {
        public CloudQueue GetQ(string name)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=leadgen;AccountKey=+qeHG2/Z7BwWZyZM38q/yVvMKUl1V20H/X4s6f2vQbD4ll6JksoBGLfH2RfkLe35k0O908OAV9itAm66NFCjWg==");

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference(name);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();

            return queue;
        }

        public void AddListingSearch(int leadSearchId )
        {
            var q = GetQ("listingsearch");

            // Create a message and add it to the queue
            var qMessage = new CloudQueueMessage(leadSearchId.ToString());
            q.AddMessage(qMessage);
        }

        public List<string> GetListingQMessages()
        {
            var q = GetQ("listingsearch");

            var messages = q.PeekMessages(5);

            return messages.Select(message => message.AsString).ToList();
        }
    }
}
