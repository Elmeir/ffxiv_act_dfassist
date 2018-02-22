using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace DFAssist
{
    public static class NativeMethods
    {
        /// <summary>
        /// Gets the handle of the window that currently has focus.
        /// </summary>
        /// <returns>
        /// The handle of the window that currently has focus.
        /// </returns>
        [DllImport("user32")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Activates the specified window.
        /// </summary>
        /// <param name="hWnd">
        /// The handle of the window to be focused.
        /// </param>
        /// <returns>
        /// True if the window was focused; False otherwise.
        /// </returns>
        [DllImport("user32")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Windows API function to animate a window.
        /// </summary>
        [DllImport("user32")]
        public static extern bool AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);

        // Activate or minimize a window
        [DllImport("User32.DLL")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("Iphlpapi.dll", SetLastError = true)]
        public static extern uint GetExtendedTcpTable(IntPtr tcpTable, ref int tcpTableLength, bool sort, AddressFamily ipVersion, int tcpTableType, int reserved);
    }
}