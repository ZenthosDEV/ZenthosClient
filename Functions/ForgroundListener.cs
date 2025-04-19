using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ZenthosClient.Functions
{
    class ForgroundListener
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private uint _lastPid = 0;
        private bool _monitoring;
        private CancellationTokenSource _cts;
        public class AppInfo
        {
            public string Name { get; set; }
            public string Title { get; set; }
        }

        public event Action<AppInfo, int> OnAppChanged;

        public void Start()
        {
            if (_monitoring) return;

            _monitoring = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => MonitorLoop(_cts.Token));
        }

        public void Stop()
        {
            _monitoring = false;
            _cts?.Cancel();
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    IntPtr hwnd = GetForegroundWindow();
                    if (hwnd != IntPtr.Zero)
                    {
                        GetWindowThreadProcessId(hwnd, out uint pid);

                        if (pid != _lastPid)
                        {
                            _lastPid = pid;
                            try
                            {
                                var proc = Process.GetProcessById((int)pid);
                                var title = proc.MainWindowTitle.Length > 0 ? proc.MainWindowTitle : "System";
                                OnAppChanged?.Invoke(new AppInfo { Name = proc.ProcessName, Title = title }, (int)pid);
                            }
                            catch
                            {
                                OnAppChanged?.Invoke(new AppInfo { Name = "-", Title = "Unknown or Exited" }, (int)pid);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Monitor error: {ex.Message}");
                }

                await Task.Delay(500, token);
            }
        }
    }
}
