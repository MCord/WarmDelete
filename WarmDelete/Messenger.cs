using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WarmDelete
{
    /// <summary>
    /// sends windows messages to processes.
    /// </summary>
    public class Messenger
    {
        private const UInt32 WmClose = 0x0010;

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void SendCloseMessage(Process process)
        {
            if(process.MainWindowHandle != IntPtr.Zero)
            {
                process.CloseMainWindow();
            }

            foreach (ProcessThread pt in process.Threads)
            {
                EnumThreadWindows((uint)pt.Id, EnumThreadCallback, IntPtr.Zero);
            }
        }
        static bool EnumThreadCallback(IntPtr hWnd, IntPtr lParam)
        {
            // Close the enumerated window.
            PostMessage(hWnd, WmClose, IntPtr.Zero, IntPtr.Zero);

            return true;
        }
    }
}