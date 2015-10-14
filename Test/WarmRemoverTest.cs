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
        private string tempFile = Path.GetTempFileName();
        private WarmRemover subject = new WarmRemover();

        [Test]
        public void RemoveFileTest()
        {
            Unlocker.Allow =Unlocker.Result.Kill;
            var process = CreateLock(tempFile);
            subject.RemoveInternal(tempFile);
            Assert.True(process.HasExited);
        }
        [Test]
        public void RemoveDirTest()
        {
            Unlocker.Allow = Unlocker.Result.Kill;
            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var sub = Path.Combine(dir, "A\\B\\C");
            Directory.CreateDirectory(sub);
            var subFile = Path.Combine(sub, "SomeFileDeepInsideDirectories.dll");
            var process = CreateLock(subFile);
            subject.RemoveInternal(dir);
            Assert.True(process.HasExited);
        }

        [Test]
        public void CloseWindowsApplicationUsingMessages()
        {
            Unlocker.Allow = Unlocker.Result.Message;

            var process = CreateLock(tempFile, false);
            var wd = new WarmRemover();
            wd.Remove(tempFile);
            Assert.True(process.HasExited);
        }

        private static Process CreateLock(string file, bool hidden = true)
        {
            var asm = Assembly.Load("Naughty");
            var process = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = hidden,
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
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