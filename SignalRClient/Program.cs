using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using ConsoleExtensions;
using Microsoft.Extensions.CommandLineUtils;
using System.Linq;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication(){
                Name="SignalRClient",
                FullName="SignalR console chat",
                Description="Simple SignalR console chat impl"
            };
            
            app.HelpOption("-h|--help");
            app.VersionOption("--version","1.0.0");
            app.Option("-u","Username",CommandOptionType.SingleValue);
            app.Option("-s","Server",CommandOptionType.SingleValue);
            try{
                app.Execute(args);
            }
            catch(CommandParsingException ex){
                System.Console.WriteLine(ex.Message);
            }

            var name = app.Options.FirstOrDefault(opt=>opt.Template=="-u")?.Values.FirstOrDefault();
            var server = app.Options.FirstOrDefault(opt=>opt.Template=="-s")?.Values.FirstOrDefault();
            if(name is not null){
                using var chatClient = new ChatClient($"http://{server}/myhub");
                Task.WaitAll(chatClient.Start(name));
            }
        }
    }
}
