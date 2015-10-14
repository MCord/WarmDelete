using System.Diagnostics;
using System.IO;

namespace Naughty
{
    internal class Program
    {
        /// <summary>
        /// this application takes a path and locks it for test purposes. the process is killed if the parent process is no longer there.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var targetFile = args[0];
            System.Console.WriteLine(targetFile);
            var file = File.Create(targetFile);

            file.WriteByte(255);
            file.Flush();

            var me = Process.GetCurrentProcess();
            var dad = ParentProcessUtilities.GetParentProcess(me.Id);
            dad.WaitForExit(10000);
            file.Close();
            File.Delete(targetFile);
        }
    }
}