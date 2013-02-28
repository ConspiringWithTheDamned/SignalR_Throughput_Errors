using System;
using System.Collections.Generic;
using Domain;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client Starting...");

            var hubConnection = new HubConnection("http://localhost:6666");
            var hubProxy = hubConnection.CreateHubProxy("SimpleHub");

            HashSet<Data> received = new HashSet<Data>();
            HashSet<Data> requested = new HashSet<Data>();
            object locker = new object();

            hubProxy.On<Data>("Respond", s =>
                                                {
                                                    lock (locker)
                                                    {
                                                        received.Add(s);
                                                        Console.WriteLine("Request: {0}\t\t Response: {1}", requested.Count, received.Count);
                                                    }
                                                });
            hubConnection.Start().Wait();
            Console.WriteLine("Client Started");

            var totalRequestsToMake = 5000;
            Console.WriteLine("Making {0} Requests...", totalRequestsToMake);

            for (int i = 0; i < totalRequestsToMake; i++)
            {
                var s = Guid.NewGuid().ToString();
                requested.Add(new Data{String = s, Number = i});
            }

            foreach (var r in requested)
            {
                if (hubConnection.State != ConnectionState.Connected)
                {
                    hubConnection.Start().Wait();
                }

                hubProxy.Invoke("Request", r)
                    .ContinueWith(task =>
                                      {
                                          if (!task.IsFaulted) return;
                                            
                                          Console.WriteLine("Error making Request: {0}", r.Number);
                                          foreach (var innerException in task.Exception.InnerExceptions)
                                          {
                                              Console.WriteLine("\t{0}", innerException);
                                          }

                                      }
                    );
            }

            Console.WriteLine("Requests Complete");

            Console.ReadLine();
        }
    }
}
