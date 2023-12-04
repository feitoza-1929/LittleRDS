using System.Net;

Server server = new(IPAddress.Parse("127.0.0.1"), 6643);
server.Start();