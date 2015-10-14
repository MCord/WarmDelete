using System;
using System.Reflection;
using WarmDelete;
using Xunit;

namespace Test
{
    public class FileServiceTest
    {
        [Fact]
        public void IsDirectory()
        {
            Assert.True(FileService.IsDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
            Assert.False(FileService.IsDirectory(Assembly.GetExecutingAssembly().Location));
        }
    }
}