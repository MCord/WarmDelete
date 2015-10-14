using System;

namespace WarmDelete
{
    public static class Log
    {
        public static Action<string> Info = Console.WriteLine;
        public static Action<string> Verbose = msg =>
        {
            //NOP
        };

        public static void Enable()
        {
            Verbose = Console.WriteLine;
        }

    }
}