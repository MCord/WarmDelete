using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace WarmDelete
{
    public static class Unlocker
    {
        public static Result Allow = Result.All;
        public static int SecondsToWaitForServiceStop;

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

            var success = false;
            foreach (var locker in lockers)
            {
                if(Liberate(locker, path) != Result.Failure)
                {
                    success = true;
                }
            }

            if (!success)
            {
                throw new Exception($"Can't free {path}");
            }
        }

        private static Result Liberate(RestartManager.RM_PROCESS_INFO locker, string path)
        {
            var process = Process.GetProcessById(locker.Process.dwProcessId);
            
            /*Need to access these variables after the process is terminated.*/
            var processId = process.Id;
            var processName = process.ProcessName;

            if (Can(Result.ServiceStop) && StopService(locker))
            {
                Log.Info($"Stopped windows service {locker.strServiceShortName} ({locker.strAppName}) with id {processId}.");
                return Result.Message;
            }

            if (Can(Result.Message) && SendCloseMessage(process))
            {
                Log.Info($"Closed {processName} with id {processId} via close message.");
                return Result.Message;
            }

            if (Can(Result.Kill) && Kill(process))
            {
                Log.Info($"Killed {processName} with id {processId}.");
                return Result.Kill;
            }
            return Result.Failure;
        }

        private static bool StopService(RestartManager.RM_PROCESS_INFO locker)
        {
            if (!string.IsNullOrEmpty(locker.strServiceShortName))
            {
                Log.Verbose($"Found windows service {locker.strServiceShortName}");
                var service = new ServiceController(locker.strServiceShortName);
                service.Stop();

                try
                {
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(SecondsToWaitForServiceStop));
                    return true;
                }
                catch(Exception ex)
                {
                    Log.Error($"Service {locker.strServiceShortName} failed to stop:" + ex.Message);
                    return false;
                }
            }

            return false;
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