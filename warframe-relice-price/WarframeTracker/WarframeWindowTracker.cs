using System;
using System.Diagnostics;

using warframe_relice_price.Utils;

// This class is to track the Warframe game window and its properties
namespace warframe_relice_price.WarframeTracker
{
    class WarframeWindowTracker
    {
        public IntPtr GetWarframeWindow() 
        {
            if (!GlobalState.ProcessID.HasValue || !WarframeProcess.checkIfRunning())
            {
                return IntPtr.Zero;
            }

            try
            {
                var pid = GlobalState.ProcessID.Value;
                var proc = Process.GetProcessById(pid);
                return proc.MainWindowHandle;
            }
            catch (ArgumentException)
            {
                // PID no longer exists
                GlobalState.ProcessID = null;
                return IntPtr.Zero;
            }
            catch (InvalidOperationException)
            {
                return IntPtr.Zero;
            }

        }

        public bool TryGetBounds(IntPtr hwnd, out Win32.RECT rect)
        {
            return Win32.GetWindowRect(hwnd, out rect);
        }
    }

}
