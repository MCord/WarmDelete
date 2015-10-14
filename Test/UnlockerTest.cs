using NUnit.Framework;
using WarmDelete;

namespace Test
{
    public class UnlockerTest
    {
        [Test]
        public void CheckAccess()
        {
            Unlocker.Allow = Unlocker.Result.Message;
            Assert.IsTrue(Unlocker.Can(Unlocker.Result.Message));
            Assert.IsFalse(Unlocker.Can(Unlocker.Result.CloseHandle));
            Unlocker.Allow = Unlocker.Result.All;
            Assert.True(Unlocker.Can(Unlocker.Result.CloseHandle));
        }
    }
}