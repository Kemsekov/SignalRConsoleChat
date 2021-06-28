using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using ConsoleExtensions;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using var chatClient = new ChatClient("http://localhost:7000/myhub");
            Task.WaitAll(chatClient.Start( args.Length>0 && string.IsNullOrEmpty(args[0]) ? "User" : args[0]));
        }
    }
}
