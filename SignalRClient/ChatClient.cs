using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleExtensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRClient
{
    public class ChatClient : IDisposable
    {
        private CancellationTokenSource source = null;
        protected ConsoleManager consoleManager = null;
        protected HubConnection connection = null;
        string UserName = null;
        public ChatClient(string url)
        {
            source = new();
            consoleManager = new();
            connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();
            connection.Closed += Closed;
            connection.On<string,string>("Recieve",OnMessageRecieved);
            //connection.SendAsync("Send","hello",source.Token);
        }
        protected void OnMessageRecieved(string who,string msg){
            consoleManager.WriteLine(who+"# "+msg,who==UserName ? ConsoleColor.Cyan : ConsoleColor.White);
        }
        protected async Task Closed(Exception error){
            if(error is not null){
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(error.Message);
                    Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("Try to reconnect in 500 ms...");
                    Console.ResetColor();
                    await Task.Delay(500,source.Token);
                    await connection.StartAsync(source.Token);
                }
        }
        
        public async Task Start(string UserName){
            this.UserName = UserName;
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Connecting to server...");
            await connection.StartAsync();
            System.Console.WriteLine("Connected!");
            Console.ResetColor();
            consoleManager.Prefix = UserName;
            consoleManager.WriteLeadLine();
            while(connection.State!=HubConnectionState.Disconnected){
                string message = consoleManager.ReadLine().Trim();
                if(message=="/exit") break;
                if(!string.IsNullOrWhiteSpace(message))
                    await connection.SendAsync("Send",UserName,message,source.Token);
            }
        }
        public async Task Stop(){
            await connection.StopAsync(source.Token);
        }
        private bool isDisposed = false;
        public void Dispose()
        {
            Task.WaitAll(Stop());
            source.Cancel();
            source.Dispose();
            connection.DisposeAsync();
            isDisposed = true;
        }
        ~ChatClient(){
            if(!isDisposed){
                Dispose();
                isDisposed = true;
            }
        }
    }
}