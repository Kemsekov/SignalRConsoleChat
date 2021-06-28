using System;
using System.Text;

namespace ConsoleExtensions
{
    public class ConsoleManager
    {
        public string LeadLine{get=>Prefix+PostFix;}
        public string Prefix{get;set;} = "User";
        public string PostFix{get;set;} = ":";
        public ConsoleColor LeadLineColor = ConsoleColor.White;
        protected StringBuilder Input = new StringBuilder();
        public ConsoleManager()
        {
        }

        public event Action<ConsoleKey> OnKey{
            add{
                onKey+=value;
            }
            remove{
                onKey-=value;
            }}
        protected Action<ConsoleKey> onKey;

        ///<param name="endLineKey">The key that specify when line is end</param>
        ///<summary>Reads a line</summary>
        public string ReadLine(ConsoleKey endLineKey = ConsoleKey.Enter){
            lock(Input){
                ConsoleKeyInfo c = default;
                onKey?.Invoke(c.Key);
                while(c.Key!=endLineKey){
                    c = Console.ReadKey(true);
                    switch(c.Key){
                        case ConsoleKey.Backspace:
                            if(Input.Length>0)
                                Input.Remove(Input.Length-1,1);
                            break;
                        default:
                            Input.Append(c.KeyChar);
                            break;
                    }
                    WriteLeadLine();
                }
                string res = Input.ToString();
                Input.Clear();
                WriteLeadLine();
                return res;
            }
        }
        protected void ClearCurrentLine(){
            Console.SetCursorPosition(0,Console.CursorTop);
            System.Console.Write(new string(' ',Console.WindowWidth-1));
            Console.SetCursorPosition(0,Console.CursorTop);
        }
        public void WriteLeadLine(){
            string msg = LeadLine+Input.ToString();
            ClearCurrentLine();
            System.Console.Write(msg);
        }
        public void WriteLine(string message,ConsoleColor messageColor = ConsoleColor.White){
            ClearCurrentLine();
            var pos = Console.GetCursorPosition();
            Console.ForegroundColor = messageColor;
            Console.SetCursorPosition(pos.Left,pos.Top);
            System.Console.WriteLine(message);

            pos = Console.GetCursorPosition();
            Console.ForegroundColor = this.LeadLineColor;
            Console.SetCursorPosition(pos.Left,pos.Top);
            WriteLeadLine();
            
            pos = Console.GetCursorPosition();
            Console.ResetColor();
            Console.SetCursorPosition(pos.Left,pos.Top);
        }
    }
}