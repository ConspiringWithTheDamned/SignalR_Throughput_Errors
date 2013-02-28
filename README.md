SignalR Throughput Issues
==============================

A single connection load test using Console Hub/Client.

Essentially we would like to use SignalR out of the web context as a publish/subscribe mechanism for real time ticking market data from within Excel.

When a user opens a sheet, a custom Excel RTD addin (the signal R client) subscribes to all market data streams in the cells on the server (the signalR hub; Currently a Windows Service).

After implementing SignalR we found that not all requests were getting from the client to the hub in our example sheet (around 2000 requests).

This example is an oversimplification of the issue but seems to consistently reproduce the error

System.Net.WebException: The underlying connection was closed: A connection that was expected to be kept alive was closed by the server. ---> System.IO.IOException: Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host. ---> System.Net.Sockets.SocketException: An existing connection was forcibly closed by the remote host
