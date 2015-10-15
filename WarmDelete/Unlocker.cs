using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using VmcController.Services;

namespace WarmDelete
{
    public static class Unlocker
    {
        public static Result Allow = Result.All;
        public static int SecondsToWaitForHandleRelease = 30;

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
            var handles = DetectOpenFiles.GetOpenFilesEnumerator(process.Id).Where(h => h.FilePath.StartsWith(path, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            /*Need to access these variables after the process is terminated.*/
            var processId = process.Id;
            var processName = process.ProcessName;

            if (Can(Result.ServiceStop) && StopService(locker, handles))
            {
                Log.Info($"Stopped windows service {locker.strServiceShortName} ({locker.strAppName}) with id {processId}.");
                return Result.Message;
            }

            if (Can(Result.Message) && SendCloseMessage(process, handles))
            {
                Log.Info($"Closed {processName} with id {processId} via close message.");
                return Result.Message;
            }

            if(Can(Result.CloseHandle) && CloseHandle(locker, handles, path))
            {
                Log.Info($"Closed {handles.Length} handle(s) in process {processName} with id {processId}.");
                return Result.CloseHandle;
            }
            if (Can(Result.Kill) && Kill(process))
            {
                Log.Info($"Killed {processName} with id {processId}.");
                return Result.Kill;
            }
            return Result.Failure;
        }

        private static bool CloseHandle(RestartManager.RM_PROCESS_INFO locker, DetectOpenFiles.HandleRecord[] handles, string path)
        {
            foreach (var handle in handles)
            {
                try
                {
                    handle.Close();
                }
                catch (Exception ex)
                {
                    Log.Verbose($"Error while closing handle {ex}");
                    return false;
                }
            }
            return Wait(handles);
        }

        private static bool StopService(RestartManager.RM_PROCESS_INFO locker, DetectOpenFiles.HandleRecord[] handles)
        {
            if (!string.IsNullOrEmpty(locker.strServiceShortName))
            {
                Log.Verbose($"Found windows service {locker.strServiceShortName}");
                var service = new ServiceController(locker.strServiceShortName);
                service.Stop();

                try
                {
                    return Wait(handles);
                }
                catch(Exception ex)
                {
                    Log.Error($"Service {locker.strServiceShortName} failed to stop:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        private static bool Wait(DetectOpenFiles.HandleRecord[] handles)
        {
            var start = DateTime.Now;
            var timeout = TimeSpan.FromSeconds(SecondsToWaitForHandleRelease);

            while (handles.Any(h => h.IsOpen))
            {
                if((DateTime.Now-start) > timeout)
                {
                    return false;
                }
                Thread.Sleep(100);
            }

            return true;
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


        private static bool SendCloseMessage(Process process, DetectOpenFiles.HandleRecord[] handles)
        {
            Messenger.SendCloseMessage(process);
            return Wait(handles);
        }

    }
}