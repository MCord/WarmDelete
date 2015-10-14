﻿using System;
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

            foreach (var locker in lockers)
            {
                Liberate(locker, path);
            }
        }

        private static Result Liberate(RestartManager.RM_PROCESS_INFO locker, string path)
        {
            var process = Process.GetProcessById(locker.Process.dwProcessId);

            Log.Info($"Killing process {process.ProcessName} with id {process.Id}");
            if (Can(Result.Message) && SendCloseMessage(process))
            {
                Log.Verbose($"Closed ${process.ProcessName} with id {process.Id} via close message.");
                return Result.Message;
            }

            if (Can(Result.Message) && Kill(process))
            {
                Log.Verbose($"Killed ${process.ProcessName} with id {process.Id}.");
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