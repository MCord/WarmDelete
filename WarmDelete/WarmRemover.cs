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
            File.SetAttributes(target, FileAttributes.Normal);

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
            catch (Exception)
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
                DeleteFile(target);
            }
            catch(Exception) //Can be UnauthorizedAccessException for exe files or IOException for other in use files. 
            {
                Unlocker.Liberate(target);
                DeleteFile(target);
            }
            Log.Verbose($"Removed: {target}");
        }

        private static void DeleteFile(string target)
        {
            File.SetAttributes(target, FileAttributes.Normal);
            File.Delete(target);
        }
    }
}