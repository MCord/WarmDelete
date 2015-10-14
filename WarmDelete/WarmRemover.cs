using System;
using System.IO;

namespace WarmDelete
{
    public class WarmRemover
    {
        public bool VerboseMode { get; set; }
        public bool RemoveContentOnly { get; set; }

        public int Remove(string target)
        {
            try
            {
                RemoveInternal(target);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 2;
            }
        }

        internal void RemoveInternal(string target)
        {
            if (FileService.IsDirectory(target))
            {
                RemoveDirectory(target);
                return;
            }

            RemoveFile(target);
        }

        private void RemoveDirectory(string target, bool contentOnly = false)
        {
            foreach (var file in Directory.GetFiles(target))
            {
                RemoveFile(file);
            }

            foreach (var dir in Directory.GetDirectories(target))
            {
                RemoveDirectory(dir);
            }

            try
            {
                if (contentOnly)
                {
                    Log.Verbose($"Removed Content From: {target}");
                    return;
                }

                Directory.Delete(target);
            }
            catch (IOException)
            {
                Unlocker.Liberate(target);
                Directory.Delete(target);
            }
            Log.Verbose($"Removed: {target}");
        }

        private void RemoveFile(string target)
        {
            if (!File.Exists(target))
            {
                Log.Verbose($"File Not Found: {target}");
                return;
            }

            try
            {
                File.Delete(target);
            }
            catch(IOException)
            {
                Unlocker.Liberate(target);
                File.Delete(target);
            }
            Log.Verbose($"Removed: {target}");
        }
    }
}