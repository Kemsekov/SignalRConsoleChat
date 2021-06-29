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
                    consoleManager.WriteLine(error.Message,ConsoleColor.Red);
                }
            await Task.FromResult(1);
        }
        
        public async Task Start(string UserName){
            this.UserName = UserName;
            consoleManager.Prefix = UserName;
            
            consoleManager.WriteLeadLine();
            while(!source.Token.IsCancellationRequested){
                while(connection.State==HubConnectionState.Connected){
                    string message = consoleManager.ReadLine().Trim();
                    if(message=="/exit"){
                        Console.WriteLine("Exiting...",ConsoleColor.Green);
                        return;
                    }                     
                    if(!string.IsNullOrWhiteSpace(message) && connection.State==HubConnectionState.Connected)
                        await connection.SendAsync("Send",UserName,message,source.Token);
                }
                consoleManager.WriteLine("Connecting to server...",ConsoleColor.Green);
            
                while(connection.State!=HubConnectionState.Connected){
                    try{
                        connection.StartAsync(source.Token).Wait(2000);
                    }
                    catch(Exception){

                    }
                }
                consoleManager.WriteLine("Connected!",ConsoleColor.Green);
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