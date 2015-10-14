using System.Diagnostics;

namespace WarmDelete
{
    public static class Unlocker
    {
        public static void Liberate(string path)
        {
            var lockers = RestartManager.WhoIsLocking(path);
            foreach (var locker in lockers)
            {
                Liberate(locker);
            }
        }

        private static void Liberate(Process locker)
        {
            Log.Info($"Killing process {locker.ProcessName} with id {locker.Id}");
            locker.Kill();
            locker.WaitForExit();
        }
    }
}