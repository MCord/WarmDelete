using System;
using System.Diagnostics;

namespace WarmDelete
{
    public static class Unlocker
    {
        public static Result Allow = Result.All;

        [Flags]
        public enum Result
        {
            Message = 0x01,
            ServiceStop =0x10,
            CloseHandle = 0x100,
            Kill = 0x1000,
            Failure = 0x10000,
            All = 0x11111
        }
        public static void Liberate(string path)
        {
            var lockers = RestartManager.WhoIsLocking(path);

            if(lockers.Count == 0)
            {
                Log.Verbose($"Nothing is locking {path}");
                return ;
            }

            foreach (var locker in lockers)
            {
                if(Liberate(locker, path) == Result.Failure)
                {
                    throw new Exception($"Can't free {path}");
                }
            }
        }

        private static Result Liberate(RestartManager.RM_PROCESS_INFO locker, string path)
        {
            var process = Process.GetProcessById(locker.Process.dwProcessId);

            if (Can(Result.Message) && SendCloseMessage(process))
            {
                Log.Info($"Closed {process.ProcessName} with id {process.Id} via close message.");
                return Result.Message;
            }

            if (Can(Result.Kill) && Kill(process))
            {
                Log.Info($"Killed {process.ProcessName} with id {process.Id}.");
                return Result.Kill;
            }
            return Result.Failure;
        }

        private static bool Kill(Process process)
        {
            process.Kill();
            process.WaitForExit();
            return true;
        }

        internal static bool Can(Result access)
        {
            return (Allow & access) == access;
        }


        private static bool SendCloseMessage(Process process)
        {
            Messenger.SendCloseMessage(process);
            return process.WaitForExit(5000);
        }

    }
}