using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace LittleWarGameClient
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern void SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern void EnumWindows(CallBackPtr lpEnumFunc, IntPtr lParam);

        private delegate bool CallBackPtr(IntPtr hwnd, int lParam);

        private static readonly CallBackPtr callBackPtr = Callback;
        private static List<WinStruct> _WinStructList = new();

        internal static bool IsDoubleInstance = false;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var args = ParseArguments(Environment.GetCommandLineArgs()[1..]);
            Thread splashthread = new(() =>
            {
                SplashScreen.Instance.ShowDialog();
            });
            splashthread.IsBackground = true;
            splashthread.SetApartmentState(ApartmentState.STA);
            splashthread.Start();
            if (!args.TryGetValue("profile", out string? profileName))
                profileName = "main";
            using Mutex mutex = new(true, $"Global\\LittleWarGameClient_{profileName}", out bool createdNew);
            if (createdNew)
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                GameForm.InstanceName = profileName;
                ApplicationConfiguration.Initialize();
                Application.Run(OverlayForm.Instance);
            }
            else
            {
                IsDoubleInstance = true;
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        var clientWindows = GetWindows(process.Handle).Where(window => window.WinTitle == $"Littlewargame({profileName})");
                        if (clientWindows.Any())
                        {
                            var clientMainWindow = clientWindows.First();
                            SetForegroundWindow(clientMainWindow.MainWindowHandle);
                            break;
                        }
                    }
                }
            }
        }

        private static Dictionary<string, string> ParseArguments(string[] args)
        {
            args = Array.ConvertAll(args, d => d.ToLower());
            Dictionary<string, string> arguments = new();

            for (int i = 0; i < args.Length; i += 2)
            {
                if (args.Length == i + 1 || args[i + 1].StartsWith("-"))
                {
                    arguments.Add(args[i][1..], string.Empty);
                    i--;
                }
                if (args.Length >= i + 1 && !args[i + 1].StartsWith("-"))
                    arguments.Add(args[i][1..], args[i + 1]);
            }
            return arguments;
        }

        private static bool Callback(IntPtr hWnd, int lparam)
        {
            StringBuilder sb = new(256);
            GetWindowText(hWnd, sb, 256);
            if (sb.Length > 0)
                _WinStructList.Add(new WinStruct { MainWindowHandle = hWnd, WinTitle = sb.ToString() });
            return true;
        }

        private static List<WinStruct> GetWindows(IntPtr pHandle)
        {
            _WinStructList = new List<WinStruct>();
            EnumWindows(callBackPtr, pHandle);
            return _WinStructList;
        }
    }

    internal struct WinStruct
    {
        internal string? WinTitle;
        internal IntPtr MainWindowHandle;
    }
}