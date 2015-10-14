using System.IO;

namespace WarmDelete
{
    public static class FileService
    {
        public static bool IsDirectory(string path)
        {
            var fileAttributes = File.GetAttributes(path);
            return (fileAttributes & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}