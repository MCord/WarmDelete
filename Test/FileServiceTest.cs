using System;
using System.Reflection;
using NUnit.Framework;
using WarmDelete;

namespace Test
{
    public class FileServiceTest
    {
        [Test]
        public void IsDirectory()
        {
            Assert.True(FileService.IsDirectory(Environment.GetFolderPath(Environment.SpecialFolder.System)));
            Assert.False(FileService.IsDirectory(Assembly.GetExecutingAssembly().Location));
        }
    }
}