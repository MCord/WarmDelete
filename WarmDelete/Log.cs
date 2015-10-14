using System;

namespace WarmDelete
{
    public static class Log
    {
        public static void Info(string messgae)
        {
            Console.WriteLine(messgae);
        } 
        public static Action<string> Verbose = msg =>
        {
            //NOP
        };

        public static void EnableVerboseMode()
        {
            Verbose = Console.WriteLine;
        }

        public static void Error(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}