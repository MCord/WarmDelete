using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using WarmDelete;

namespace Test
{
    public class WarmRemoverTest
    {
        [Test]
        public void RemoveFileTest()
        {
            var file = Path.GetTempFileName();
            var process = CreateLock(file);
            
            var wd = new WarmRemover();

            wd.RemoveInternal(file);
            Assert.True(process.HasExited);
        }
        [Test]
        public void RemoveDirTest()
        {
            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var sub = Path.Combine(dir, "A\\B\\C");
            Directory.CreateDirectory(sub);
            var subFile = Path.Combine(sub, "SomeFileDeepInsideDirectories.dll");


            var process = CreateLock(subFile);
            var wd = new WarmRemover();
            wd.RemoveInternal(dir);
            Assert.True(process.HasExited);
        }

        private static Process CreateLock(string file)
        {
            var asm = Assembly.Load("Naughty");
            var process = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = asm.Location,
                Arguments = file
            });

            WaitForFile(file);
            return process;
        }

        private static void WaitForFile(string file)
        {
            do
            {
                Thread.Sleep(100);
            } while (!File.Exists(file) || new FileInfo(file).Length == 0);
        }
    }
}