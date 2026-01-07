using System;
using System.Diagnostics;
using warframe_relice_price.Utils;



namespace warframe_relice_price.WarframeTracker
{
    static class WarframeProcess
    {   
        private static string processName = "Warframe.x64";

        public static event Action<int>? WarframeStarted;
        public static event Action<int>? WarframeStopped;

        public static string GetWarframeProcessName()
        {
            return processName;
        }

        public static bool checkIfRunning()
        {
            if (!GlobalState.ProcessID.HasValue)
            {
                return false;
            }
            int PID = GlobalState.ProcessID.Value;

            try
            {
                return !Process.GetProcessById(PID).HasExited;
            }
            catch (ArgumentException)
            {
                // PID no longer exists
                GlobalState.ProcessID = null;
                return false;
            }
        }

        public static bool TryUpdateFromPolling()
        {
            bool wasRunning = GlobalState.ProcessID.HasValue && checkIfRunning();

            int? foundPid = null;

            foreach (var proc in Process.GetProcessesByName(processName))
            {
                try
                {
                    if (!proc.HasExited)
                    {
                        foundPid = proc.Id;
                        break;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            if (!foundPid.HasValue)
            {
                if (wasRunning && GlobalState.ProcessID.HasValue)
                {
                    int oldPid = GlobalState.ProcessID.Value;
                    GlobalState.ProcessID = null;
                    Logger.Log($"Warframe process with PID - [{oldPid}] has stopped (polling).");
                    WarframeStopped?.Invoke(oldPid);
                    return true;
                }

                GlobalState.ProcessID = null;
                return false;
            }

            int newPid = foundPid.Value;

            if (!wasRunning || !GlobalState.ProcessID.HasValue || GlobalState.ProcessID.Value != newPid)
            {
                GlobalState.ProcessID = newPid;
                Logger.Log($"Warframe process is starting with PID - [{newPid}] (polling).");
                WarframeStarted?.Invoke(newPid);
                return true;
            }

            GlobalState.ProcessID = newPid;
            return false;
        }
    }
}
