
using LittleRDS.PubSub.Subscribers;
using LittleRDS.Server;


// Subscribers
AOFSub.GetInstance();

// Server
Server server = new();
server.Start();