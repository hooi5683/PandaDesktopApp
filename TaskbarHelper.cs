using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PandaDesktopApp
{
    public static class TaskbarHelper
    {
        [DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

        private const int ABM_GETTASKBARPOS = 0x00000005;

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left, top, right, bottom;
        }

        public static Rect GetTaskbarPosition()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(typeof(APPBARDATA));
            IntPtr result = SHAppBarMessage(ABM_GETTASKBARPOS, ref abd);
            if (result == IntPtr.Zero)
                throw new InvalidOperationException("Failed to get taskbar position.");

            return new Rect(abd.rc.left, abd.rc.top, abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top);
        }

        public static double GetTaskbarY()
        {
            var rect = GetTaskbarPosition();
            return rect.Top;
        }
    }
}
