using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;

namespace ConsoleHub
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hub Starting...");
            using (WebApplication.Start<Startup>("http://localhost:6666"))
            {
                Console.WriteLine("Hub Started");

                Console.ReadLine();
                Console.WriteLine("Queue size {0}", SimpleHub.AsyncResponder.Queue.Count);
                SimpleHub.AsyncResponder.StartConsuming();
                
                Console.ReadLine();

                SimpleHub.AsyncResponder.Stop();
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapHubs();
        }
    }

    public class SimpleHub:Hub
    {
        public static AsyncResponder AsyncResponder = new AsyncResponder();
        private static int i;
        public void Request(Data key)
        {
            Console.WriteLine("Request Received: {0}", ++i);
            AsyncResponder.Queue.Add(key);
        }
    }


    public class AsyncResponder
    {
        public readonly BlockingCollection<Data> Queue = new BlockingCollection<Data>();
        private Task _task;

        public void StartConsuming()
        {
          _task = Task.Run(() =>
                         {
                             foreach (var item in Queue.GetConsumingEnumerable())
                             {
                                 var hubContext = GlobalHost.ConnectionManager.GetHubContext<SimpleHub>();
                                 hubContext.Clients.All.Respond(item);
                             }
                         });
        }

        public void Stop()
        {
            SimpleHub.AsyncResponder.Queue.CompleteAdding();
            Task.WaitAll(_task);
        }
    }
}
