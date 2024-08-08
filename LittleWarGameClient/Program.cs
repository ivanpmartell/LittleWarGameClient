using CefSharp;
using CefSharp.DevTools.Overlay;
using LittleWarGameClient.Properties;
using Loyc.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LittleWarGameClient
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(CallBackPtr lpEnumFunc, IntPtr lParam);

        private delegate bool CallBackPtr(IntPtr hwnd, int lParam);

        private static CallBackPtr callBackPtr = Callback;
        private static List<WinStruct> _WinStructList = new List<WinStruct>();

        internal static bool IsDoubleInstance = false;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread splashthread = new Thread(() =>
            {
                SplashScreen.Instance.ShowDialog();
            });
            splashthread.IsBackground = true;
            splashthread.SetApartmentState(ApartmentState.STA);
            splashthread.Start();
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "Global\\LittleWarGameClient", out createdNew))
            {
                if (createdNew)
                {
                    // To customize application configuration such as set high DPI settings or default font,
                    // see https://aka.ms/applicationconfiguration.
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
                            var clientWindows = GetWindows(process.Handle).Where(window => window.WinTitle == "Littlewargame");
                            if (clientWindows.Count() > 0)
                            {
                                var clientMainWindow = clientWindows.First();
                                SetForegroundWindow(clientMainWindow.MainWindowHandle);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static bool Callback(IntPtr hWnd, int lparam)
        {
            StringBuilder sb = new StringBuilder(256);
            int res = GetWindowText(hWnd, sb, 256);
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